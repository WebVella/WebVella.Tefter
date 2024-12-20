﻿global using FluentResults;
global using FluentValidation;
global using FluentValidation.Results;
global using Fluxor;
global using Fluxor.Blazor.Web.Components;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Routing;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.FluentUI.AspNetCore.Components;
global using Microsoft.FluentUI.AspNetCore.Components.Extensions;
global using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;
global using Microsoft.JSInterop;
global using Nito.AsyncEx;
global using Npgsql;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Data;
global using System.Globalization;
global using System.Linq;
global using System.Net.Mail;
global using System.Reflection;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;
global using System.Web;
global using ClosedXML;
global using ClosedXML.Excel;
//webvella
global using WebVella.Tefter.Jobs;
global using WebVella.Tefter.Database;
global using WebVella.Tefter.Database.Dbo;
global using WebVella.Tefter.Errors;
global using WebVella.Tefter.Identity;
global using WebVella.Tefter.Messaging;
global using WebVella.Tefter.Migrations;
global using WebVella.Tefter.Models;
global using WebVella.Tefter.Services;

/// Use cases
global using WebVella.Tefter.UseCases.AppState;
global using WebVella.Tefter.UseCases.Login;
global using WebVella.Tefter.UseCases.UserState;
global using WebVella.Tefter.UseCases.Export;
global using WebVella.Tefter.Utility;
/// Components
global using WebVella.Tefter.Web.Components;
global using WebVella.Tefter.Web.Models;

//State
global using WebVella.Tefter.Web.Store;
global using WebVella.Tefter.Web.Utils;