namespace SharpMcAe.Class.Builders {
    /// <summary>
    /// 命令构建器基类
    /// </summary>
    public abstract class CommandBuilderBase<T> {
        public virtual List<ParticleCommand> Commands { get; set; } = new();

        public virtual List<ParticleCommand> TempCommands { get; set; } = new();

        /// <summary>
        /// 计算每 Tick 指令数
        /// </summary>
        /// <returns></returns>
        public virtual List<int> AverageCmdCount(int count, int t0, int t1, Func<double, double> fun = default!) {
            List<double> countList = new();
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
        public virtual void CommandsToSequence(int t0, int t1, Func<double, double> fun = null) {
            List<ParticleCommand> cmds = TempCommands;
            if (t0 == t1) {
                foreach (ParticleCommand cmd in cmds) {
                    cmd.Tick = t0;
                    Commands.Add(cmd);
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
                        Commands.Add(cmd);
                    }
                    index1 = index2;
                }
            }

            TempCommands.Clear();
        }
    }
}
