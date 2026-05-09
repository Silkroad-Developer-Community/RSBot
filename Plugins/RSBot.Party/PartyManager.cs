using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RSBot.Core;
using RSBot.Core.Components;
using RSBot.Core.Event;
using RSBot.Core.Objects.Party;

namespace RSBot.Party
{
    public class PartyManager
    {
        public PartyManager()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            EventManager.SubscribeEvent("OnEnterGame", OnEnterGame);
            EventManager.SubscribeEvent("OnDeletePartyEntry", OnDeletePartyEntry);
            EventManager.SubscribeEvent("OnPartyMemberLeave", new Action<PartyMember>(OnPartyMemberLeave));
            EventManager.SubscribeEvent("OnPartyMemberBanned", new Action<PartyMember>(OnPartyMemberBanned));
            EventManager.SubscribeEvent("OnPartyDismiss", OnPartyDismiss);
            EventManager.SubscribeEvent("OnAgentServerDisconnected", OnPartyDismiss);
            EventManager.SubscribeEvent("OnPartyLeaderChange", OnPartyData);
            EventManager.SubscribeEvent("OnPartyData", OnPartyData);
        }

        private async void OnEnterGame()
        {
            await Task.Delay(5000);

            if (
                Game.Ready
                && Bundle.Container.PartyMatching.Config.AutoReform
                && !Bundle.Container.PartyMatching.HasMatchingEntry
            )
                Bundle.Container.PartyMatching.Create();
        }

        private void OnPartyMemberLeave(PartyMember member)
        {
            if (Bundle.Container.PartyMatching.Config.AutoReform)
                if (
                    Game.Party != null
                    && !Bundle.Container.PartyMatching.HasMatchingEntry
                    && Game.Party.Members?.Count < Game.Party.Settings.MaxMember
                )
                    Bundle.Container.PartyMatching.Create();
        }

        private void OnPartyMemberBanned(PartyMember member)
        {
            if (member.Name != Game.Player.Name)
            {
                if (Bundle.Container.PartyMatching.Config.AutoReform)
                    if (
                        Game.Party != null
                        && !Bundle.Container.PartyMatching.HasMatchingEntry
                        && Game.Party.Members?.Count < Game.Party.Settings.MaxMember
                    )
                        Bundle.Container.PartyMatching.Create();
            }
            else
                OnPartyDismiss();
        }

        public static void OnPartyDismiss()
        {
            if (!Game.Ready)
                return;

            Bundle.Container.PartyMatching.HasMatchingEntry = false;
            OnDeletePartyEntry();
        }

        private static void OnDeletePartyEntry()
        {
            Bundle.Container.PartyMatching.Id = 0;

            if (Game.Ready && Bundle.Container.PartyMatching.Config.AutoReform)
                Bundle.Container.PartyMatching.Create();
        }

        public void OnPartyData()
        {
            try
            {
                if (Game.Party.Members == null)
                {
                    OnPartyDismiss();
                    return;
                }
            }
            catch { }
        }

        public static string[] GetBuffingGroups()
        {
            return PlayerConfig.GetArray<string>("RSBot.Party.Buffing.Groups");
        }

        public static void SaveBuffingGroups(IEnumerable<string> buffs)
        {
            PlayerConfig.SetArray("RSBot.Party.Buffing.Groups", buffs);
        }

        public static string GetBuffingMembers()
        {
            return PlayerConfig.Get("RSBot.Party.Buffing", string.Empty);
        }

        public static void SaveBuffingPartyMembers(string members)
        {
            PlayerConfig.Set("RSBot.Party.Buffing", members);
            PlayerConfig.Save();

            EventManager.FireEvent("OnPartyBuffSettingsChanged");
        }

        public static void SaveAutoPartyPlayerList(IEnumerable<string> players)
        {
            PlayerConfig.SetArray("RSBot.Party.AutoPartyList", players);

            Bundle.Container.Refresh();
        }

        public static void SaveCommandPlayersList(IEnumerable<string> players)
        {
            PlayerConfig.SetArray("RSBot.Party.Commands.PlayersList", players);

            Bundle.Container.Refresh();
        }

        public static string GetSelectedGroup()
        {
            return PlayerConfig.Get("RSBot.Party.Buffing.SelectedGroup", "Default");
        }

        public static void LeaveParty()
        {
            Game.Party?.Leave();
        }

        public static void SetPartyExpAutoShare(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.EXPAutoShare", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyItemAutoShare(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.ItemAutoShare", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAllowInvites(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AllowInvitations", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoAcceptInvitesAll(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AcceptAll", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoAcceptInvitesFromList(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AcceptList", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoInviteAll(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.InviteAll", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoInviteList(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.InviteList", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoAcceptAtTrainingPlace(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AtTrainingPlace", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoAcceptIfBotStopped(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AcceptIfBotStopped", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoLeaveIfMasterNot(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.LeaveIfMasterNot", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoLeaveMaster(string masterName)
        {
            PlayerConfig.Set("RSBot.Party.LeaveIfMasterNotName", masterName);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyCommandListenFromMaster(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.Commands.ListenFromMaster", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyCommandListenFromList(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.Commands.ListenOnlyList", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetFollowMaster(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AlwaysFollowPartyMaster", enabled);

            Bundle.Container.Refresh();
        }

        public static void SetPartyAutoJoinByName(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AutoJoin.ByName", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoJoinByTitle(bool enabled)
        {
            PlayerConfig.Set("RSBot.Party.AutoJoin.ByTitle", enabled);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoJoinName(string name)
        {
            PlayerConfig.Set("RSBot.Party.AutoJoin.Name", name);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void SetPartyAutoJoinTitle(string title)
        {
            PlayerConfig.Set("RSBot.Party.AutoJoin.Title", title);
            Bundle.Container.AutoParty.Refresh();
        }

        public static void BanishMemberFromParty(string memberName)
        {
            if (Game.Party.IsLeader)
                Game.Party.GetMemberByName(memberName)?.Banish();
        }

        public static void DeletePartyMatchingEntry()
        {
            if (!Game.Ready)
                return;
            Bundle.Container.PartyMatching.Config.AutoReform = false;
            Bundle.Container.PartyMatching.Delete();
        }
    }
}
