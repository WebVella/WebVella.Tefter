global using System.ComponentModel;
global using Microsoft.FluentUI.AspNetCore.Components;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.JSInterop;
global using Fluxor;
global using Microsoft.AspNetCore.Components;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Microsoft.Extensions.Configuration;
global using Microsoft.AspNetCore.StaticFiles;
global using Microsoft.AspNetCore.Hosting;
global using System.Runtime.ExceptionServices;

global using WebVella.Tefter.Services;
global using WebVella.Tefter.Database;
global using WebVella.Tefter.Addons;
global using WebVella.Tefter.Models;
global using WebVella.Tefter.Migrations;

global using WebVella.Tefter.EmailSender;
global using WebVella.Tefter.EmailSender.Models;
global using WebVella.Tefter.EmailSender.Services;
global using WebVella.Tefter.EmailSender.Components;
global using WebVella.Tefter.EmailSender.Addons;

global using FluentValidation;
global using FluentValidation.Results;
global using Npgsql;
global using System.Data;
global using MimeKit;
global using MailKit.Net.Smtp;
global using MailKit.Security;