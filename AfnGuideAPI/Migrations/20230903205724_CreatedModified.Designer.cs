﻿// <auto-generated />
using System;
using AfnGuideAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AfnGuideAPI.Migrations
{
    [DbContext(typeof(AfnGuideDbContext))]
    [Migration("20230903205724_CreatedModified")]
    partial class CreatedModified
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AfnGuideAPI.Models.Bulletin", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Bulletins");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Channel", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Abbreviation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ChannelNumber")
                        .HasColumnType("int");

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("EndTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsSplit")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("StartTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Promo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("AfnId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("ImageData")
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPromoB")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AfnId")
                        .IsUnique()
                        .HasFilter("[AfnId] IS NOT NULL");

                    b.ToTable("Promos");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AfnId")
                        .HasColumnType("int");

                    b.Property<DateTime>("AirDateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChannelId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("EpisodeTitle")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Genre")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPremiere")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Rating")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.TimeZone", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Abbreviation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DSTEndsOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DSTStartsOn")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsBackwards")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ModifiedOnUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ObservesDST")
                        .HasColumnType("bit");

                    b.Property<int>("Offset")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TimeZones");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Promo", b =>
                {
                    b.HasOne("AfnGuideAPI.Models.Schedule", "Schedule")
                        .WithOne("Promo")
                        .HasForeignKey("AfnGuideAPI.Models.Promo", "AfnId")
                        .HasPrincipalKey("AfnGuideAPI.Models.Schedule", "AfnId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Schedule", b =>
                {
                    b.HasOne("AfnGuideAPI.Models.Channel", "Channel")
                        .WithMany("Schedules")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Channel");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Channel", b =>
                {
                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("AfnGuideAPI.Models.Schedule", b =>
                {
                    b.Navigation("Promo");
                });
#pragma warning restore 612, 618
        }
    }
}
