using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchiSteamFarm;
using ArchiSteamFarm.Localization;
using static ArchiSteamFarm.Steam.Integration.ArchiWebHandler;
using ArchiSteamFarm.Core;
using System.Composition;
using ArchiSteamFarm.Plugins.Interfaces;
using ArchiSteamFarm.Steam;
using ArchiSteamFarm.Web;
using ArchiSteamFarm.Steam.Data;
using ArchiSteamFarm.Web.Responses;

namespace Booster.Booster
{
	[Export(typeof(IPlugin))]
	internal sealed class Booster : IBotCommand
	{


		public string Name => nameof(Booster);
		public Version Version => typeof(Booster).Assembly.GetName().Version;

		public async Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args)
		{
			

			switch (args[0].ToUpperInvariant())
			{
				case "CREATE":

					{

						return await ResponseCreateBoosters(bot, steamID, ArchiSteamFarm.Core.Utilities.GetArgsAsText(args, 1, ","), Utilities.GetArgsAsText(args, 2, ",")).ConfigureAwait(false);
						
					}

				case "SENDGEMS":

					{
						
						return await ResponseSendGems(bot, steamID, Utilities.GetArgsAsText(args, 1, ","),Utilities.GetArgsAsText(args,2,",")).ConfigureAwait(false);
						

					}
				default:
					
					return null;
			}

		}

		public void OnLoaded()
		{
			Console.WriteLine("Booster Plugin Loaded!");


		}



		private static async Task<string> ResponseCreateBoosters(Bot Bot,ulong steamID, string fileName)
		{
			
			if (steamID == 0)
			{
				Bot.ArchiLogger.LogNullError(nameof(steamID));

				return null;
			}

			

			if (!Bot.IsConnectedAndLoggedOn)
			{
				return Bot.Commands.FormatBotResponse(Strings.BotNotConnected);
			}

			List<uint> gameid = ReadGames.getCraftingids(fileName);

			if (gameid != null)
			{

				foreach (uint i in gameid)
				{

					await CreateBooster(Bot,i).ConfigureAwait(false);
					await Task.Delay(1000).ConfigureAwait(false);


				}
				Console.Beep();
				Console.Beep();
			}
			else
			{
				return Bot.Commands.FormatBotResponse(Strings.NothingFound);
			}


			return Bot.Commands.FormatBotResponse(Strings.Done);
		}

		
		private static async Task<string> ResponseCreateBoosters(Bot Bot,ulong steamID, string botNames, string fileName)
		{
			if ((steamID == 0) || string.IsNullOrEmpty(botNames))
			{
				ASF.ArchiLogger.LogNullError(nameof(steamID) + " || " + nameof(botNames));

				return null;
			}

			HashSet<Bot> bots = Bot.GetBots(botNames);

			if ((bots == null) || (bots.Count == 0))
			{
				return  Bot.Commands.FormatBotResponse(string.Format(Strings.BotNotFound, botNames));
			}
			IList<string> results = await Utilities.InParallel(bots.Select(bot => ResponseCreateBoosters(bot,steamID,fileName))).ConfigureAwait(false);

			List<string> responses = new List<string>(results.Where(result => !string.IsNullOrEmpty(result)));

			return responses.Count > 0 ? string.Join(Environment.NewLine, responses) : null;
		}

		internal static async Task<bool> CreateBooster(Bot Bot,uint appID)
		{
			if (appID == 0)
			{
				Bot.ArchiLogger.LogNullError(nameof(appID)); 

				return false;
			}

			string profileURL = await Bot.ArchiWebHandler.GetAbsoluteProfileURL().ConfigureAwait(false);
			
			if (string.IsNullOrEmpty(profileURL))
			{
				Bot.ArchiLogger.LogGenericWarning(Strings.WarningFailed);

				return false;
			}

			
			Uri request = new(SteamCommunityURL + "/tradingcards/ajaxcreatebooster/");
			// Extra entry for sessionID
			Dictionary<string, string> data = new Dictionary<string, string>(3, StringComparer.Ordinal) {
				{ "appid", appID.ToString() },
				{ "series",1.ToString() },
				{"tradability_preference",2.ToString() },
			};

			ObjectResponse<ResultResponse> response = await Bot.ArchiWebHandler.UrlPostToJsonObjectWithSession<ResultResponse>(request, data: data).ConfigureAwait(false);

			return true;
		}

		private static async Task<string> ResponseSendGems(Bot Bot, ulong steamID, string botNames,string gems)
		{

			if (steamID == 0)
			{
				Bot.ArchiLogger.LogNullError(nameof(steamID));

				return null;
			}

			

			if (!Bot.IsConnectedAndLoggedOn)
			{
				return Bot.Commands.FormatBotResponse(Strings.BotNotConnected);
			}
			HashSet<Bot> bots = Bot.GetBots(botNames);
			bots.Remove(Bot);

			foreach(Bot b in bots) {
				
			IAsyncEnumerable<ArchiSteamFarm.Steam.Data.Asset> inventory =  Bot.ArchiWebHandler.GetInventoryAsync(ASF.GlobalConfig.SteamOwnerID);

			HashSet<ArchiSteamFarm.Steam.Data.Asset> send = new HashSet<ArchiSteamFarm.Steam.Data.Asset>();
				
				
				await foreach (ArchiSteamFarm.Steam.Data.Asset item in inventory)
			{
				
				if (item.Type == ArchiSteamFarm.Steam.Data.Asset.EType.SteamGems && !item.Marketable && item.Tradable)
				{



						ArchiSteamFarm.Steam.Data.Asset i = new ArchiSteamFarm.Steam.Data.Asset(753,6,item.ClassID,Convert.ToUInt32(gems),item.InstanceID,item.AssetID,false,true,item.Tags,item.RealAppID,item.Type,item.Rarity);
						send.Add(i);
					}
			}


				
				(bool Success, HashSet<ulong> MobileTradeOfferIDs) = await Bot.ArchiWebHandler.SendTradeOffer(b.SteamID,send).ConfigureAwait(false);
				await Bot.Actions.HandleTwoFactorAuthenticationConfirmations(true).ConfigureAwait(false);
				
				
			}
			

			
			
			return Bot.Commands.FormatBotResponse(Strings.Done);

		}

		


			
		}
		}






	

