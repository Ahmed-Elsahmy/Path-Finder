using BLL.Dtos.AiDtos;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services.ResumeBuilderService
{
    public static class ResumePdfGenerator
    {
        // Color palette
        private static readonly string PrimaryColor = "#1a237e";
        private static readonly string AccentColor = "#303f9f";
        private static readonly string TextDark = "#212121";
        private static readonly string TextMedium = "#424242";
        private static readonly string TextLight = "#757575";
        private static readonly string DividerColor = "#e0e0e0";
        private static readonly string AccentBg = "#f5f5f5";
        private static readonly string SkillBadgeBg = "#e8eaf6";
        private static readonly string SkillBadgeText = "#283593";

        public static byte[] Generate(ResumeBuilderRS resume, string style = "Professional")
        {
            // 🚨 مُهم جداً: ترخيص الاستخدام المجاني للمكتبة
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginTop(30);
                    page.MarginBottom(30);
                    page.MarginHorizontal(35);

                    page.Content().Column(col =>
                    {
                        // 1. Header — Name + Contact Info
                        col.Item().Container().PaddingBottom(8).Column(header =>
                        {
                            header.Item().Text(resume.FullName ?? "Your Name")
                                .FontSize(26)
                                .Bold()
                                .FontColor(PrimaryColor);

                            var contactParts = new List<string>();
                            if (!string.IsNullOrEmpty(resume.Email)) contactParts.Add(resume.Email);
                            if (!string.IsNullOrEmpty(resume.Phone)) contactParts.Add(resume.Phone);
                            if (!string.IsNullOrEmpty(resume.Location)) contactParts.Add(resume.Location);

                            if (contactParts.Any())
                            {
                                header.Item().PaddingTop(4).Text(string.Join("  •  ", contactParts))
                                    .FontSize(9)
                                    .FontColor(TextMedium);
                            }
                        });

                        col.Item().LineHorizontal(2).LineColor(AccentColor);

                        // 2. Professional Summary
                        if (!string.IsNullOrWhiteSpace(resume.ProfessionalSummary))
                        {
                            col.Item().PaddingTop(12).Column(section =>
                            {
                                SectionHeader(section, "PROFESSIONAL SUMMARY");
                                section.Item().PaddingTop(4)
                                    .Background(AccentBg)
                                    .Padding(10)
                                    .Text(resume.ProfessionalSummary)
                                    .FontSize(9.5f)
                                    .FontColor(TextDark)
                                    .LineHeight(1.5f);
                            });
                        }

                        // 3. Skills
                        if (resume.SkillSections != null && resume.SkillSections.Any())
                        {
                            col.Item().PaddingTop(12).Column(section =>
                            {
                                SectionHeader(section, "TECHNICAL & SOFT SKILLS");
                                foreach (var skillGroup in resume.SkillSections)
                                {
                                    section.Item().PaddingTop(4).Row(row =>
                                    {
                                        row.ConstantItem(120).Text(skillGroup.Category + ":")
                                            .FontSize(9).Bold().FontColor(AccentColor);

                                        row.RelativeItem().Text(text =>
                                        {
                                            for (int i = 0; i < skillGroup.Skills.Count; i++)
                                            {
                                                text.Span(skillGroup.Skills[i]).FontSize(9).FontColor(TextDark);
                                                if (i < skillGroup.Skills.Count - 1)
                                                    text.Span("  |  ").FontSize(9).FontColor(TextLight);
                                            }
                                        });
                                    });
                                }
                            });
                        }

                        // 4. Experience
                        if (resume.Experience != null && resume.Experience.Any())
                        {
                            col.Item().PaddingTop(12).Column(section =>
                            {
                                SectionHeader(section, "PROFESSIONAL EXPERIENCE");
                                foreach (var exp in resume.Experience)
                                {
                                    section.Item().PaddingTop(8).Column(entry =>
                                    {
                                        entry.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text(exp.Position).FontSize(10.5f).Bold().FontColor(TextDark);
                                            row.ConstantItem(130).AlignRight().Text(exp.Duration).FontSize(9).FontColor(TextLight);
                                        });

                                        entry.Item().Text(exp.Company).FontSize(9.5f).Italic().FontColor(AccentColor);

                                        if (exp.BulletPoints != null && exp.BulletPoints.Any())
                                        {
                                            foreach (var bullet in exp.BulletPoints)
                                            {
                                                entry.Item().PaddingTop(3).PaddingLeft(12).Row(bulletRow =>
                                                {
                                                    bulletRow.ConstantItem(10).Text("▸").FontSize(8).FontColor(AccentColor);
                                                    bulletRow.RelativeItem().Text(bullet).FontSize(9).FontColor(TextMedium).LineHeight(1.4f);
                                                });
                                            }
                                        }
                                    });
                                }
                            });
                        }

                        // 5. Education
                        if (resume.Education != null && resume.Education.Any())
                        {
                            col.Item().PaddingTop(12).Column(section =>
                            {
                                SectionHeader(section, "EDUCATION");
                                foreach (var edu in resume.Education)
                                {
                                    section.Item().PaddingTop(6).Column(entry =>
                                    {
                                        entry.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text(text =>
                                            {
                                                text.Span(edu.Degree).FontSize(10.5f).Bold().FontColor(TextDark);
                                                if (!string.IsNullOrEmpty(edu.FieldOfStudy))
                                                    text.Span($" — {edu.FieldOfStudy}").FontSize(10).FontColor(TextMedium);
                                            });
                                            row.ConstantItem(130).AlignRight().Text(edu.Duration).FontSize(9).FontColor(TextLight);
                                        });
                                        entry.Item().Text(edu.Institution).FontSize(9.5f).Italic().FontColor(AccentColor);
                                    });
                                }
                            });
                        }

                        // 6. Certifications
                        if (resume.Certifications != null && resume.Certifications.Any())
                        {
                            col.Item().PaddingTop(12).Column(section =>
                            {
                                SectionHeader(section, "CERTIFICATIONS");
                                foreach (var cert in resume.Certifications)
                                {
                                    section.Item().PaddingTop(3).PaddingLeft(12).Row(row =>
                                    {
                                        row.ConstantItem(10).Text("✦").FontSize(7).FontColor(AccentColor);
                                        row.RelativeItem().Text(cert).FontSize(9).FontColor(TextDark);
                                    });
                                }
                            });
                        }

                        // 7. Additional Sections
                        if (!string.IsNullOrWhiteSpace(resume.AdditionalSections))
                        {
                            col.Item().PaddingTop(12).Column(section =>
                            {
                                SectionHeader(section, "ADDITIONAL INFORMATION");
                                section.Item().PaddingTop(4).Text(resume.AdditionalSections)
                                    .FontSize(9).FontColor(TextMedium).LineHeight(1.4f);
                            });
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated by Path Finder AI  •  ").FontSize(7).FontColor(TextLight);
                        text.CurrentPageNumber().FontSize(7).FontColor(TextLight);
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void SectionHeader(ColumnDescriptor column, string title)
        {
            column.Item().PaddingBottom(2).Column(header =>
            {
                header.Item().Text(title).FontSize(11).Bold().FontColor(PrimaryColor).LetterSpacing(0.08f);
                header.Item().PaddingTop(2).LineHorizontal(1).LineColor(DividerColor);
            });
        }
    }
}