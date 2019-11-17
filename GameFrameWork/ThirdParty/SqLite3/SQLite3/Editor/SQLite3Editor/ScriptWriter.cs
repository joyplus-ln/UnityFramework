using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework.Reflection.SQLite3Helper;

namespace Framework.Editor.SQLite3Creator
{
    public class ScriptWriter
    {
        public static void Writer(string InPath, ref TableData InTableData)
        {
            if (InPath == null) return;

            FileInfo info = new FileInfo(InPath);
            List<string> oldContent;

            if (info.Exists)
            {
                oldContent = new List<string>(100);
                using (FileStream stream = info.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        bool isNeedRead = false;
                        while (null != (line = reader.ReadLine()))
                        {
                            if (!isNeedRead && line.Contains("*Self Code Begin*")) isNeedRead = true;
                            if (isNeedRead) oldContent.Add(line);
                            if (isNeedRead && line.Contains("*Self Code End*")) isNeedRead = false;
                        }

                        reader.Close();
                    }
                    stream.Close();
                }
            }
            else
            {
                oldContent = new List<string>(4)
                {
                    "        //-------------------------------*Self Code Begin*-------------------------------",
                    "        //Custom code.",
                    "        //-------------------------------*Self Code End*   -------------------------------"
                };
            }

            string filename = InTableData.TableName;
            StringBuilder sb = new StringBuilder(1024);
            int length = InTableData.ColumnName.Length;
            sb.Append("/*\n")
                .Append(" * --->SQLite3 dataSyncBase table structure.<---\n")
                .Append(" * --->This class code is automatically generated。<---\n")
                .Append(" * --->If you need to modify, please place the custom code between <Self Code Begin> and <Self Code End>.\n")
                .Append(" *                                                                                    --szn\n")
                .Append(" */\n\n")
                .Append("using Framework.Reflection.SQLite3Helper;\n")
                .Append("using Framework.Reflection.Sync;\n")
                .Append("\n\n")
                .Append("namespace SQLite3TableDataTmp\n")
                .Append("{\n");

            sb.Append("    public enum ").Append(filename).Append("Enum\n")
                .Append("    {\n");
            for (int i = 0; i < length; i++)
            {
                if (InTableData.IsColumnEnables[i])
                    sb.Append("        ").Append(InTableData.ColumnName[i]).Append(",\n");
            }
            //sb.Append("        Max\n");
            sb.Append("    }\n\n");

            sb.Append("    public partial class ").Append(filename).Append(" : SyncBase").Append("\n")
                .Append("    {\n");
            
            for (int i = 0; i < length; i++)
            {
                if (!InTableData.IsColumnEnables[i]) continue;
                if (InTableData.SQLite3Constraints[i] != SQLite3Constraint.Default)
                    sb.Append("        [SQLite3Constraint(")
                        .Append(SQLite3ConstraintAttribute.ConvertToString(InTableData.SQLite3Constraints[i]))
                        .Append(")]\n");

                sb.Append("        [Sync((int)").Append(filename).Append("Enum.").Append(InTableData.ColumnName[i]).Append(")]\n")
                    .Append("        public ")
                    .Append(InTableData.CSharpTypes[i])
                    .Append(" ")
                    .Append(InTableData.ColumnName[i])
                    .Append(0 == i ? " { get; private set; }" : " { get; set; }");

                if (null == InTableData.ColumnDescribes || string.IsNullOrEmpty(InTableData.ColumnDescribes[i]))
                    sb.Append("\n\n");
                else
                    sb.Append("  //").Append(InTableData.ColumnDescribes[i]).Append("\n\n");
            }

            sb.Append("        public ").Append(filename).Append("()\n")
                .Append("        {\n")
                .Append("        }\n\n");



            sb.Append("        public ").Append(filename).Append("(");
            for (int i = 0; i < length; ++i)
            {
                if (InTableData.IsColumnEnables[i])
                    sb.Append(InTableData.CSharpTypes[i])
                        .Append(" In").Append(InTableData.ColumnName[i])
                        .Append(", ");
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(")\n");
            sb.Append("        {\n");

            for (int i = 0; i < length; ++i)
            {
                if (InTableData.IsColumnEnables[i])
                    sb.Append("            ").Append(InTableData.ColumnName[i])
                        .Append(" = In").Append(InTableData.ColumnName[i])
                        .Append(";\n");
            }
            sb.Append("        }\n\n");

            int count = oldContent.Count;
            for (int i = 0; i < count; ++i)
            {
                sb.Append(oldContent[i]).Append("\n");
            }
            sb.Append("        \n\n");

            sb.Append("        public   ").Append(filename).Append("  Clone(){");
            sb.Append("        \n\n");
            sb.Append("        ");
            sb.Append(filename).Append(" clone = new ").Append(filename).Append("();\n");
            for (int i = 0; i < length; ++i)
            {
                if (InTableData.IsColumnEnables[i])
                    sb.Append("            clone.").Append(InTableData.ColumnName[i])
                        .Append(" = ").Append(InTableData.ColumnName[i])
                        .Append(";\n");
            }

            sb.Append("        \n\n");
            sb.Append("        return clone;");
            sb.Append("        }\n\n");

            sb.Append("        public override string ToString()\n")
                .Append("        {\n")
                .Append("            return \"").Append(filename).Append(" : ")
                .Append(InTableData.ColumnName[0])
                .Append(" = \" + ")
                .Append(InTableData.ColumnName[0]);

            for (int i = 1; i < length; ++i)
            {
                if (InTableData.IsColumnEnables[i])
                    sb.Append(" + ").Append("\", ")
                        .Append(InTableData.ColumnName[i]).Append(" = \" + ").
                        Append(InTableData.ColumnName[i]);
            }
            sb.Append(";\n");
            sb.Append("        }\n\n");

            sb.Append("    }\n");
            sb.Append("}\n");

            if (info.Directory != null && !info.Directory.Exists) info.Directory.Create();
            if (info.Exists) info.Delete();

            File.WriteAllText(InPath, sb.ToString(), Encoding.UTF8);
        }
    }
}