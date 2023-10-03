using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMcAe.Class {
    public record Command {
        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public int Tick { get; set; }

        public string Mode { get; set; }

        public string Block { get; set; }

        public Command(double x, double y, double z) {
            X = x;
            Y = y;
            Z = z;
        }

        public Command(int tick, double x, double y, double z, string block, string mode = "replace") {
            Tick = tick;
            X = x;
            Y = y;
            Z = z;
            Block = block;
            Mode = mode;
        }

        public override string ToString() {
            return $"setblock {X} {Y} {Z} {Block} {Mode}";
        }
    }

    public record ParticleCommand : Command {
        public string Name { get; set; }
        public double Dx { get; set; }
        public double Dy { get; set; }
        public double Dz { get; set; }
        public double Speed { get; set; }
        public int Amount { get; set; }       

        public ParticleCommand(int tick, string name, double x, double y, double z, double dx, double dy, double dz, double speed, int amount, string mode = "force") : base(tick,x,y,z,"name",mode) {
            this.Tick = tick;
            this.Name = name;
            this.X = Math.Round(x, 2);
            this.Y = Math.Round(y, 2);
            this.Z = Math.Round(z, 2);
            this.Dx = Math.Round(dx, 2);
            this.Dy = Math.Round(dy, 2);
            this.Dz = Math.Round(dz, 2);
            this.Speed = speed;
            this.Amount = amount;
            this.Mode = mode;
        }

        public override string ToString() {
            return $"particle {this.Name} {this.X} {this.Y} {this.Z} {this.Dx} {this.Dy} {this.Dz} {this.Speed} {this.Amount} {this.Mode}";
        }
    }
}
