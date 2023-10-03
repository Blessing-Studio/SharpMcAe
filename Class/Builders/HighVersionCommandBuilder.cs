using System;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SharpMcAe.Class.Builders {
    public record ColorBlock : Command {
        public double Tick { get; set; }

        public string Name { get; set; }

        public double Dx { get; set; }

        public double Dy { get; set; }

        public double Dz { get; set; }

        public double Counts { get; set; }

        public int Age { get; set; }

        public double Cr { get; set; }

        public double Cg { get; set; }

        public double Cb { get; set; }

        public double Sx { get; set; }

        public double Sy { get; set; }

        public double Sz { get; set; }
        public ColorBlock(double tick, string name, double x, double y, double z, double cr, double cg, double cb, double sx, double sy, double sz, double dx, double dy, double dz, double counts, double age)
        : base(x, y, z) {
            Tick = tick;
            Name = name;
            Cr = Math.Round(cr, 2);
            Cg = Math.Round(cg, 2);
            Cb = Math.Round(cb, 2);
            Dx = Math.Round(dx, 2);
            Dy = Math.Round(dy, 2);
            Dz = Math.Round(dz, 2);
            Sx = Math.Round(sx, 2);
            Sy = Math.Round(sy, 2);
            Sz = Math.Round(sz, 2);
            Counts = counts;
            Age = Convert.ToInt16(age);
        }

        public override string ToString() {
            return $"particleex normal {this.Name} {this.X} {this.Y} {this.Z} {this.Cr} {this.Cg} {this.Cb} 1 {this.Sx} {this.Sy} {this.Sz} {this.Dx} {this.Dy} {this.Dz} {this.Counts} {this.Age}";
        }
    }

    public record BlockClone : Command {
        public int Dx { get; set; }
        public int Dy { get; set; }
        public int Dz { get; set; }

        public BlockClone(double x, double y, double z, double dx, double dy, double dz) : base(x, y, z) {
            Dx = (int)dx;
            Dy = (int)dy;
            Dz = (int)dz;
        }

        public override string ToString() {
            return $"clone {this.Dx} {this.Dy} {this.Dz} {this.Dx} {this.Dx} {this.Dy} {this.Dz} {this.X} {this.Y} {this.Z} masked";
        }
    }

    public record ColorBlockSpeedExpression : ColorBlock {
        public double Cl { get; set; }

        public dynamic? SpeedExpression { get; set; }

        public ColorBlockSpeedExpression(double tick, string name, double x, double y, double z, double cr, double cg, double cl, double cb, double sx, double sy, double sz, double dx, double dy, double dz, double counts, double age, dynamic? speedExpression) : base(tick, name, x, y, z, cr, cg, cb, sx, sy, sz, dx, dy, dz, counts, age) {
            Cl = Math.Round(cl, 2);
            SpeedExpression = speedExpression;
        }

        public override string ToString() {
            var text = $"particleex normal {this.Name} {this.X} {this.Y} {this.Z} {this.Cr} {this.Cg} {this.Cb} {this.Cl} {this.Sx} {this.Sy} {this.Sz} {this.Dx} {this.Dy} {this.Dz} {this.Counts} {this.Age}";
            return (SpeedExpression == null ? text : $"{text} {this.SpeedExpression}");
        }
    }

    public record Block : Command {
        public string? Name { get; set; }

        public Block(double x, double y, double z, string name) : base((int)x, (int)y, (int)z) {
            Name = name;
        }

        public override string ToString() {
            return $"setblock {this.X} {this.Y} {this.Z} {this.Name} destroy";
        }
    }

    public record FallingBlock : Command {
        public string? Name { get; set; }

        public double MotionX { get; set; }

        public double MotionY { get; set; }

        public double MotionZ { get; set; }

        public int Gravity { get; set; }

        public int Visible { get; set; }

        public int Time { get; set; }

        public FallingBlock(double x, double y, double z, string name, double time, double motionX, double motionY, double motionZ, int gravity, int visible) : base(x, y, z) {
            Name = name;
            Time = (int)time;
            MotionX = Math.Round(motionX, 2);
            MotionY = Math.Round(motionY, 2);
            MotionZ = Math.Round(motionZ, 2);
            Gravity = gravity;
            Visible = visible;
        }

        public override string ToString() {
            return "summon falling_block " + $"{this.X} {this.Y} {this.Z} " + "{{BlockState:{{Name:\"" + Name + "\"}},Time:" + Time + $",Motion:[{MotionX}d,{MotionY}d,{MotionZ}sd],NoGravity:{Gravity},Invisible:" + Visible + "}}";
        }
    }

    public class HighVersionCommandBuilder {
        public static string AddColorfulExpression(bool boolean, double speed) {
            (double sinXT, double cosYT, double cosZT) = (speed * 6, speed * 4, speed * 3);
            return (boolean ? $"'(cr,cg,cb)=(sin(t/{sinXT})/4+0.65,cos(t/{cosYT})/3+0.6,cos(t/{cosZT})/3+0.6)'" : "");
        }

        public List<dynamic> TempCommands { get; set; } = new();

        public List<ColorBlockSpeedExpression> Commands { get; set; } = new();

        public static Tuple<double, double, double>? GetColor(string mode = "none", int customColor = 3) {
            var ran = new Random();
            List<List<double>> colorList = new()
            {
                new(){1,1,0+0.1*ran.NextDouble()},
                new(){1,0+0.1*ran.NextDouble(), 0+0.1*ran.NextDouble()},
                new(){ 0 + 0.1 * ran.NextDouble(), 1, 0 + 0.1 * ran.NextDouble() },
                new(){ 0 + 0.1 * ran.NextDouble(), 1, 1 },
                new(){ 0 + 0.1 * ran.NextDouble(), 0 + 0.1 * ran.NextDouble(), 1 },
                new(){1, 0 + 0.1 * ran.NextDouble(), 1 },
                new(){ 1 - 0.1 * ran.NextDouble(), 0.4 },
                new(){ 1, 0, 0.5 },
                new(){ 0, 1 - 0.1 * ran.NextDouble(), 0.5 },
                new(){ 1, 0.3, 0 }
            };
            if (mode == "none") {
                int g = (int)(colorList.Count * ran.NextDouble());
                (double cr, double cg, double cb) = (colorList[g][0], colorList[g][1], colorList[g][2]);
                return new(cr, cg, cb);
            } else if (mode == "custom") {
                (double cr, double cg, double cb) = (colorList[customColor][0], colorList[customColor][1], colorList[customColor][2]);
                return new(cr, cg, cb);
            }
            return null;
        }

        public List<int> AverageCmdCount(int count, double t0, double t1) {
            List<double> countList = new();
            double tick = t1 - t0 + 1;
            double i = count / tick;

            double r = count % tick;
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

        public static List<double> ChangeCommandCount(int count, double t0, double t1, double firstCommand, string mode = "forward") {
            List<double> countList = new();
            List<double> countsList = new();
            int tick = (int)(t1 - t0 + 1);
            double commandLength = count;

            var commandNum = (2 * count - 2 * (firstCommand == 0 ? firstCommand + 1 : firstCommand) * tick) / (tick * tick);

            if (mode == "forward") {
                for (int t = 0; t < tick; t++) {
                    var deterMine = firstCommand + 1;
                    if (deterMine > (int)commandLength) firstCommand = commandLength;
                    countList.Add((int)Math.Round(firstCommand, 0));
                    firstCommand += commandNum;
                    commandLength -= firstCommand;
                }
            }
            if (mode == "backward") {
                for (int t = 0; t < tick; t++) {
                    var deterMine = firstCommand + 1;
                    if (deterMine > (int)commandLength) firstCommand = commandLength;
                    countList.Add((int)Math.Round(firstCommand, 0));
                    firstCommand += commandNum;
                    commandLength -= firstCommand;
                }

                int sigh = countsList.Count;
                for (int t = 0; t < tick; t++) {
                    countList.Add(countsList[sigh - t - 1]);
                }
            }
            return countList;
        }

        public void CommandsToSequence(double t0, double t1, string funcMode = "average", double firstCommand = 0, double changeCommand = 1) {
            var commands = TempCommands;
            switch (funcMode) {
                case "average":
                    if (t0 == t1) {
                        foreach (var command in commands) {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    } else {
                        var countList = AverageCmdCount(commands.Count, t0, t1);
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++) {
                            int c = countList[i];
                            index2 += c;
                            for (int j = index1; j < index2; j++) {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                            index1 = index2;
                        }
                    }
                    break;
                case "forward":
                    if (t0 == t1) {
                        foreach (var command in commands) {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    } else {
                        var countList = ChangeCommandCount(commands.Count, t0, t1, firstCommand, "forward");
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++) {
                            double c = countList[i];
                            index2 += (int)c;
                            for (int j = index1; j < index2; j++) {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                            index1 = index2;
                        }
                    }
                    break;
                case "backward":
                    if (t0 == t1) {
                        foreach (var command in commands) {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    } else {
                        var countList = ChangeCommandCount(commands.Count, t0, t1, firstCommand, "backward");
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++) {
                            double c = countList[i];
                            index2 += (int)c;
                            for (int j = index1; j < index2; j++) {
                                var cmd = commands[j];
                                cmd.Tick = t0 + i;
                                Commands.Add(cmd);
                            }
                            index1 = index2;
                        }
                    }
                    break;
                case "random":
                    if (t0 == t1) {
                        foreach (var command in commands) {
                            command.Tick = t0;
                            Commands.Add(command);
                        }
                    } else {
                        var countList = AverageCmdCount(commands.Count, t0, t1);
                        (int index1, int index2) = (0, 0);
                        for (int i = 0; i < countList.Count; i++) {
                            double c = countList[i];
                            index2 += (int)c;
                            for (int j = index1; j < index2; j++) {
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

        public Tuple<double, double, double> ColorCount(double t) {
            var cr = Math.Abs(Math.Round((Math.Sin((Math.PI / 180) * (t / 6)) / 4) + 0.65, 2));
            var cg = Math.Abs(Math.Round((Math.Cos((Math.PI / 180) * (t / 4)) / 3) + 0.6, 2));
            var cb = Math.Abs(Math.Round((Math.Sin((Math.PI / 180) * (t / 3)) / 3) + 0.6, 2));
            return (new(cr, cg, cb));
        }

        public void StaticParticleBuild(double t0, double t1, List<List<dynamic>> inputPoints, string particleName, double xDistance, double yDistance, double zDistance, string funcSpeed = "average", string colorMode = "normal", double cr = 1, double cg = 1, double cb = 1, double sx = 0, double sy = 0, double sz = 0, double count = 1, double age = 0, double staticColor = 0, double colorSpeed = 1, bool changeColorExpression = false, int changeColorExpressionSpeed = 1) {
            TempCommands.Clear();
            if (colorMode == "normal") {
                foreach (var c in inputPoints) {
                    staticColor += colorSpeed;
                    (int length1, int length2) = (c.Count, c.Count - 1);
                    if (c[0] is not List<dynamic>) {
                        if (length1 == 3 || length2 == 3) {
                            (double x, double y, double z) = (c[0], c[1], c[2]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else if (length1 == 6 || length2 == 6) {
                            (cr, cg, cb) = ColorCount(staticColor);
                            (double x, double y, double z, sx, sy, sz) = (c[0], c[1], c[2], c[3], c[4], c[5]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else if (length1 == 8 || length2 == 8) {
                            (cr, cg, cb) = ColorCount(staticColor);
                            (double x, double y, double z, sx, sy, sz, count, age) = (c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else if (length1 == 11 || length2 == 11) {
                            (double x, double y, double z, sx, sy, sz, cr, cg, cb, count, age) = (c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7], c[8], c[9], c[10]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else {
                            Console.WriteLine("unknown list");
                            continue;
                        }
                    } else {
                        var cLength = c.Count;
                        for (int k = 0; k < cLength; ++k) {
                            (length1, length2) = (c[0].Count, c[0].Count - 1);
                            (cr, cg, cb) = ColorCount(k);
                            if (length1 == 3 || length2 == 3) {
                                (double x, double y, double z) = (c[k][0], c[k][1], c[k][2]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            } else if (length1 == 6 || length2 == 6) {
                                (double x, double y, double z, sx, sy, sz) = (c[k][0], c[k][1], c[k][2], c[k][3], c[k][4], c[k][5]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            } else if (length1 == 8 || length2 == 8) {
                                (double x, double y, double z, sx, sy, sz, count, age) = (c[k][0], c[k][1], c[k][2], c[k][3], c[k][4], c[k][5], c[k][6], c[k][7]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            } else if (length1 == 11 || length2 == 11) {
                                (double x, double y, double z, cr, cg, cb, sx, sy, sz, count, age) = (c[k][0], c[k][1], c[k][2], c[k][3], c[k][4], c[k][5], c[k][6], c[k][7], c[k][8], c[k][9], c[k][10]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            }
                        }
                    }
                }
            } else if (colorMode == "custom") {
                foreach (var c in inputPoints) {
                    staticColor += colorSpeed;
                    (int length1, int length2) = (c.Count, c.Count - 1);
                    if (!c[0] is List<dynamic>) {
                        if (length1 == 3 || length2 == 3) {
                            (cr, cg, cb) = ColorCount(staticColor);
                            (double x, double y, double z) = (c[0], c[1], c[2]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else if (length1 == 6 || length2 == 6) {
                            (cr, cg, cb) = ColorCount(staticColor);
                            (double x, double y, double z, sx, sy, sz) = (c[0], c[1], c[2], c[3], c[4], c[5]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else if (length1 == 8 || length2 == 8) {
                            (cr, cg, cb) = ColorCount(staticColor);
                            (double x, double y, double z, sx, sy, sz, count, age) = (c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else if (length1 == 11 || length2 == 11) {
                            (double x, double y, double z, sx, sy, sz, cr, cg, cb, count, age) = (c[0], c[1], c[2], c[3], c[4], c[5], c[6], c[7], c[8], c[9], c[10]);
                            if (c[^1] is string) {
                                double speedExpression = c[^1];
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                TempCommands.Add(cmd);
                            } else if (changeColorExpression) {
                                var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                TempCommands.Add(cmd);
                            } else {
                                var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                TempCommands.Add(cmd);
                            }
                        } else {
                            Console.WriteLine("unknown list");
                            continue;
                        }
                    } else {
                        var cLength = c.Count;
                        for (int k = 0; k < cLength; ++k) {
                            (length1, length2) = (c[0].Count, c[0].Count - 1);
                            if (length1 == 3 || length2 == 3) {
                                (double x, double y, double z) = (c[k][0], c[k][1], c[k][2]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            } else if (length1 == 6 || length2 == 6) {
                                (double x, double y, double z, sx, sy, sz) = (c[k][0], c[k][1], c[k][2], c[k][3], c[k][4], c[k][5]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            } else if (length1 == 8 || length2 == 8) {
                                (double x, double y, double z, sx, sy, sz, count, age) = (c[k][0], c[k][1], c[k][2], c[k][3], c[k][4], c[k][5], c[k][6], c[k][7]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            } else if (length1 == 11 || length2 == 11) {
                                (double x, double y, double z, cr, cg, cb, sx, sy, sz, count, age) = (c[k][0], c[k][1], c[k][2], c[k][3], c[k][4], c[k][5], c[k][6], c[k][7], c[k][8], c[k][9], c[k][10]);
                                if (c[k][length1 - 1] is string) {
                                    var speedExpression = c[k][length1 - 1];
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, speedExpression);
                                    TempCommands.Add(cmd);
                                } else if (changeColorExpression) {
                                    var colorExpression = AddColorfulExpression(changeColorExpression, changeColorExpressionSpeed);
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, colorExpression);
                                    TempCommands.Add(cmd);
                                } else {
                                    var cmd = new ColorBlockSpeedExpression(t0, particleName, x, y, z, cr, cg, 1, cb, sx, sy, sz, xDistance, yDistance, zDistance, count, age, null);
                                    TempCommands.Add(cmd);
                                }
                            }
                        }
                    }
                }
            }

            CommandsToSequence(t0, t1, firstCommand: 0, changeCommand: 1, funcMode: funcSpeed);
        }

        public void BlockPlace(List<List<double>> blockPoints, List<List<double>> placePoints, double t0, double t1, double firstCommand = 0, double changeCommand = 1, string funcMode = "average") {
            int count = 0;
            foreach (var p in blockPoints) {
                (double dx, double dy, double dz) = (p[0], p[1], p[2]);
                (double x, double y, double z) = (placePoints[count][0], placePoints[count][1], placePoints[count][2]);
                var cmd = new BlockClone(x, y, z, dx, dy, dz);
                TempCommands.Add(cmd);
                count++;
            }
            CommandsToSequence(t0, t1, funcMode, firstCommand, changeCommand);
        }

        public void SummonFallingBlock(List<List<dynamic>> points, double t0, double t1, double firstCommand = 0, double changeCommand = 1, string funcMode = "average") {
            foreach (var p in points) {
                var cmd = new FallingBlock(p[0], p[1], p[2], p[3], p[4], p[5], p[6], p[7], p[8], p[9]);
                TempCommands.Add(cmd);
            }
            CommandsToSequence(t0, t1, funcMode, firstCommand, changeCommand);
        }

        public void SetBlocks(List<List<double>> points, string name, int t0, int t1, double firstCommand = 0, double changeCommand = 1, string funcMode = "average", string blockMode = "replace") {
            foreach (var p in points) {
                var cmd = new Command(t0, p[0], p[1], p[2], name, blockMode);
                TempCommands.Add(cmd);
            }
            CommandsToSequence(t0, t1, funcMode, firstCommand, changeCommand);
        }

        public void SetBlock(List<double> point, string name, int t0, int t1, double firstCommand = 0, double changeCommand = 1, string funcMode = "average", string blockMode = "replace") {
            var cmd = new Command(t0, point[0], point[1], point[2], name, blockMode);
            TempCommands.Add(cmd);
            CommandsToSequence(t0, t1, funcMode, firstCommand, changeCommand);
        }
    }
}
