using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace DigiAeon.Core.Services
{
    public class CsvService
    {
        public IEnumerable<T> ParseTo<T>(string filePath)
        {
            List<T> records;

            using (var reader = File.OpenText(filePath))
            {
                using (var csvReader = new CsvReader(reader))
                {
                    records = csvReader.GetRecords<T>().ToList();
                }
            }
            // http://www.tugberkugurlu.com/archive/how-and-where-concurrent-asynchronous-io-with-asp-net-web-api

            //var result = Task.WhenAll(records.Select(x => Test()).ToList());

            return records;
        }
    }
}
