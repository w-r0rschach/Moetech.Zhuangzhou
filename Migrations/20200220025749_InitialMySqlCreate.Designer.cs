﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moetech.Zhuangzhou.Data;

namespace Moetech.Zhuangzhou.Migrations
{
    [DbContext(typeof(VirtualMachineDB))]
    [Migration("20200220025749_InitialMySqlCreate")]
    partial class InitialMySqlCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.CommonAuthority", b =>
                {
                    b.Property<int>("AuthorityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AuthorityName")
                        .HasColumnType("int")
                        .HasMaxLength(20);

                    b.HasKey("AuthorityId");

                    b.ToTable("CommonAuthority");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.CommonCorrelation", b =>
                {
                    b.Property<int>("CorrelationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("PersonnelId")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("CorrelationId");

                    b.ToTable("CommonCorrelation");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.CommonDepartment", b =>
                {
                    b.Property<int>("DepId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DepName")
                        .IsRequired()
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.Property<int>("ParentNumber")
                        .HasColumnType("int");

                    b.HasKey("DepId");

                    b.ToTable("CommonDepartment");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.CommonPersonnelInfo", b =>
                {
                    b.Property<int>("PersonnelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int>("AppMaxCount")
                        .HasColumnType("int");

                    b.Property<string>("Avatar")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("Degree")
                        .HasColumnType("int");

                    b.Property<int>("DepId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DepartureTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IdentityCard")
                        .HasColumnType("varchar(18) CHARACTER SET utf8mb4")
                        .HasMaxLength(18);

                    b.Property<int>("IsSecrecy")
                        .HasColumnType("int");

                    b.Property<int>("IsStruggle")
                        .HasColumnType("int");

                    b.Property<int>("IsWork")
                        .HasColumnType("int");

                    b.Property<string>("LiveAddress")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Mailbox")
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<int>("MaritalStatus")
                        .HasColumnType("int");

                    b.Property<string>("Nation")
                        .HasColumnType("varchar(6) CHARACTER SET utf8mb4")
                        .HasMaxLength(6);

                    b.Property<DateTime>("OnboardingTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Password")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("PersonnelName")
                        .HasColumnType("varchar(50) CHARACTER SET utf8mb4")
                        .HasMaxLength(50);

                    b.Property<int>("PersonnelNo")
                        .HasColumnType("int");

                    b.Property<int>("PersonnelSex")
                        .HasColumnType("int");

                    b.Property<string>("Phone")
                        .HasColumnType("varchar(500) CHARACTER SET utf8mb4")
                        .HasMaxLength(500);

                    b.Property<DateTime>("TrialTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("WeChatAccount")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.HasKey("PersonnelId");

                    b.ToTable("CommonPersonnelInfo");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.CommonRole", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.HasKey("RoleId");

                    b.ToTable("CommonRole");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.CommonRoleAuthority", b =>
                {
                    b.Property<int>("RoleAuthorityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AuthorityId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("RoleAuthorityId");

                    b.ToTable("CommonRoleAuthority");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.MachApplyAndReturn", b =>
                {
                    b.Property<int>("ApplyAndReturnId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("ApplyTime")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ApplyUserID")
                        .HasColumnType("int");

                    b.Property<int>("ExamineResult")
                        .HasColumnType("int");

                    b.Property<int>("ExamineUserID")
                        .HasColumnType("int");

                    b.Property<int>("MachineInfoID")
                        .HasColumnType("int");

                    b.Property<int>("OprationType")
                        .HasColumnType("int");

                    b.Property<string>("Remark")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<DateTime>("ResultTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("ApplyAndReturnId");

                    b.ToTable("MachApplyAndReturn");
                });

            modelBuilder.Entity("Moetech.Zhuangzhou.Models.MachineInfo", b =>
                {
                    b.Property<int>("MachineId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<double>("MachineDiskCount")
                        .HasColumnType("double");

                    b.Property<string>("MachineIP")
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.Property<double>("MachineMemory")
                        .HasColumnType("double");

                    b.Property<string>("MachinePassword")
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.Property<int>("MachineState")
                        .HasColumnType("int");

                    b.Property<int>("MachineSystem")
                        .HasColumnType("int");

                    b.Property<string>("MachineUser")
                        .HasColumnType("varchar(20) CHARACTER SET utf8mb4")
                        .HasMaxLength(20);

                    b.HasKey("MachineId");

                    b.ToTable("MachineInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
