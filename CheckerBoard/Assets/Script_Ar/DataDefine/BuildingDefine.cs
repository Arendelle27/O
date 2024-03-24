
    public class BuildingDefine
    {
        public int TID { get; set; }
        public string Name { get; set; }
        public Building_Type Class { get; set;}
        public Building_Type Type { get; set; }
        public string Resource { get; set; }
        public string Description { get; set; }
        public Building_Condition_Type Condition { get; set; }
        public int NumericalValue { get; set; }
        public int Resource1Cost { get; set; }
        public int Resource2Cost { get; set; }
        public int Resource3Cost { get; set; }
        public int Production { get; set; }
        public int GatherResourceType { get; set; }
        public int GatherResourceAmount { get; set; }
        public int GatherResourceRounds { get; set; }
        public int Attack { get; set; }
        public int HostilityToRobot { get; set; }
        public int HostilityToHuman { get; set; }
}
