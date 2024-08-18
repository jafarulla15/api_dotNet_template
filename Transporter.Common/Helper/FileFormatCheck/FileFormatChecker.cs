using ClosedXML.Excel;
using Transporter.Common.DTO;
using Transporter.Common.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transporter.Common.Helper.FileFormatCheck
{
    public class FileFormatChecker : IFileFormatChecker
    {
        //FilePathRead objFilePath = _configuration.GetSection("Attachments").Get<FilePathRead>();
        /// <summary>
        /// Check 
        /// </summary>
        /// <param name="formaterFilePath"></param>
        /// <param name="toBeCheckedFilePath"></param>
        /// <returns></returns>
        public async Task<string> ValidateFileCellHeaderAndCellContentTypeWithStandardFile(string formaterFilePath, string toBeCheckedFilePath)
        {
            FileFormatCheckOutPut fileFormatCheckOutPut = await GetCellHeaderKeyAndCellContentTypeFromFile(formaterFilePath);
            string errorMessage = await ValidateFileCellHeaderAndCellContentType(toBeCheckedFilePath, fileFormatCheckOutPut.lstFileFormatChecker);

            return errorMessage;
        }

        public async Task<string> ValidateFileCellHeaderAndCellContentType(string filePath, List<FileFormatInfo> lstHeaderKeyAndType)
        {
            string validationError = string.Empty;
            List<FileFormatInfo> lstHeaderKeyAndTypeChecked = new List<FileFormatInfo>();
            List<FileFormatInfo> lstHeaderKeyAndTypeFiltered = new List<FileFormatInfo>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var workbook = new XLWorkbook(fileStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    int rowIndex = 0;

                    //** (1) Check Header Key
                    foreach (var row in worksheet.Rows())
                    {
                        if (rowIndex == 0)
                        {
                            // Only Header Cell
                            int cellIndex = 0;
                            foreach (var cell in row.Cells())
                            {
                                try
                                {
                                    //(a) Is this cell give in check list
                                    lstHeaderKeyAndTypeFiltered = lstHeaderKeyAndType.FindAll(o => o.cellHeaderKeyName == cell.Value.ToString());
                                    if (lstHeaderKeyAndTypeFiltered.Count > 0)
                                    {
                                        //(b) check duplicate or not
                                        lstHeaderKeyAndTypeFiltered = lstHeaderKeyAndTypeChecked.FindAll(o => o.cellHeaderKeyName == cell.Value.ToString());
                                        if (lstHeaderKeyAndTypeFiltered.Count > 0)
                                        {
                                            // already exist.
                                            validationError += Environment.NewLine + "Duplicate Cell at Cell Index (" + cellIndex + ") ";
                                        }
                                        else
                                        {
                                            lstHeaderKeyAndTypeChecked.Add(lstHeaderKeyAndTypeFiltered[0]);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    validationError += Environment.NewLine + "Invalid File Header at Cell(" + cellIndex + ") ";
                                }

                                cellIndex++;
                            }

                            //** check is there any missing required cell
                            if (lstHeaderKeyAndType.Count != lstHeaderKeyAndTypeChecked.Count)
                            {
                                // some cell are missing
                                foreach (FileFormatInfo fileFormatCheckInfo in lstHeaderKeyAndType)
                                {
                                    lstHeaderKeyAndTypeFiltered = lstHeaderKeyAndType.FindAll(o => o.cellHeaderKeyName.ToString() == fileFormatCheckInfo.cellHeaderKeyName.ToString());
                                    if (lstHeaderKeyAndTypeFiltered.Count <= 0)
                                    {
                                        validationError += Environment.NewLine + "Missing Cell name (" + lstHeaderKeyAndTypeFiltered[0].cellHeaderKeyName + ") ";
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Only Header check
                            break;
                        }

                        rowIndex++;
                    }

                    //** (2) Check Cell Content Type
                    rowIndex = 0;
                    foreach (var row in worksheet.Rows())
                    {
                        if (rowIndex == 0)
                        {
                            //No need of any thing with header
                        }
                        else
                        {
                            int cellIndex = 0;
                            foreach (var cell in row.Cells())
                            {
                                try
                                {
                                    lstHeaderKeyAndTypeFiltered = lstHeaderKeyAndType.FindAll(o => o.cellHeaderKeyName == cell.Value.ToString());
                                    if (lstHeaderKeyAndTypeFiltered.Count > 0)
                                    {
                                        //** Check cell content type
                                        Convert.ChangeType(cell.Value.ToString(), lstHeaderKeyAndTypeFiltered[0].cellContentType);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    validationError += Environment.NewLine + "Invalid data type at Row(" + rowIndex + "), Cell(" + cellIndex + ") ";
                                }

                                cellIndex++;
                            }
                        }

                        rowIndex++;
                    }
                }
            }

            return validationError;
        }

        public async Task<string> ValidateFileCellHeader(string filePath, Dictionary<int, string> headerKeyStr)
        {
            string validationError = string.Empty;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var workbook = new XLWorkbook(fileStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    int rowIndex = 0;

                    foreach (var row in worksheet.Rows())
                    {
                        if (rowIndex == 0)
                        {
                            // Only Header Cell

                            int cellIndex = 0;
                            foreach (var cell in row.Cells())
                            {
                                try
                                {
                                    if (headerKeyStr.Count > cellIndex)
                                    {
                                        if (cell.Value.ToString() != headerKeyStr[cellIndex])
                                        {
                                            validationError += Environment.NewLine + "Invalid File Header at Cell(" + cellIndex + ") ";
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    validationError += Environment.NewLine + "Invalid File Header at Cell(" + cellIndex + ") ";
                                }

                                cellIndex++;
                            }
                        }
                        else
                        {
                            // Only Header check
                            break;
                        }

                        rowIndex++;
                    }
                }
            }

            return validationError;
        }

        public async Task<string> ValidateFileContentTypeDependingOnIndex(string filePath, Dictionary<int, Type> headerKey)
        {
            string validationError = string.Empty;

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (var workbook = new XLWorkbook(fileStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    int rowIndex = 0;

                    foreach (var row in worksheet.Rows())
                    {
                        if (rowIndex == 0)
                        {
                            //firstRow = false;
                        }
                        else
                        {
                            int cellIndex = 0;
                            foreach (var cell in row.Cells())
                            {
                                try
                                {
                                    if (headerKey.Count > cellIndex)
                                    {
                                        //var test = Convert.ChangeType(cell.Value.ToString(), headerKey[cellIndex]);
                                        Convert.ChangeType(cell.Value.ToString(), headerKey[cellIndex]);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    validationError += Environment.NewLine + "Invalid data at Row(" + rowIndex + "), Cell(" + cellIndex + ") ";
                                }

                                cellIndex++;
                            }
                        }

                        rowIndex++;
                    }
                }
            }

            return validationError;
        }

        public async Task<ResponseMessage> ValidateFileHeaderMatch(string filePath, Dictionary<int, string> headerKey, int userIDLoggedIn)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {


            }
            return null;
        }

        #region "Private"

        private async Task<FileFormatCheckOutPut> GetCellHeaderKeyAndCellContentTypeFromFile(string formaterFilePath)
        {
            FileFormatCheckOutPut fileFormatCheckOutPut = new FileFormatCheckOutPut();
            string validationError = string.Empty;
            List<FileFormatInfo> lstHeaderKeyAndType = new List<FileFormatInfo>();

            using (FileStream fileStream = new FileStream(formaterFilePath, FileMode.Open))
            {
                using (var workbook = new XLWorkbook(fileStream))
                {
                    var worksheet = workbook.Worksheet(1);
                    int rowIndex = 0;

                    foreach (var row in worksheet.Rows())
                    {
                        if (rowIndex == 0)
                        {
                            //** 1st of Cell (Header cell Key)
                            int cellIndex = 0;
                            foreach (var cell in row.Cells())
                            {
                                try
                                {
                                    if (cell.Value.ToString() == "END")
                                    {
                                        break;
                                    }

                                    FileFormatInfo objFileFormatChecker = new FileFormatInfo();
                                    objFileFormatChecker.index = cellIndex;
                                    objFileFormatChecker.cellHeaderKeyName = cell.Value.ToString();
                                    lstHeaderKeyAndType.Add(objFileFormatChecker);
                                }
                                catch (Exception ex)
                                {
                                    validationError += Environment.NewLine + "Invalid given file formater at Row(" + rowIndex + "), Cell(" + cellIndex + ") ";
                                }

                                cellIndex++;
                            }
                        }
                        else if (rowIndex == 1)
                        {
                            //** 2nd row 
                            int cellIndex = 0;
                            foreach (var cell in row.Cells())
                            {
                                if (cell.Value.ToString() == "END")
                                {
                                    break;
                                }

                                try
                                {
                                    lstHeaderKeyAndType[cellIndex].cellContentType = Type.GetType(cell.Value.ToString()); // Type.GetType("System.DateTime")
                                }
                                catch (Exception ex)
                                {
                                    validationError += Environment.NewLine + "Invalid given file type format at Row(" + rowIndex + "), Cell(" + cellIndex + ") ";
                                }

                                cellIndex++;
                            }
                        }
                        else
                        {
                            break;
                        }

                        rowIndex++;
                    }
                }
            }

            fileFormatCheckOutPut.lstFileFormatChecker = lstHeaderKeyAndType;
            fileFormatCheckOutPut.errorMessageOfFormatChecker = validationError;
            return fileFormatCheckOutPut;
        }


        #endregion

    }

    public interface IFileFormatChecker
    {
        Task<string> ValidateFileCellHeaderAndCellContentType(string filePath, List<FileFormatInfo> lstHeaderKeyAndType);
        // Task<FileFormatCheckOutPut> GetCellHeaderKeyAndCellContentTypeFromFile(string formaterFilePath);
        Task<string> ValidateFileCellHeader(string filePath, Dictionary<int, string> headerKeyStr);
        Task<string> ValidateFileContentTypeDependingOnIndex(string filePath, Dictionary<int, Type> headerKey);
        Task<ResponseMessage> ValidateFileHeaderMatch(string filePath, Dictionary<int, string> headerKey, int userIDLoggedIn);
        Task<string> ValidateFileCellHeaderAndCellContentTypeWithStandardFile(string formaterFilePath, string toBeCheckedFilePath);
    }




}
