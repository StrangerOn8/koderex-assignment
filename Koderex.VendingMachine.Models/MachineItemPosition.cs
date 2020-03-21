using System;
using System.Collections.Generic;
using System.Text;

namespace Koderex.VendingMachine.Models {
    public class MachineItemPosition : IEquatable<MachineItemPosition> {
        public string Row { get; set; }
        public int Column { get; set; }
        public bool Equals(MachineItemPosition other) {
            return other != null &&
                   Row.ToLower() == other.Row.ToLower() &&
                   Column == other.Column;
        }
        public override int GetHashCode() {
            return HashCode.Combine(Row.ToLower(), Column);
        }
        public override string ToString() {
            return $"[{Row.ToUpper()}{Column}]";
        }
    }
}
