﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ResumeAnalyzerAPI.Data;

#nullable disable

namespace ResumeAnalyzerAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("ResumeAnalyzerAPI.Entities.ResumeAnalysisHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("MatchingSkills")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.PrimitiveCollection<string>("MissingSkills")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Recommendations")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("SimilarityScore")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("ResumeAnalysisHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
