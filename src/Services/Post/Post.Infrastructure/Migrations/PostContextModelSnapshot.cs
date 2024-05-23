﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Post.Infrastructure.Persistence;

#nullable disable

namespace Post.Infrastructure.Migrations
{
    [DbContext(typeof(PostContext))]
    partial class PostContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Post.Domain.Entities.PostActivityLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<int>("FromStatus")
                        .HasColumnType("integer")
                        .HasColumnName("from_status");

                    b.Property<DateTimeOffset?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_date");

                    b.Property<string>("Note")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("note");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid")
                        .HasColumnName("post_id");

                    b.Property<int>("ToStatus")
                        .HasColumnType("integer")
                        .HasColumnName("to_status");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_post_activity_logs");

                    b.HasIndex("PostId")
                        .HasDatabaseName("ix_post_activity_logs_post_id");

                    b.ToTable("PostActivityLogs");
                });

            modelBuilder.Entity("Post.Domain.Entities.PostBase", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<Guid>("AuthorUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("author_user_id");

                    b.Property<long>("CategoryId")
                        .HasColumnType("bigint")
                        .HasColumnName("category_id");

                    b.Property<string>("Content")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("content");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_date");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean")
                        .HasColumnName("is_paid");

                    b.Property<DateTimeOffset?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("name");

                    b.Property<DateTimeOffset?>("PaidDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("paid_date");

                    b.Property<double>("RoyaltyAmount")
                        .HasColumnType("double precision")
                        .HasColumnName("royalty_amount");

                    b.Property<string>("SeoDescription")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("seo_description");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("varchar(250)")
                        .HasColumnName("slug");

                    b.Property<string>("Source")
                        .HasMaxLength(120)
                        .HasColumnType("character varying(120)")
                        .HasColumnName("source");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasDefaultValue(0)
                        .HasColumnName("status");

                    b.Property<string>("Summary")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("summary");

                    b.Property<string>("Tags")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)")
                        .HasColumnName("tags");

                    b.Property<string>("Thumbnail")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("thumbnail");

                    b.Property<int>("ViewCount")
                        .HasColumnType("integer")
                        .HasColumnName("view_count");

                    b.HasKey("Id")
                        .HasName("pk_posts");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasDatabaseName("ix_posts_slug");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Post.Domain.Entities.PostActivityLog", b =>
                {
                    b.HasOne("Post.Domain.Entities.PostBase", null)
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_post_activity_logs_posts_post_id");
                });
#pragma warning restore 612, 618
        }
    }
}
