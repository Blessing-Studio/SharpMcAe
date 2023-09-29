﻿using SharpMcAe.Class;
using SharpMcAe.Class.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMcAe {
    /// <summary>
    /// 命令构建器
    /// </summary>
    public class CommandBuilder {
        public List<ParticleCommand> Commands { get; set; } = new();

        public List<ParticleCommand> TempCommands { get; set; } = new();

        /// <summary>
        /// 计算每 Tick 指令数
        /// </summary>
        /// <returns></returns>
        public List<int> AverageCmdCount(int count, int t0, int t1, Func<double, double> fun) {
            List<double> countList = new List<double>();
            int tick = t1 - t0 + 1;
            if (fun != null) {
                double sum = 0;
                for (int t = t0; t <= t1; t++) {
                    var n = fun(t);
                    countList.Add(n * count);
                    sum += n;
                }
                for (int t = 0; t < tick; t++) {
                    countList[t] /= sum;
                }
            } else {
                int i = count / tick;

                int r = count % tick;
                for (int n = 0; n < tick; n++) {
                    countList.Add(i);
                }

                for (int n = 0; n < r; n++) {
                    if (Math.Round((double)tick / r) * n < countList.Count) {
                        countList[(int)Math.Round((double)tick / r) * n] += 1;
                    }
                }
            }

            return countList.Select(x => (int)x).ToList();
        }
        
        /// <summary>
        /// 将指令列表转为 t0-t1 Tick 的序列并添加到总列表中
        /// </summary>
        /// <param name="t0">起始刻</param>
        /// <param name="t1">结束刻</param>
        public void CommandsToSequence(int t0, int t1, Func<double, double> fun = null) {
            List<ParticleCommand> cmds = this.TempCommands;
            if (t0 == t1) {
                foreach (ParticleCommand cmd in cmds) {
                    cmd.Tick = t0;
                    this.Commands.Add(cmd);
                }
            } else {
                List<int> countList = AverageCmdCount(cmds.Count, t0, t1, fun);
                // 按每tick粒子数量切片列表
                int index1 = 0, index2 = 0;
                for (int i = 0; i < countList.Count; i++) {
                    int c = countList[i];
                    index2 += c;
                    for (int j = index1; j < index2; j++) {
                        ParticleCommand cmd = cmds[j];
                        cmd.Tick = t0 + i;
                        this.Commands.Add(cmd);
                    }
                    index1 = index2;
                }
            }
            this.TempCommands.Clear();
        }

        /// <summary>
        /// 生成普通粒子序列
        /// </summary>
        public void StaticParticle(int t0, int t1, List<List<double>> points, string name, double dx, double dy, double dz, double speed, int amount) {
            this.TempCommands.Clear();
            foreach (List<double> p in points) {
                ParticleCommand cmd = new(t0, name, p[0], p[1], p[2], dx, dy, dz, speed, amount);
                this.TempCommands.Add(cmd);
            }

            CommandsToSequence(t0, t1);
        }

        /// <summary>
        /// 生成带初速度的粒子序列，每个粒子具有指定初速度
        /// </summary>
        /// <param name="points"></param>
        /// <param name="motions"></param>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="name"></param>
        /// <param name="speed"></param>
        public void MotionParticle(List<List<double>> points, List<List<double>> motions, int t0, int t1, string name, double speed) {
            this.TempCommands.Clear();
            for (int i = 0; i < points.Count; i++) {
                List<double> p = points[i];
                List<double> m = motions[i];
                double x = p[0], y = p[1], z = p[2];
                double dx = m[0], dy = m[1], dz = m[2];
                var cmd = new ParticleCommand(t0, name, x, y, z, dx, dy, dz, speed, 0);
                this.TempCommands.Add(cmd);
            }

            CommandsToSequence(t0, t1);
        }

        /// <summary>
        /// 生成粒子移动动画
        /// </summary>
        public void MotionMove(List<List<double>> points1, List<List<double>> points2, int t0, int t1, string name, double speed, double zoom = 11) {
            List<List<double>> motions = new List<List<double>>();
            for (int i = 0; i < points1.Count; i++) {
                List<double> p1 = points1[i];
                List<double> p2 = points2[i];
                List<double> motion = new List<double> { (p2[0] - p1[0]) / zoom, (p2[1] - p1[1]) / zoom, (p2[2] - p1[2]) / zoom };
                motions.Add(motion);
            }
            MotionParticle(points1, motions, t0, t1, name, speed);
        }

        /// <summary>
        /// 生成粒子扩散动画
        /// </summary>
        public void MotionSpreadFromPoint(List<List<double>> points, double x, double y, double z, int t0, int t1, string name, double speed, double zoom = 11) {
            List<List<double>> motions = new List<List<double>>();
            foreach (List<double> point in points) {
                List<double> motion = new List<double> { (point[0] - x) / zoom, (point[1] - y) / zoom, (point[2] - z) / zoom };
                motions.Add(motion);
            }
            List<List<double>> pointsForMotion = Enumerable.Repeat(new List<double> { x, y, z }, motions.Count).ToList();
            MotionParticle(pointsForMotion, motions, t0, t1, name, speed);
        }

        /// <summary>
        /// 生成粒子中心扩散动画
        /// </summary>
        public void MotionCentreSpread(List<List<double>> points, int t0, int t1, string name, double speed, double zoom = 11) {
            DrawUtil du = new();
            List<double> midpoint = du.GetMidpoint(points);
            double x = midpoint[0], y = midpoint[1], z = midpoint[2];
            MotionSpreadFromPoint(points, x, y, z, t0, t1, name, speed, zoom);
        }

        /// <summary>
        /// 生成粒子收缩动画
        /// </summary>
        public void MotionShrinkToPoint(List<List<double>> points, double x, double y, double z, int t0, int t1, string name, double speed, double zoom = 11) {
            List<List<double>> motions = new();
            foreach (List<double> point in points) {
                List<double> motion = new() { (x - point[0]) / zoom, (y - point[1]) / zoom, (z - point[2]) / zoom };
                motions.Add(motion);
            }

            MotionParticle(points, motions, t0, t1, name, speed);
        }

        /// <summary>
        /// 生成粒子中心收缩动画
        /// </summary>
        public void MotionCentreShrink(List<List<double>> points, int t0, int t1, string name, double speed, double zoom = 11) {
            DrawUtil du = new();
            List<double> midpoint = du.GetMidpoint(points);
            double x = midpoint[0], y = midpoint[1], z = midpoint[2];
            MotionShrinkToPoint(points, x, y, z, t0, t1, name, speed, zoom);
        }

        /// <summary>
        /// 以参数方程形式生成命令
        /// </summary>
        public void CommandBuild(int t0, int t1, Func<double, List<double>> fun, string name, double dx, double dy, double dz, double speed, int amount, int ppt = 3) {
            DrawUtil du = new();
            double dt = 1.0 / ppt;
            for (int tick = t0; tick < t1; tick++) {
                for (int i = 0; i < ppt; i++) {
                    List<List<double>> points = du.ArrayTran(fun(tick + i * dt));
                    foreach (List<double> p in points) {
                        ParticleCommand cmd = new(tick, name, p[0], p[1], p[2], dx, dy, dz, speed, amount);
                        this.Commands.Add(cmd);
                    }
                }
            }
        }

        /// <summary>
        /// 生成普通粒子序列,时间的浓度方程版
        /// </summary>
        public void StaticParticleBuild(int t0, int t1, List<List<double>> points, string name, double dx, double dy, double dz, double speed, int amount, Func<double, double> fun) {
            this.TempCommands.Clear();
            foreach (List<double> p in points) {
                var cmd = new ParticleCommand(t0, name, p[0], p[1], p[2], dx, dy, dz, speed, amount);
                this.TempCommands.Add(cmd);
            }
            CommandsToSequence(t0, t1, fun);
        }
    }
}
