using System;

namespace Framework.Reflection.SQLite3Helper
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SQLite3ConstraintAttribute : Attribute
    {
        private string constraint;
        public string Constraint { get { return constraint; } }

        public SQLite3ConstraintAttribute(SQLite3Constraint InConstraint)
        {
            constraint = string.Empty;
            if ((InConstraint & SQLite3Constraint.PrimaryKey) == SQLite3Constraint.PrimaryKey)
                constraint += "PRIMARY KEY ";
            if ((InConstraint & SQLite3Constraint.AutoIncrement) == SQLite3Constraint.AutoIncrement)
                constraint += "AUTOINCREMENT ";
            if ((InConstraint & SQLite3Constraint.Unique) == SQLite3Constraint.Unique)
                constraint += "UNIQUE ";
            if ((InConstraint & SQLite3Constraint.NotNull) == SQLite3Constraint.NotNull)
                constraint += "NOT NULL ";
        }

        public static string ConvertToString(SQLite3Constraint InConstraint)
        {
            string result = string.Empty;
            if ((InConstraint & SQLite3Constraint.PrimaryKey) != 0)
                result += "SQLite3Constraint.PrimaryKey | ";
            if ((InConstraint & SQLite3Constraint.Unique) != 0)
                result += "SQLite3Constraint.Unique | ";
            if ((InConstraint & SQLite3Constraint.AutoIncrement) != 0)
                result += "SQLite3Constraint.AutoIncrement | ";
            if ((InConstraint & SQLite3Constraint.NotNull) != 0)
                result += "SQLite3Constraint.NotNull | ";

            return result == string.Empty ? string.Empty : result.Remove(result.Length - 2, 2);
        }
    }
}