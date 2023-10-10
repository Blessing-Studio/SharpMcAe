using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMcAe.Class.Builders
{
    public record ColorBlockL : ColorBlock
    {
        public ColorBlockL(double tick, string name, double x, double y, double z, double cr, double cg, double cb, double sx, double sy, double sz, double dx, double dy, double dz, double counts, double age)
        : base(tick, name, x, y, z, cr, cg, cb, sx, sy, sz, dx, dy, dz, counts, age) { }

        public override string ToString()
        {
            return $"particleex normal {this.Name} {this.X} {this.Y} {this.Z} {this.Cr} {this.Cg} {this.Cb} 1 240 {this.Sx} {this.Sy} {this.Sz} {this.Dx} {this.Dy} {this.Dz} {this.Counts} {this.Age}";
        }
    }

    public record ColorBlockLSpeedExpression : ColorBlockL
    {
        public double Cl { get; set; }

        public dynamic? SpeedExpression { get; set; }

        public ColorBlockLSpeedExpression(double tick, string name, double x, double y, double z, double cr, double cg, double cl, double cb, double sx, double sy, double sz, double dx, double dy, double dz, double counts, double age, dynamic? speedExpression) : base(tick, name, x, y, z, cr, cg, cb, sx, sy, sz, dx, dy, dz, counts, age) 
        {
            Cl = Math.Round(cl, 2);
            SpeedExpression = speedExpression;
        }

        public override string ToString()
        {
            var text = $"particleex normal {this.Name} {this.X} {this.Y} {this.Z} {this.Cr} {this.Cg} {this.Cb} {this.Cl} 240 {this.Sx} {this.Sy} {this.Sz} {this.Dx} {this.Dy} {this.Dz} {this.Counts} {this.Age}";
            return (SpeedExpression == null ? text : $"{text} {this.SpeedExpression}");
        }
    }


    public class LowVersionCommandBuilder
    {
        public List<dynamic> TempCommands { get; set; } = new();

        public List<dynamic> Commands { get; set; } = new();

        public static Tuple<double, double, double> GetColor()
        {
            var ran = new Random().NextDouble();
            List<List<double>> colorList = new()
            {
                new(){ 1 - 0.1 * ran, 1 - 0.1 * ran, 1 - 0.1 * ran},
                new(){ 1, 1, 0 + 0.1 * ran},
                new(){ 1, 0 + 0.1 * ran, 0 + 0.1 * ran },
                new(){ 0 + 0.1 * ran, 1, 0 + 0.1 * ran },
                new(){ 0 + 0.1 * ran, 1, 1 },
                new(){ 0 + 0.1 * ran, 0 + 0.1 * ran, 1 },
                new(){ 1, 0 + 0.1 * ran, 1 },
                new(){ 1 - 0.1 * ran, 1, 0.4 },
                new(){ 1, 0, 0.5 },
                new(){ 0, 1 - 0.1 * ran, 0.5 },
                new(){ 1, 0.3, 0 }
            };
            int g = Convert.ToInt32(colorList.Count * new Random().NextDouble());
            (double cr, double cg, double cb) = (colorList[g][0], colorList[g][1], colorList[g][2]);
            return new(cr, cg, cb);
        }

        public List<int> AverageCmdCount(int count, double t0, double t1)
        {
            List<double> countList = new();
            double tick = t1 - t0 + 1;
            double i = count / tick;

            double r = count % tick;
            for (int n = 0; n < tick; n++)
            {
                countList.Add(i);
            }

            for (int n = 0; n < r; n++)
            {
                if (Math.Round((double)tick / r) * n < countList.Count)
                {
                    countList[(int)Math.Round((double)tick / r) * n] += 1;
                }
            }
            return countList.Select(x => (int)x).ToList();
        }

        public static List<double> ChangeCommandCount(int count, double t0, double t1, double firstCommand, string mode = "forward")
        {
            List<double> countList = new();
            List<double> countsList = new();
            int tick = (int)(t1 - t0 + 1);
            double commandLength = count;

            var commandNum = (2 * count - 2 * (firstCommand == 0 ? firstCommand + 1 : firstCommand) * tick) / (tick * tick);

            if (mode == "forward")
            {
                for (int t = 0; t < tick; t++)
                {
                    var deterMine = firstCommand + 1;
                    if (deterMine > (int)commandLength) firstCommand = commandLength;
                    countList.Add((int)Math.Round(firstCommand, 0));
                    firstCommand += commandNum;
                    commandLength -= firstCommand;
                }
            }
            if (mode == "backward")
            {
                for (int t = 0; t < tick; t++)
                {
                    var deterMine = firstCommand + 1;
                    if (deterMine > (int)commandLength) firstCommand = commandLength;
                    countList.Add((int)Math.Round(firstCommand, 0));
                    firstCommand += commandNum;
                    commandLength -= firstCommand;
                }

                int sigh = countsList.Count;
                for (int t = 0; t < tick; t++)
                {
                    countList.Add(countsList[sigh - t - 1]);
                }
            }
            return countList;
        }

        public void CommandsToSequence(double t0, double t1, string funcMode = "average", double firstCommand = 0, double changeCommand = 1)
        {
            var commands = TempCommands;
            switch (funcMode)
            {
                case "average":
                    if (t0 == t1)
                    {
                        foreach (var command in commands)
                        {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    }
                    else
                    {
                        var countList = AverageCmdCount(commands.Count, t0, t1);
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++)
                        {
                            int c = countList[i];
                            index2 += c;
                            for (int j = index1; j < index2; j++)
                            {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                            index1 = index2;
                        }
                    }
                    break;
                case "forward":
                    if (t0 == t1)
                    {
                        foreach (var command in commands)
                        {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    }
                    else
                    {
                        var countList = ChangeCommandCount(commands.Count, t0, t1, firstCommand, "forward");
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++)
                        {
                            double c = countList[i];
                            index2 += (int)c;
                            for (int j = index1; j < index2; j++)
                            {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                            index1 = index2;
                        }
                    }
                    break;
                case "backward":
                    if (t0 == t1)
                    {
                        foreach (var command in commands)
                        {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    }
                    else
                    {
                        var countList = ChangeCommandCount(commands.Count, t0, t1, firstCommand, "backward");
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++)
                        {
                            double c = countList[i];
                            index2 += (int)c;
                            for (int j = index1; j < index2; j++)
                            {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                            index1 = index2;
                        }
                    }
                    break;
                case "random":
                    if (t0 == t1)
                    {
                        foreach (var command in commands)
                        {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    }
                    else
                    {
                        var countList = AverageCmdCount(commands.Count, t0, t1);
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++)
                        {
                            double c = countList[i];
                            index2 += (int)c;
                            for (int j = index1; j < index2; j++)
                            {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                        }
                    }
                    break;
            }
            TempCommands.Clear();
        }

        public Tuple<double, double, double>? ColorCount(string colorMode, string colorSwitch, double t)
        {
            if (!string.IsNullOrEmpty(colorSwitch) && colorMode == "normal")
            {
                var cr = Math.Abs(Math.Round((Math.Sin((Math.PI / 180) * t) / 3) + 0.5, 2));
                var cg = Math.Abs(Math.Round((Math.Cos((Math.PI / 180) * t) / 3) + 0.5, 2));
                var cb = Math.Abs(Math.Round((Math.Sin((Math.PI / 180) * t) / 2) + 0.5, 2));
                return new(cr, cg, cb);

            }
            else return null;
        }

        public void StaticColorBlockParticle()
        {
            //TODO:下次再写，先咕一会儿（doge
        }

        public void BlockPlace(List<List<double>> blockPoints, List<List<double>> placePoints, double t0, double t1, double firstCommand = 0, double changeCommand = 1, string funcMode = "average")
        {
            var counts = 0;
            foreach(var point in blockPoints)
            {
                (double dx, double dy, double dz) = (point[0], point[1], point[2]);
                (double x, double y, double  z) = (placePoints[counts][0], placePoints[counts][1], placePoints[counts][2]);
                var cmd = new BlockClone(x, y, z, dx, dy, dz);
                TempCommands.Add(cmd);
                counts++;
            }
            CommandsToSequence(t0, t1, funcMode, firstCommand, changeCommand);
        } 
    }
}
