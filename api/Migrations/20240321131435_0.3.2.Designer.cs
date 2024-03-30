﻿// <auto-generated />
using FeChat.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace api.Migrations
{
    [DbContext(typeof(Db))]
    [Migration("20240321131435_0.3.2")]
    partial class _032
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FeChat.Models.Entities.Events.EventEntity", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("EventId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<int>("TypeId")
                        .HasColumnType("integer");

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Members.MemberEntity", b =>
                {
                    b.Property<int>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MemberId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Password")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("ResetCode")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<int>("ResetTime")
                        .HasColumnType("integer");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("MemberId");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Members.MemberOptionsEntity", b =>
                {
                    b.Property<int>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OptionId"));

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("OptionName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("OptionValue")
                        .HasColumnType("text");

                    b.HasKey("OptionId");

                    b.ToTable("MembersOptions");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Messages.AttachmentEntity", b =>
                {
                    b.Property<int>("AttachmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AttachmentId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<string>("Link")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int>("MessageId")
                        .HasColumnType("integer");

                    b.HasKey("AttachmentId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Messages.GuestEntity", b =>
                {
                    b.Property<int>("GuestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("GuestId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("GuestId");

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Messages.MessageEntity", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MessageId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("Message")
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<int>("Seen")
                        .HasColumnType("integer");

                    b.Property<int>("ThreadId")
                        .HasColumnType("integer");

                    b.HasKey("MessageId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Messages.ThreadEntity", b =>
                {
                    b.Property<int>("ThreadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ThreadId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<int>("GuestId")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("ThreadSecret")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("WebsiteId")
                        .HasColumnType("integer");

                    b.HasKey("ThreadId");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Messages.TypingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<int>("ThreadId")
                        .HasColumnType("integer");

                    b.Property<int>("Updated")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Typing");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Plans.PlanEntity", b =>
                {
                    b.Property<int>("PlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("PlanId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<string>("Currency")
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.Property<string>("Name")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Price")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("PlanId");

                    b.ToTable("Plans");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Plans.PlansFeaturesEntity", b =>
                {
                    b.Property<int>("FeatureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("FeatureId"));

                    b.Property<string>("FeatureText")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int>("PlanId")
                        .HasColumnType("integer");

                    b.HasKey("FeatureId");

                    b.ToTable("PlansFeatures");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Plans.PlansMetaEntity", b =>
                {
                    b.Property<int>("MetaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MetaId"));

                    b.Property<string>("MetaName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("MetaValue")
                        .HasColumnType("text");

                    b.Property<int>("PlanId")
                        .HasColumnType("integer");

                    b.HasKey("MetaId");

                    b.ToTable("PlansMeta");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Plans.PlansRestrictionsEntity", b =>
                {
                    b.Property<int>("RestrictionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RestrictionId"));

                    b.Property<int>("PlanId")
                        .HasColumnType("integer");

                    b.Property<string>("RestrictionName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int>("RestrictionValue")
                        .HasColumnType("integer");

                    b.HasKey("RestrictionId");

                    b.ToTable("PlansRestrictions");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Settings.SettingsEntity", b =>
                {
                    b.Property<int>("OptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("OptionId"));

                    b.Property<string>("OptionName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("OptionValue")
                        .HasColumnType("text");

                    b.HasKey("OptionId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Subscriptions.SubscriptionEntity", b =>
                {
                    b.Property<int>("SubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SubscriptionId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<int>("Expiration")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("NetId")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("OrderId")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("PlanId")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("SubscriptionId");

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Subscriptions.SubscriptionsMetaEntity", b =>
                {
                    b.Property<int>("MetaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("MetaId"));

                    b.Property<string>("MetaName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("MetaValue")
                        .HasColumnType("text");

                    b.Property<int>("SubscriptionId")
                        .HasColumnType("integer");

                    b.HasKey("MetaId");

                    b.ToTable("SubscriptionsMeta");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Transactions.TransactionEntity", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("TransactionId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("NetId")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("OrderId")
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<int>("PlanId")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("SubscriptionId")
                        .HasColumnType("integer");

                    b.HasKey("TransactionId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("FeChat.Models.Entities.Websites.WebsiteEntity", b =>
                {
                    b.Property<int>("WebsiteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("WebsiteId"));

                    b.Property<int>("Created")
                        .HasColumnType("integer");

                    b.Property<string>("Domain")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int>("Enabled")
                        .HasColumnType("integer");

                    b.Property<string>("Header")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<int>("MemberId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("Url")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.HasKey("WebsiteId");

                    b.ToTable("Websites");
                });
#pragma warning restore 612, 618
        }
    }
}