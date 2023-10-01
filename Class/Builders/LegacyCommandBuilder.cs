using System.Diagnostics.Metrics;
using System.Diagnostics;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace SharpMcAe.Class.Builders {
    //WIP
    class LegacyCommandBuilder : CommandBuilderBase<CommandColorblockSpeedExpression> {
        public new List<CommandColorblockSpeedExpression> Commands { get; set; } = new();

        public new List<CommandColorblockSpeedExpression> TempCommands { get; set; } = new();

        public override List<int> AverageCmdCount(int count, int t0, int t1, Func<double, double> fun = default!) {
            List<double> countList = new();
            int tick = t1 - t0 + 1;

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

            return countList.Select(x => (int)x).ToList();
        }

        /// <summary>
        /// 将指令列表转为t0-t1 Tick的序列并添加到总列表中
        /// </summary>
        /// <param name="t0">起始刻</param>
        /// <param name="t1">结束刻</param>
        /// <param name="functionMode">输出方法</param>
        /// <param name="firstCmd">第一秒的命令数量</param>
        /// <param name="changeCmd">每tick增加的的命令数量</param>
        public void CommandsToSequence(int t0, int t1, Func<double, double> fun = null, string functionMode = "average", double firstCmd = 0, double changeCmd = 1) {
            var cmds = this.TempCommands;
            int index1, index2;

            if (functionMode == "average") {
                if (t0 == t1) {
                    foreach (var cmd in cmds) {
                        cmd.Tick = t0;
                        this.Commands.Add(cmd);
                    }
                } else {
                    List<int> countList = AverageCmdCount(cmds.Count, t0, t1, fun);
                    index1 = index2 = 0;

                    for (int i = 0; i < countList.Count; i++) {
                        index2 += countList[i];

                        for (int j = index1; j < index2; j++) {
                            cmds[j].Tick = t0 + i;
                            this.Commands.Add(cmds[j]);
                        }

                        index1 = index2;
                    }
                }
            }

            if (functionMode == "forward" || functionMode == "backward") {
                if (t0 == t1) {
                    foreach (var cmd in cmds) {
                        cmd.Tick = t0;
                        this.Commands.Add(cmd);
                    }
                } else {
                    List<int> countList = ChangeCmdCount(cmds.Count, t0, t1, firstCmd, changeCmd, functionMode);
                    index1 = index2 = 0;

                    for (int i = 0; i < countList.Count; i++) {
                        index2 += countList[i];

                        for (int j = index1; j < index2; j++) {
                            cmds[j].Tick = t0 + i;
                            this.Commands.Add(cmds[j]);
                        }

                        index1 = index2;
                    }
                }
            }

            if (functionMode == "random") {
                if (t0 == t1) {
                    foreach (var cmd in cmds) {
                        cmd.Tick = t0;
                        this.Commands.Add(cmd);
                    }
                } else {
                    List<int> countList = AverageCmdCount(cmds.Count, t0, t1, fun);
                    index1 = index2 = 0;

                    for (int i = 0; i < countList.Count; i++) {
                        index2 += countList[i];

                        for (int j = index1; j < index2; j++) {
                            cmds[j].Tick = t0 + i;
                            this.Commands.Add(cmds[j]);
                        }

                        index1 = index2;
                    }
                }
            }

            this.TempCommands.Clear();
        }

        /// <summary>
        /// 获取颜色RGB码
        /// </summary>
        /// <returns></returns>
        public Color GetColor() {
            var random = new Random();
            var ran = random.NextDouble();
            List<Color> colorList = new()
            {
                new Color(1 - 0.1 * ran, 1 - 0.1 * ran, 1 - 0.1 * ran),
                new Color(1, 1, 0 + 0.1 * ran),
                new Color(1, 0 + 0.1 * ran, 0 + 0.1 * ran),
                new Color(0 + 0.1 * ran, 1, 0 + 0.1 * ran),
                new Color(0 + 0.1 * ran, 1, 1),
                new Color(0 + 0.1 * ran, 0 + 0.1 * ran, 1),
                new Color(1, 0 + 0.1 * ran, 1),
                new Color(1 - 0.1 * ran, 1, 0.4),
                new Color(1, 0, 0.5),
                new Color(0, 1 - 0.1 * ran, 0.5),
                new Color(1, 0.3, 0)
            };
 
            int g = random.Next(colorList.Count);
            return colorList[g];
        }

        /// <summary>
        /// 变速计算每tick的指令量
        /// </summary>
        /// <param name="count">指令列表长度</param>
        /// <param name="t0">起始时间</param>
        /// <param name="t1">结束时间</param>
        /// <param name="firstCmd">第一秒执行命令长度</param>
        /// <param name="changeCmd">执行命令加速度</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public List<int> ChangeCmdCount(int count, double t0, double t1, double firstCmd, double changeCmd, string mode = "forward") {
            List<int> countList = new List<int>();
            List<int> countsList = new List<int>();
            int tick = (int)(t1 - t0 + 1);
            double lengthCmd = count;
            double cmdNum;

            if (firstCmd.GetType() == typeof(int) || firstCmd.GetType() == typeof(float)) {
                if (mode == "forward") {
                    if (firstCmd == 0) {
                        firstCmd += 1;
                        cmdNum = (2 * count - 2 * firstCmd * tick) / Math.Pow(tick, 2);
                    } else {
                        cmdNum = (2 * count - 2 * firstCmd * tick) / Math.Pow(tick, 2);
                    }

                    for (int t = 0; t < tick; t++) {
                        int determine = (int)(firstCmd + 1);

                        if (determine > lengthCmd) {
                            firstCmd = lengthCmd;
                        }

                        countList.Add((int)Math.Round(firstCmd));
                        firstCmd += cmdNum;
                        lengthCmd -= firstCmd;
                    }
                }

                if (mode == "backward") {
                    if (firstCmd == 0) {
                        firstCmd += 1;
                        cmdNum = (2 * count - 2 * firstCmd * tick) / Math.Pow(tick, 2);
                    } else {
                        cmdNum = (2 * count - 2 * firstCmd * tick) / Math.Pow(tick, 2);
                    }

                    for (int t11 = 0; t11 < tick; t11++) {
                        int determine = (int)(firstCmd + 1);

                        if (determine > lengthCmd) {
                            firstCmd = lengthCmd;
                        }

                        countsList.Add((int)firstCmd);
                        firstCmd += cmdNum;
                        lengthCmd -= firstCmd;
                    }

                    int sigh = countsList.Count;

                    for (int t2 = 0; t2 < tick; t2++) {
                        countList.Add(countsList[sigh - t2 - 1]);
                    }
                }
            } else {
                throw new ArgumentException("格式有误，无法执行");
            }

            return countList;
        }

        /// <summary>
        /// 计算颜色
        /// </summary>
        /// <param name="colorMode"></param>
        /// <param name="colorSwitch"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public Color ColorCount(string colorMode, bool colorSwitch, double t) {
            if (colorSwitch) {
                if (colorMode == "normal") {
                    double cr = Math.Abs(Math.Round((Math.Sin((Math.PI / 180) * t) / 3) + 0.5, 2));
                    double cg = Math.Abs(Math.Round((Math.Cos((Math.PI / 180) * t) / 3) + 0.5, 2));
                    double cb = Math.Abs(Math.Round((Math.Sin((Math.PI / 180) * t) / 2) + 0.5, 2));

                    return new(cr, cg, cb);
                }
            }

            return default;
        }
        
        [Obsolete]
        public bool ListDetermine<T>(List<T> list) {
            foreach (var c in list) {
                if (c.GetType() != typeof(List<T>)) {
                    return false;
                }
            }

            return true;
        }

        public void StaticColorblockParticle(double t0, double t1, List<List<double>> inputPoints, string name, double dx, double dy, double dz, string functionMode = "average", string colorMode = "normal", string colorSwitch = "normal", double sx = 0, double sy = 0, double sz = 0, int count = 1, int age = 0, double firstCmd = 0, double changeCmd = 1, double colorSpeed = 1, double cr = 1, double cg = 1, double cb = 1, bool debug = false) {
            this.TempCommands.Clear();
            double color = 0;

            if (colorSwitch == "normal") {
                if (debug) {
                    Console.WriteLine(2);
                }

                foreach (var c in inputPoints) {
                    color += 1 + colorSpeed * new Random().NextDouble();
                    int lenList = c.Count;

                    if (lenList == 3) {
                        if (colorMode == "normal") {
                            Color colorTuple = ColorCount(colorMode, true, color);
                            double x = Convert.ToDouble(c[0]), y = Convert.ToDouble(c[1]), z = Convert.ToDouble(c[2]);

                            CommandColorblockSpeedExpression cmd = new(t0, name, x, y, z, colorTuple.R, colorTuple.G, colorTuple.B,
                                              1, sx, sy, sz, dx, dx, dz, count, age);
                            TempCommands.Add(cmd);
                        }
                    } else if (lenList == 6) {
                        if (colorMode == "normal") {
                            Color colorTuple = ColorCount(colorMode, true, color);
                            double x = Convert.ToDouble(c[0]), y = Convert.ToDouble(c[1]), z = Convert.ToDouble(c[2]);
                            sx = Convert.ToDouble(c[3]); sy = Convert.ToDouble(c[4]); sz = Convert.ToDouble(c[5]);

                            CommandColorblockSpeedExpression cmd = new(t0, name, x, y, z, colorTuple.R, colorTuple.G, colorTuple.B,
                                            1, sx, sy, sz, dx, dx, dz, count, age);
                            TempCommands.Add(cmd);
                        }
                    } else if (lenList == 8) {
                        if (colorMode == "normal") {
                            var colorTuple = ColorCount(colorMode, true, color);
                            double x = Convert.ToDouble(c[0]), y = Convert.ToDouble(c[1]), z = Convert.ToDouble(c[2]);
                            sx = Convert.ToDouble(c[3]); sy = Convert.ToDouble(c[4]); sz = Convert.ToDouble(c[5]);
                            count = Convert.ToInt32(c[6]); age = Convert.ToInt32(c[7]);

                            CommandColorblockSpeedExpression cmd = new(t0, name, x, y, z, colorTuple.R, colorTuple.G, colorTuple.B,
                                            1, sx, sy, sz, dx, dx, dz, count, age);
                            TempCommands.Add(cmd);
                        }
                    } else {
                        continue;
                    }
                }
            }
        }
    }

    public enum ColorSwitch {
        Close,
        Normal,
        Custom,
        Picture,
    }

    /// <summary>
    /// 粒子参数类
    /// </summary>
    public class ParticleParameters {
        /// <summary>
        /// 起始时间
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public double EndTime { get; set; }

        /// <summary>
        /// 输入的点列表
        /// </summary>
        public List<List<double>> InputPoints { get; set; }

        /// <summary>
        /// 粒子名称
        /// </summary>
        public string ParticleName { get; set; }

        /// <summary>
        /// x方向范围
        /// </summary>
        public double RangeX { get; set; }

        /// <summary>
        /// y方向范围
        /// </summary>
        public double RangeY { get; set; }

        /// <summary>
        /// z方向范围
        /// </summary>
        public double RangeZ { get; set; }

        /// <summary>
        /// 输出方式，默认为'average'
        /// </summary>
        public string FunctionMode { get; set; } = "average";

        /// <summary>
        /// 颜色方式，默认为'normal'
        /// </summary>
        public string ColorMode { get; set; } = "normal";

        /// <summary>
        /// 颜色开关，默认为'normal'
        /// </summary>
        public ColorSwitch ColorSwitch { get; set; } = ColorSwitch.Normal;

        /// <summary>
        /// x方向速度，默认为0
        /// </summary>
        public double SpeedX { get; set; } = 0;

        /// <summary>
        /// y方向速度，默认为0
        /// </summary>
        public double SpeedY { get; set; } = 0;

        /// <summary>
        /// z方向速度，默认为0
        /// </summary>
        public double SpeedZ { get; set; } = 0;

        /// <summary>
        /// 粒子数量，默认为1
        /// </summary>
        public int ParticleCount { get; set; } = 1;

        // 粒子寿命，默认为0
        public int ParticleAge { get; set; } = 0;

        // 第一秒的粒子量，默认为0
        public double InitialParticleCount { get; set; } = 0;

        // 每tick改变的粒子量，默认为1
        public double ChangeInParticleCountPerTick { get; set; } = 1;

        // 颜色速度，默认为1
        public double ColorSpeed { get; set; } = 1;

        // 红色分量，默认为1
        public double RedComponent { get; set; } = 1;

        // 绿色分量，默认为1
        public double GreenComponent { get; set; } = 1;

        // 蓝色分量，默认为1
        public double BlueComponent { get; set; } = 1;
    }

    public class CommandColorblockSpeedExpression {
        public double Tick { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Cr { get; set; }
        public double Cg { get; set; }
        public double Cb { get; set; }
        public double Cl { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public double Sx { get; set; }
        public double Sy { get; set; }
        public double Sz { get; set; }
        public int Counts { get; set; }
        public int Age { get; set; }
        public string SpeedExpression { get; set; }

        public CommandColorblockSpeedExpression(double tick, string name, double x, double y, double z,
                                                double cr, double cg, double cb, double cl,
                                                double sx, double sy, double sz,
                                                double dx, double dy, double dz,
                                                int counts, int age, string speedExpression = null) {
            Tick = tick;
            Name = name;
            X = Math.Round(x, 2);
            Y = Math.Round(y, 2);
            Z = Math.Round(z, 2);
            Cr = Math.Round(cr, 2);
            Cg = Math.Round(cg, 2);
            Cb = Math.Round(cb, 2);
            Cl = Math.Round(cl, 2);
            Dx = Math.Round(dx, 2);
            Dy = Math.Round(dy, 2);
            Dz = Math.Round(dz, 2);
            Sx = Math.Round(sx, 2);
            Sy = Math.Round(sy, 2);
            Sz = Math.Round(sz, 2);
            Counts = counts;
            Age = age;
            SpeedExpression = speedExpression;
        }

        public override string ToString() {
            if (SpeedExpression == null) {
                return $"particleex normal {Name} {X} {Y} {Z} {Cr} {Cg} {Cb} {Cl} 240 " +
                       $"{Sx} {Sy} {Sz} {Dx} {Dy} {Dz} {Counts} {Age}";
            }

            return $"particleex normal {Name} {X} {Y} {Z} {Cr} {Cg} {Cb} {Cl} 240 " +
                   $"{Sx} {Sy} {Sz} {Dx} {Dy} {Dz} {Counts} " +
                   $"{Age} " + SpeedExpression;
        }
    }
}
