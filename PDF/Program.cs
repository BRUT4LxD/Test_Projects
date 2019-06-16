using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace PDF
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Document document = new Document(PageSize.LETTER);

            string path = "magicPdf.pdf";
            using (FileStream stream = File.Create(path))
            {
                var messageWallBuilder = new PdfMessageWallBuilder();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                document.Add(messageWallBuilder.BuildReply("Thor", DateTime.Now));
                document.Add(messageWallBuilder.BuildReply("Thor", DateTime.Now));
                document.Add(messageWallBuilder.BuildReply("Thor", DateTime.Now));
                document.Add(messageWallBuilder.BuildReply("Thor", DateTime.Now));
                document.Add(messageWallBuilder.BuildReply("Thor", DateTime.Now));
                document.Close();
            }
        }
    }

}
