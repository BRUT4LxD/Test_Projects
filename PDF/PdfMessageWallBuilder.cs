using iTextSharp.text;
using iTextSharp.text.pdf;
using System;

namespace PDF
{
    internal class PdfMessageWallBuilder
    {
        private const float DateFontSize = 10;
        private const float DescriptionFontSize = 13;
        private const float TitleFontSize = 16;
        private const float NameFontSize = 11;
        private const int DateGrayIntensity = 150;
        private const int DescriptionGrayIntensity = 120;
        private const Font.FontFamily FontFamily = Font.FontFamily.HELVETICA;
        public PdfDiv BuildReply(string name, DateTime date)
        {
            var replyDiv = new PdfDiv();
            Paragraph nameWithDateParagraph = CreateNameWithDate(name, date);
            Paragraph descriptionParagraph = CreateReplyContent(
                "This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. This is a content. ");

            replyDiv.AddElement(nameWithDateParagraph);
            replyDiv.AddElement(descriptionParagraph);

            return replyDiv;
        }

        private static Paragraph CreateNameWithDate(string name, DateTime date)
        {
            var nameChunk = new Chunk(name) { Font = new Font(FontFamily, NameFontSize) };

            var replyDateChunk = new Chunk("   " + date.ToString("f"))
            {
                Font = new Font(FontFamily, DateFontSize)
            };
            replyDateChunk.Font.SetColor(DateGrayIntensity, DateGrayIntensity, DateGrayIntensity);

            // Wrap Chunks into Paragraph to ensure line break
            return new Paragraph { nameChunk, replyDateChunk, Chunk.NEWLINE };
        }

        private static Paragraph CreateReplyContent(string content)
        {
            var text = new Paragraph(content) { Font = new Font(FontFamily, DescriptionFontSize) };
            text.Font.SetColor(DescriptionGrayIntensity, DescriptionGrayIntensity, DescriptionGrayIntensity);
            text.Add(Chunk.NEWLINE);
            text.Add(Chunk.NEWLINE);
            return text;
        }
        public PdfDiv BuildConversation(string title)
        {
            PdfDiv pdfConversation = new PdfDiv();

            
            
            return pdfConversation;
        }

    }
}
