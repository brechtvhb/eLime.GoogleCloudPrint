using System;

namespace eLime.GoogleCloudPrint.Models
{
    public class PrinterAccess
    {
        private Role Role { get; set; }

        public String Scope { get; set; }

        public String Name { get; set; }

        public Membership Membership { get; set; }

        public Type Type { get; set; }

        public String Email { get; set; }
    }

    public enum Role
    {
        Device,
        User,
        Manager,
        Owner
    }

    public enum Membership
    {
        None,

        Member,

        Manager
    }

    public enum Type
    {
        User,

        Group,
        Domain
    }
}