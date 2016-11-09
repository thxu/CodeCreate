using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Model生成器
{
    public partial class CreateModelForm : Form
    {
        private static string StrXlsPath { get; set; }

        public CreateModelForm()
        {
            InitializeComponent();
        }

        private void btnOpenXls_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "表格文件(*.xls)|*.xls";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StrXlsPath = ofd.FileName;
            }
            labXlsPath.Text = StrXlsPath;
        }

        private void CreateModel_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StrXlsPath))
            {
                MessageBox.Show("请先选择xls文件");
                return;
            }
            try
            {
                DataTable dt = new ExcelHelper(StrXlsPath).ExcelToDataTable("Sheet1", true);
                //strModel = strModel + "\r\n" + GetAddCode(dt) + "\r\n" + GetBatchAddCode(dt) + "\r\n" + GetUpdateCode(dt);
                string ModelFileName = Path.GetFileNameWithoutExtension(StrXlsPath);

                // IRepository
                if (this.checkModel.Checked)
                {
                    // Model
                    string model = GetModel(dt, ModelFileName);
                    Write(ModelFileName, model);
                }

                // IRepository
                if (this.checkIRepository.Checked)
                {
                    string iRepositoryFileName = string.Format("I{0}Repository", ModelFileName);
                    string iRepository = GetIRepository(iRepositoryFileName);
                    Write(iRepositoryFileName, iRepository);
                }

                // Repository
                if (this.checkRepository.Checked)
                {
                    string repositoryFileName = string.Format("{0}Repository", ModelFileName);
                    string repository = GetRepository(repositoryFileName, dt);
                    Write(repositoryFileName, repository);
                }

                // Factory
                if (this.checkFactory.Checked)
                {
                    string factoryFileName = string.Format("{0}Factory", ModelFileName);
                    string factory = GetFactory();
                    Write(factoryFileName, factory);
                }

                // Event
                if (this.checkEvent.Checked)
                {
                    string eventFileName = string.Format("{0}Event", ModelFileName);
                    string @event = GetTableEvent(eventFileName, dt);
                    WriteSql(eventFileName, @event);
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("生成成功");
        }

        private static void Write(string name, string strModel)
        {
            string path = StrXlsPath.Substring(0, StrXlsPath.LastIndexOf(@"\")) + @"\" + name + ".cs";
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(strModel);
            }
        }

        private static void WriteSql(string name, string strModel)
        {
            string path = StrXlsPath.Substring(0, StrXlsPath.LastIndexOf(@"\")) + @"\" + name + ".sql";
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(strModel);
            }
        }

        private string GetModel(DataTable dtData, string strName)
        {
            if (dtData == null)
            {
                return null;
            }
            StringBuilder strRes = new StringBuilder();
            strRes.Append("using System;\r\n");
            strRes.Append("using System.Runtime.Serialization;");
            strRes.AppendLine(Environment.NewLine);
            strRes.Append("namespace T");
            strRes.Append("\r\n");
            strRes.Append("{");
            strRes.Append("\r\n");
            strRes.Append("\t/// <summary>\r\n");
            strRes.AppendFormat("\t/// {0}\r\n", strName);
            strRes.Append("\t/// </summary>\r\n");
            strRes.Append("\t[DataContract]\r\n");
            strRes.AppendFormat("\tpublic class {0}", strName);
            strRes.Append("\r\n\t{");
            foreach (DataRow row in dtData.Rows)
            {
                strRes.Append(GetMemberData(row));
            }
            strRes.Append("\t}\r\n");
            strRes.Append("}");
            return strRes.ToString();
        }

        private string GetMemberData(DataRow drData)
        {
            StringBuilder strData = new StringBuilder();
            strData.Append("\r\n\t\t/// <summary>\r\n");
            strData.AppendFormat("\t\t/// {0}\r\n", drData["Name"]);
            strData.Append("\t\t/// </summary>\r\n");
            strData.Append("\t\t[DataMember]\r\n");
            string strType = drData["Data Type"].ToString();
            strData.AppendFormat("\t\tpublic {0} {1} ", GetMemberType(strType), drData["Code"]);
            strData.Append("{ get; set; }\r\n");
            return strData.ToString();
        }

        private string GetMemberType(string strType)
        {
            if (string.IsNullOrWhiteSpace(strType))
            {
                return null;
            }
            if (strType.Contains("bigint"))
            {
                return "long";
            }
            if (strType.Contains("binary") || strType.Contains("blob"))
            {
                return "byte[]";
            }
            if (strType.Contains("bit") || strType.Contains("boolean"))
            {
                return "bool";
            }
            if (strType.Contains("varchar") || strType.Contains("text"))
            {
                return "string";
            }
            if (strType.Contains("bool"))
            {
                return "bool";
            }
            if (strType.Contains("char"))
            {
                return "char";
            }
            if (strType.Contains("date") || strType.Contains("timestamp") || strType.Contains("datetime"))
            {
                return "DateTime";
            }
            if (strType.Contains("dec"))
            {
                return "decimal";
            }
            if (strType.Contains("double"))
            {
                return "double";
            }
            if (strType.Contains("float"))
            {
                return "float";
            }
            if (strType.Contains("int"))
            {
                return "int";
            }
            if (strType.Contains("long"))
            {
                return "long";
            }
            if (strType.Contains("tinyint"))
            {
                return "int";
            }
            return "object";
        }

        private List<string> GetProp(DataTable dtData) => (from DataRow dr in dtData.Rows select dr["Code"].ToString()).ToList();

        private List<string> GetType(DataTable dtData)
        {
            List<string> res = new List<string>();
            foreach (DataRow dr in dtData.Rows)
            {
                res.Add(dr["Data Type"].ToString());
            }
            return res;
        }

        private string GetAddCode(DataTable dtData)
        {
            List<string> propList = GetProp(dtData);
            StringBuilder addCode = new StringBuilder();
            string tbName = Path.GetFileNameWithoutExtension(StrXlsPath);
            addCode.Append("\t\t/// <summary>\r\n");
            addCode.Append("\t\t/// 添加\r\n");
            addCode.Append("\t\t/// <param name=\"entity\">实体</param>\r\n");
            addCode.Append("\t\t/// </summary>\r\n");
            addCode.AppendFormat("\t\tpublic long Add({0} entity)\r\n", tbName);
            addCode.Append("\t\t{\r\n");
            addCode.Append("\t\t\tthis.ClearParameters();\r\n");
            addCode.Append("\t\t\tStringBuilder sql = new StringBuilder();\r\n");
            addCode.AppendFormat("\t\t\tsql.Append(\"INSERT INTO {0} (\");\r\n", tbName);
            int i = 1;
            foreach (string prop in propList)
            {
                if (i == propList.Count)
                {
                    addCode.AppendFormat("\t\t\tsql.Append(\" {0} \");\r\n", prop);
                }
                else
                {
                    addCode.AppendFormat("\t\t\tsql.Append(\" {0}, \");\r\n", prop);
                }
                i++;
            }
            addCode.Append("\t\t\tsql.Append(\") VALUES(\");\r\n");
            i = 1;
            foreach (string prop in propList)
            {
                if (i == propList.Count)
                {
                    addCode.AppendFormat("\t\t\tsql.Append(\" @{0} \");\r\n", prop);
                }
                else
                {
                    addCode.AppendFormat("\t\t\tsql.Append(\" @{0}, \");\r\n", prop);
                }
                i++;
            }
            addCode.Append("\t\t\tsql.Append(\"); \");\r\n");
            addCode.Append("\r\n");
            i = 1;
            foreach (string prop in propList)
            {
                addCode.AppendFormat("\t\t\tthis.AddParameter(\"@{0}\", entity.{0});\r\n", prop);
                i++;
            }
            addCode.Append("\t\r\n");
            addCode.Append("\t\t\tvar result = this.ExecuteNonQuery(sql.ToString());\r\n");
            addCode.Append("\t\t\tif (result != 1)\r\n");
            addCode.Append("\t\t\t{\r\n");
            addCode.Append("\t\t\t\tthrow new CustomException(\"添加失败\");\r\n");
            addCode.Append("\t\t\t}\r\n");
            addCode.Append("\t\t\treturn result;\r\n");
            addCode.Append("\t\t}\r\n");

            return addCode.ToString();
        }

        private string GetAddsCode(DataTable dtData)
        {
            List<string> propList = GetProp(dtData);
            StringBuilder addCode = new StringBuilder();
            string tbName = Path.GetFileNameWithoutExtension(StrXlsPath);
            addCode.Append("\t\t/// <summary>\r\n");
            addCode.Append("\t\t/// 批量添加\r\n");
            addCode.Append("\t\t/// <param name=\"entitys\">实体</param>\r\n");
            addCode.Append("\t\t/// </summary>\r\n");
            addCode.AppendFormat("\t\tpublic int Add(IList<{0}> entitys)\r\n", tbName);
            addCode.Append("\t\t{\r\n");
            addCode.Append("\t\t\tthis.ClearParameters();\r\n");
            addCode.Append("\t\t\tStringBuilder sql = new StringBuilder();\r\n");
            addCode.AppendFormat("\t\t\tsql.Append(\"INSERT INTO {0} (\");\r\n", tbName);
            int i = 1;
            foreach (string prop in propList)
            {
                if (i == propList.Count)
                {
                    addCode.AppendFormat("\t\t\tsql.Append(\" {0} \");\r\n", prop);
                }
                else
                {
                    addCode.AppendFormat("\t\t\tsql.Append(\" {0}, \");\r\n", prop);
                }
                i++;
            }
            addCode.Append("\t\t\tsql.Append(\") VALUES \");\r\n");
            addCode.Append("\t\t\tfor (int i = 0; i < entitys.Count; i++)\r\n");
            addCode.Append("\t\t\t{\r\n");
            addCode.Append("\t\t\t\tsql.Append(\"(\");\r\n");
            i = 1;
            foreach (string prop in propList)
            {
                if (i == propList.Count)
                {
                    addCode.AppendFormat("\t\t\t\tsql.AppendFormat(\" @{0}{{0}}\", i);\r\n", prop);
                }
                else
                {
                    addCode.AppendFormat("\t\t\t\tsql.AppendFormat(\" @{0}{{0}},\", i);\r\n", prop);
                }
                i++;
            }
            addCode.Append("\t\t\t\tsql.Append(\"),\");\r\n");
            i = 1;
            foreach (string prop in propList)
            {
                addCode.AppendFormat("\t\t\t\tthis.AddParameter(string.Format(\"@{0}{{0}}\", i), entitys[i].{0});\r\n", prop);
                i++;
            }
            addCode.Append("\t\t\t}\r\n");
            addCode.Append("\t\t\tint result = ExecuteNonQuery(sql.ToString().TrimEnd(','));\r\n");
            addCode.Append("\t\t\tif (result != entitys.Count)\r\n");
            addCode.Append("\t\t\t{\r\n");
            addCode.Append("\t\t\t\tthrow new CustomException(\"批量插入数据失败\");\r\n");
            addCode.Append("\t\t\t}\r\n");
            addCode.Append("\t\t\treturn result;\r\n");
            addCode.Append("\t\t}\r\n");

            return addCode.ToString();
        }

        private string GetUpdateCode(DataTable dtData)
        {
            List<string> propList = GetProp(dtData);
            List<string> typeList = GetType(dtData);
            StringBuilder updateCode = new StringBuilder();
            string tbName = Path.GetFileNameWithoutExtension(StrXlsPath);
            updateCode.Append("\t\t/// <summary>\r\n");
            updateCode.Append("\t\t/// 批量添加\r\n");
            updateCode.Append("\t\t/// <param name=\"entity\">实体</param>\r\n");
            updateCode.Append("\t\t/// </summary>\r\n");
            updateCode.AppendFormat("\t\tpublic int Update({0} entity)\r\n", tbName);
            updateCode.Append("\t\t{\r\n");
            updateCode.Append("\t\t\tthis.ClearParameters();\r\n");
            updateCode.Append("\t\t\tStringBuilder sql = new StringBuilder();\r\n");
            updateCode.AppendFormat("\t\t\tsql.Append(\"UPDATE {0} SET \");\r\n", tbName);
            int i = 1;
            foreach (string prop in propList)
            {
                if (i == 1)
                {
                    i++;
                    continue;
                }
                if (i == propList.Count)
                {
                    updateCode.AppendFormat("\t\t\tsql.Append(\" {0} = @{0} \");\r\n", prop);
                }
                else
                {
                    updateCode.AppendFormat("\t\t\tsql.Append(\" {0} = @{0}, \");\r\n", prop);
                }
                i++;
            }
            updateCode.AppendFormat("\t\t\tsql.Append(\" WHERE {0} = @{0}; \");\r\n", propList[0]);
            updateCode.Append("\r\n");
            i = 0;
            foreach (string prop in propList)
            {
                if (typeList[i].Contains("varchar"))
                {
                    updateCode.AppendFormat("\t\t\tthis.AddParameter(\"@{0}\", entity.{0} ?? string.Empty);\r\n", prop);
                }
                else
                {
                    updateCode.AppendFormat("\t\t\tthis.AddParameter(\"@{0}\", entity.{0});\r\n", prop);
                }
                i++;
            }
            updateCode.Append("\r\n");
            updateCode.Append("\t\t\tvar result = this.ExecuteNonQuery(sql.ToString());\r\n");
            updateCode.Append("\t\t\tif (result != 1)\r\n");
            updateCode.Append("\t\t\t{\r\n");
            updateCode.Append("\t\t\t\tthrow new CustomException(\"更新失败\");\r\n");
            updateCode.Append("\t\t\t}\r\n");
            updateCode.Append("\t\t\treturn result;\r\n");
            updateCode.Append("\t\t}\r\n");
            return updateCode.ToString();
        }

        private string GetIRepository(string strName)
        {
            string tbName = Path.GetFileNameWithoutExtension(StrXlsPath);
            StringBuilder strRes = new StringBuilder();
            strRes.Append("using Qunau.NetFrameWork.Infrastructure;");
            strRes.AppendLine(Environment.NewLine);
            strRes.Append("namespace T");
            strRes.Append("\r\n");
            strRes.Append("{");
            strRes.Append("\r\n");
            strRes.Append("\t/// <summary>\r\n");
            strRes.AppendFormat("\t/// {0}仓储\r\n", strName);
            strRes.Append("\t/// </summary>\r\n");
            strRes.AppendFormat("\tpublic interface {0} : IAddRepository<{1}>, IAddsRepository<{1}>, IUpdateRepository<{1}>", strName, tbName);
            strRes.Append("\r\n");
            strRes.Append("\t{");
            strRes.AppendLine("\r\n");
            strRes.Append("\t}");
            strRes.Append("\r\n");
            strRes.Append("}");
            return strRes.ToString();
        }

        private string GetRepository(string strName, DataTable dtData)
        {
            StringBuilder strRes = new StringBuilder();
            //strRes.Append("using System;\r\n");
            strRes.Append("using System.Collections.Generic;\r\n");
            strRes.Append("using System.Text;\r\n");
            strRes.Append("using Qunau.NetFrameWork.Common.Exception;\r\n");
            strRes.Append("using Qunau.NetFrameWork.DbCommon;\r\n");
            strRes.Append("using Qunau.NetFrameWork.Infrastructure;");
            strRes.AppendLine(Environment.NewLine);

            strRes.Append("namespace T");
            strRes.Append("\r\n");
            strRes.Append("{");
            strRes.Append("\r\n");
            strRes.Append("\t/// <summary>\r\n");
            strRes.AppendFormat("\t/// {0}仓储接口\r\n", strName);
            strRes.Append("\t/// </summary>\r\n");
            strRes.AppendFormat("\tpublic class {0} : BaseRepository, I{0}", strName);
            strRes.Append("\r\n");
            strRes.Append("\t{\r\n");

            strRes.Append("\t\t/// <summary>\r\n");
            strRes.Append("\t\t/// Initializes a new instance of the <see cref=\"T: Qunau.NetFrameWork.DbCommon.BaseRepository\"/> class.\r\n");
            strRes.Append("\t\t/// 构造函数\r\n");
            strRes.Append("\t\t/// </summary>\r\n");
            strRes.Append("\t\t/// <param name=\"unit\">工作单元</param><param name=\"name\">链接名称</param>\r\n");
            strRes.AppendFormat("\t\tpublic {0}(IUnitOfWork unit, string name) : base(unit, name)\r\n", strName);
            strRes.Append("\t\t{\r\n");
            strRes.Append("\t\t}");
            strRes.Append(Environment.NewLine);
            strRes.Append(Environment.NewLine);
            strRes.Append(GetAddCode(dtData));
            strRes.Append(Environment.NewLine);
            strRes.Append(GetAddsCode(dtData));
            strRes.Append(Environment.NewLine);
            strRes.Append(GetUpdateCode(dtData));

            strRes.Append("\t}");
            strRes.Append("\r\n");
            strRes.Append("}");
            return strRes.ToString();
        }

        private string GetFactory()
        {
            string tbName = Path.GetFileNameWithoutExtension(StrXlsPath);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("\tpublic class {0}Factorry\r\n", tbName);
            sb.Append("\t{\r\n");
            sb.AppendFormat("\t\t#region {0}\r\n", tbName);
            sb.Append("\t\t/// <summary>\r\n");
            sb.AppendFormat("\t\t/// 创建{0}仓储接口\r\n", tbName);
            sb.Append("\t\t/// </summary>\r\n");
            sb.AppendFormat("\t\t/// {0}仓储接口\r\n", tbName);
            sb.AppendFormat("\t\tpublic static I{0}Repository CreateI{0}RepositoryRead()\r\n", tbName);
            sb.Append("\t\t{\r\n");
            sb.AppendFormat("\t\t\t return new {0}Repository(null, null);\r\n", tbName);
            sb.Append("\t\t}\r\n");
            sb.AppendFormat(Environment.NewLine);
            sb.Append("\t\t/// <summary>\r\n");
            sb.AppendFormat("\t\t/// 创建{0}仓储接口\r\n", tbName);
            sb.Append("\t\t/// </summary>\r\n");
            sb.AppendFormat("\t\t/// {0}仓储接口\r\n", tbName);
            sb.AppendFormat("\t\tpublic static I{0}Repository CreateI{0}RepositoryWrite()\r\n", tbName);
            sb.Append("\t\t{\r\n");
            sb.AppendFormat("\t\t\t return new {0}Repository(null, null);\r\n", tbName);
            sb.Append("\t\t}\r\n");
            sb.AppendFormat(Environment.NewLine);
            sb.Append("\t\t/// <summary>\r\n");
            sb.AppendFormat("\t\t/// 创建{0}仓储接口\r\n", tbName);
            sb.Append("\t\t/// </summary>\r\n");
            sb.Append("\t\t/// <param name=\"unitOfWork\">工作单元</param>\r\n");
            sb.AppendFormat("\t\t/// {0}仓储接口\r\n", tbName);
            sb.AppendFormat("\t\tpublic static I{0}Repository CreateI{0}RepositoryWrite(IUnitOfWork unitOfWork)\r\n", tbName);
            sb.Append("\t\t{\r\n");
            sb.AppendFormat("\t\t\t return new {0}Repository(unitOfWork, null);\r\n", tbName);
            sb.Append("\t\t}\r\n");
            sb.Append("\t\t#endregion\r\n");
            sb.Append("\t}");
            return sb.ToString();
        }

        private string GetTableEvent(string strName, DataTable dtData)
        {
            string tbName = Path.GetFileNameWithoutExtension(StrXlsPath);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("/****** 对象:  Event Event_C_{0}YYYYMM    脚本日期: 2016-06-02 11:32:30 ******/", tbName);
            sb.Append(Environment.NewLine);
            sb.Append("DELIMITER |");
            sb.Append(Environment.NewLine);
            sb.AppendFormat("CREATE EVENT `Event_C_{0}_YYYYMM`", tbName);
            sb.Append(Environment.NewLine);
            sb.Append("ON SCHEDULE EVERY 1 MONTH STARTS '2016-06-02 11:32:30'");
            sb.Append(Environment.NewLine);
            sb.AppendFormat("ON COMPLETION PRESERVE ENABLE COMMENT '自动创建下个{0}_YYYYMM表'", tbName);
            sb.Append(Environment.NewLine);
            sb.Append("DO BEGIN");
            sb.Append(Environment.NewLine);
            sb.Append("SET @NextTailStr = DATE_FORMAT(DATE_ADD(NOW(),INTERVAL 1 MONTH), '%Y%m');");
            sb.Append(Environment.NewLine);
            sb.AppendFormat("SET @createEventSQL = CONCAT('CREATE TABLE IF NOT EXISTS `{0}_',", tbName);
            sb.Append(Environment.NewLine);
            sb.Append("CAST(@NextTailStr AS UNSIGNED),'`(");
            //sb.Append(Environment.NewLine);

            StringBuilder content = new StringBuilder();
            int rows = dtData.Rows.Count;
            int index = 0;
            string key = string.Empty;
            foreach (DataRow dr in dtData.Rows)
            {
                content.Append(Environment.NewLine);
                string code = dr["Code"].ToString();
                string dataType = dr["Data Type"].ToString();
                string comment = dr["Comment"].ToString();
                string name = dr["Name"].ToString();
                string desc = string.IsNullOrWhiteSpace(comment) ? name : comment;
                //string p = dr["P"].ToString();
                if (index == 0)
                {
                    if (dataType == "int" || dataType == "bigint")
                    {
                        content.AppendFormat("\t{0} {1} NOT NULL AUTO_INCREMENT COMMENT ''{2}'',", code, dataType, desc);
                    }
                    else
                    {
                        content.AppendFormat("\t{0} {1} NOT NULL COMMENT ''{2}'',", code, dataType, desc);
                    }
                    key = code;
                }
                else
                {
                    content.AppendFormat("\t{0} {1} NOT NULL COMMENT ''{2}'',", code, dataType, desc);
                }
                if (index == rows - 1)
                {
                    content.Append(Environment.NewLine);
                    content.AppendFormat("\tPRIMARY KEY ({0}),", key);
                }
                index++;
            }

            sb.Append(content.ToString().TrimEnd(','));
            sb.Append(Environment.NewLine);
            sb.AppendFormat("\t) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT=''{0}表'';');", tbName);
            sb.Append(Environment.NewLine);
            sb.Append("SELECT @createEventSQL;");
            sb.Append(Environment.NewLine);
            sb.Append("PREPARE CreateEventStatement FROM @createEventSQL;");
            sb.Append(Environment.NewLine);
            sb.Append("EXECUTE CreateEventStatement;");
            sb.Append(Environment.NewLine);
            sb.Append("END");
            sb.Append(Environment.NewLine);
            sb.Append("| DELIMITER ;");
            return sb.ToString();
        }
    }
}
