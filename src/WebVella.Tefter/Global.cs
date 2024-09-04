global using FluentResults;
global using FluentValidation;
global using FluentValidation.Results;
global using Fluxor;
global using Fluxor.Blazor.Web.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Routing;
global using Microsoft.Extensions.Localization;
global using Microsoft.FluentUI.AspNetCore.Components;
global using Microsoft.FluentUI.AspNetCore.Components.Extensions;
global using Microsoft.JSInterop;
global using Microsoft.Extensions.Hosting; 
global using Nito.AsyncEx;
global using Npgsql;
global using System.ComponentModel;
global using System.Globalization;
global using System.Net.Mail;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Web;



global using WebVella.Tefter.Database.Dbo;
global using WebVella.Tefter.Errors;
global using WebVella.Tefter.Identity;
global using WebVella.Tefter.Messaging;
global using WebVella.Tefter.Utility;
global using WebVella.Tefter.Models;

/// Use cases
global using WebVella.Tefter.UseCases.AppStart;
global using WebVella.Tefter.UseCases.SharedColumnsAdmin;
global using WebVella.Tefter.UseCases.DataProviderAdmin;
global using WebVella.Tefter.UseCases.Dashboard;
global using WebVella.Tefter.UseCases.FastAccess;
global using WebVella.Tefter.UseCases.Login;
global using WebVella.Tefter.UseCases.Models;
global using WebVella.Tefter.UseCases.StateEffects;
global using WebVella.Tefter.UseCases.UserAdmin;
global using WebVella.Tefter.UseCases.Space;

//State
global using WebVella.Tefter.Web.Store.CultureState;
global using WebVella.Tefter.Web.Store.DataProviderAdminState;
global using WebVella.Tefter.Web.Store.DashboardState;
global using WebVella.Tefter.Web.Store.FastAccessState;
global using WebVella.Tefter.Web.Store.ScreenState;
global using WebVella.Tefter.Web.Store.SessionState;
global using WebVella.Tefter.Web.Store.SpaceState;
global using WebVella.Tefter.Web.Store.ThemeState;
global using WebVella.Tefter.Web.Store.UserAdminState;
global using WebVella.Tefter.Web.Store.UserState;

/// Components
global using WebVella.Tefter.Web.Brokers;
global using WebVella.Tefter.Web.Components;
global using WebVella.Tefter.Web.Models;
global using WebVella.Tefter.Web.Services;
global using WebVella.Tefter.Web.Utils;
global using Icon = Microsoft.FluentUI.AspNetCore.Components.Icon;
global using Color = Microsoft.FluentUI.AspNetCore.Components.Color;
