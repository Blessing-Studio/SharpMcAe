using SharpMcAe.Class;

namespace SharpMcAe {
    /// <summary>
    /// Minecraft函数序列管理类
    /// </summary>
    public class McFunctionSequenceManager {
        private int index = 0;

        private List<List<Command>> commandsList = new List<List<Command>>();

        private int GetSequenceDuration(IEnumerable<Command> cmds) => cmds.Max(cmd => cmd.Tick);

        private void CreateNullSequence(IEnumerable<Command> cmds) {
            int seqDuration = GetSequenceDuration(cmds);
            int dx = seqDuration - commandsList.Count;
            if (dx >= 0) {
                commandsList.AddRange(Enumerable.Range(0, dx + 1).Select(_ => new List<Command>()));
            }
        }

        /// <summary>
        /// 添加命令列表到总序列
        /// </summary>
        /// <param name="cmds">命令列表</param>
        public void AddCommand(IEnumerable<Command> commands) {
            CreateNullSequence(commands);
            foreach (var command in commands) {
                commandsList[command.Tick].Add(command);
            }
        }

        /// <summary>
        /// 在每tick添加自定义的指令。如 每tick tp玩家一次
        /// </summary>
        /// <param name="loopCmds">命令列表</param>
        public void AddCustomLoopCommands(IEnumerable<Command> loopCommands) {
            foreach (var commands in commandsList) {
                commands.AddRange(loopCommands);
            }
        }

        /// <summary>
        /// 导出函数序列
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        /// <param name="isDebug">是否调试模式（聊天栏回显当前命令）</param>
        /// <param name="buildSchedule">是否生成schedule</param>
        /// <param name="namespace_">数据包命名空间</param>
        public void SaveSequenceFile(string folder, bool isDebug = false, bool buildSchedule = true, string namespace_ = "") {
            Directory.CreateDirectory($"./{folder}");
            var scheduleTick = new List<int>();
            foreach (var cmds in commandsList.Where(cmds => cmds.Any())) {
                File.WriteAllLines($"./{folder}/{index}.mcfunction",
                    cmds.Select(cmd => cmd.ToString())
                        .Concat(isDebug ? cmds.Select(cmd => $"tellraw @p {{\"text\":\"Debug:{cmd}\",\"color\":\"aqua\"}}") : Enumerable.Empty<string>()));
                scheduleTick.Add(cmds.First().Tick);
                index++;
            }
            index = 0;
            if (buildSchedule) {
                File.WriteAllLines($"./{folder}/schedule.mcfunction",
                    scheduleTick.Select(st => $"schedule function {namespace_}:{folder}/{st} {st}"));
            }
        }

        /// <summary>
        /// 将所有指令保存为一个文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="filename">文件名</param>
        public void SaveSingleFile(string folder, string filename) {
            Directory.CreateDirectory($"./{folder}");

            File.WriteAllLines($"./{folder}/{filename}.mcfunction",
                commandsList.SelectMany(cmds => cmds.Select(cmd => cmd.ToString())));
        }

        /// <summary>
        /// 导出调用函数序列的命令方块序列生成函数, 并在总序列中写入调用下一个函数用的命令
        /// </summary>
        /// <param name="namespace_">命名空间</param>
        /// <param name="folder">文件夹名</param>
        /// <param name="x">起始x</param>
        /// <param name="y">起始y</param>
        /// <param name="z">起始z</param>
        /// <param name="facing">朝向</param>
        /// <param name="maxLength">最大长度</param>
        /// <param name="maxWidth">最大宽度</param>
        public void OutputCbSequenceFunction(string namespace_, string folder, int x, int y, int z, string facing, int maxLength, int maxWidth) {
            var cmds = new List<Command>();
            for (int i = 0; i < commandsList.Count; i++) {
                cmds.Add(new(0, x, y, z, $"minecraft:command_block{{Command:\"function {namespace_}:{folder}/{i}\"}}"));
                if (facing == "x+") {
                    commandsList.ElementAt(i).Add(new(i, x - 1, y, z, "air"));
                    z = z + (i + 1) % maxLength;
                    y = y + ((i + 1) / maxLength) % maxWidth;
                    x = x + (((i + 1) / maxLength) / maxWidth) * 3;
                    commandsList.ElementAt(i).Add(new(i, x - 1, y, z, "minecraft:redstone_block"));
                } else if (facing == "y+") {
                    commandsList.ElementAt(i).Add(new Command(i, x, y - 1, z, "air"));
                    x = x + (i + 1) % maxLength;
                    z = z + ((i + 1) / maxLength) % maxWidth;
                    y = y + (((i + 1) / maxLength) / maxWidth) * 3;
                    commandsList.ElementAt(i).Add(new Command(i, x, y - 1, z, "minecraft:redstone_block"));
                } else if (facing == "z+") {
                    commandsList.ElementAt(i).Add(new Command(i, x, y, z - 1, "air"));
                    x = x + (i + 1) % maxLength;
                    y = y + ((i + 1) / maxLength) % maxWidth;
                    z = z + (((i + 1) / maxLength) / maxWidth) * 3;
                    commandsList.ElementAt(i).Add(new Command(i, x, y, z - 1, "minecraft:redstone_block"));
                }
            }

            Directory.CreateDirectory($"./{folder}");
            File.WriteAllLines($"./{folder}/build_cb_seq.mcfunction", cmds.Select(cmd => cmd.ToString()));
        }
    }
}
