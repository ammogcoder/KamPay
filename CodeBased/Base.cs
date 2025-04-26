using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack;
using System.ComponentModel;
using System.Data;
using System.Web;

namespace Kpakam
{
    public static class Constant
    {
        public static string appSettings = "appsettings.json";
        public static string appKeys = "AppKeys";
        public static string OrgID = "AppKeys:OrgID";
        public static string SecToken = "AppKeys:SecToken";
        public static string NamesCompareRatio = "AppKeys:NamesCompareRatio";
        public static string ConnectionStringKey = "AppKeys:ConnectionString";
        public static string myPassKey = "AppKeys:myPass";
        public static string writepath = "Download";
    }
    public static class Base
    { 
        public static string LogPath()
        {
           var PathRes = Path.Combine(AppContext.BaseDirectory, Constant. writepath);
            if (!Directory.Exists(PathRes))
                Directory.CreateDirectory(PathRes);
            return PathRes;
        }
        public static ErrorLogger loggerx = new ErrorLogger(LogPath()); 
        public static string ToJsonRaw(this DataSet dataDs)
        {

            int rowsCountIn = dataDs.Tables[0].Rows.Count;
            int colsCountIn = dataDs.Tables[0].Columns.Count;
            string[] columnNamesAr = new string[dataDs.Tables[0].Columns.Count];

            for (int i = 0; i < colsCountIn; i++)
            {

                columnNamesAr[i] = dataDs.Tables[0].Columns[i].ColumnName;
            }

            string jsonSt = "[";

            for (int i = 0; i < rowsCountIn; i++)
            {
                jsonSt += "{";

                for (int j = 0; j < colsCountIn; j++)
                {
                    jsonSt += ((jsonSt.Substring(jsonSt.Length - 1) == "{" ? "" : ",") + "\"" + columnNamesAr[j] + "\":" + (dataDs.Tables[0].Rows[i][j] == DBNull.Value ? "null" : ("\"" + dataDs.Tables[0].Rows[i][j] + "\"")));
                }

                jsonSt += ("}" + (i < (rowsCountIn - 1) ? "," : ""));
            }

            jsonSt += "]";

            return jsonSt;
        }

        public static string ToJson(this DataSet dataDs)
        {
            var json = JsonConvert.SerializeObject(dataDs, Formatting.Indented);
            var token = JToken.Parse(json).ToString( Formatting.Indented);
            return JSONPrettify(token);
            //return token.ToString().Replace("\r\n", "").Replace(@"\","");
           // return JsonConvert.SerializeObject(dataDs, new JsonConverterObjectToString());
        }

        public static string JSONPrettify(string json)
        {
            using (var stringReader= new StringReader(json))
             using (var stringWriter= new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                return stringWriter.ToString();
            }
        }

        public static DataTable? ToDataTable(this string json)
        {
            var jsonLinq = JObject.Parse(json);

            // Find the first array using Linq
            var srcArray = jsonLinq.Descendants().Where(d => d is JArray).First();
            var trgArray = new JArray();
            foreach (JObject row in srcArray.Children<JObject>())
            {
                var cleanRow = new JObject();
                foreach (JProperty column in row.Properties())
                {
                    // Only include JValue types
                    if (column.Value is JValue)
                    {
                        cleanRow.Add(column.Name, column.Value);
                    }
                }

                trgArray.Add(cleanRow);
            }

            return JsonConvert.DeserializeObject<DataTable>(trgArray.ToString());
        }

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }

    class JsonConverterObjectToString : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(JTokenType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                return token.ToString();
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //serializer.Serialize(writer, value);

            //serialize as actual JSON and not string data
            var token = JToken.Parse(value.ToString());
            writer.WriteToken(token.CreateReader());

        }
    }
}