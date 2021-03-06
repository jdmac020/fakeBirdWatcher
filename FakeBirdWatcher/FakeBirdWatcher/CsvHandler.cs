﻿using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirdWatcher
{
    public class CsvHandler
    {
        public bool WriteListToFile<T>(List<T> recordsToWrite)
        {
            var directory = AppDomain.CurrentDomain.BaseDirectory;
            var fileName = $"ScannedAccounts_{DateTime.Today.ToString("yyyyMMdd")}.csv";
            var filePath = Path.Combine(directory, fileName);

            var fileExists = File.Exists(filePath);

            using (StreamWriter streamWriter = File.AppendText(filePath))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                if (fileExists)
                    csvWriter.Context.HasHeaderBeenWritten = true;

                csvWriter.WriteRecords(recordsToWrite);
            }

            return true;
        }
    }
}
