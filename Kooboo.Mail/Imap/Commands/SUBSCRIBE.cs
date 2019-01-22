﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LumiSoft.Net.IMAP;
using LumiSoft.Net;

namespace Kooboo.Mail.Imap.Commands
{
    public class SUBSCRIBE : ICommand
    {
        public string AdditionalResponse
        {
            get;set;
        }

        public string CommandName
        {
            get
            {
                return "SUBSCRIBE";
            }
        }

        public bool RequireAuth
        {
            get
            {
                return true;
            }

        }

        public bool RequireFolder
        {
            get
            {
                return false;
            }
        }

        public bool RequireTwoPartCommand
        {
            get
            {
                return false;
            }
        }

        public Task<List<ImapResponse>> Execute(ImapSession session, string args)
        {
            var folderName = TextUtils.UnQuoteString(IMAP_Utils.DecodeMailbox(args));

            if (Folder.ReservedFolder.Any(o => folderName.StartsWith(o.Value, StringComparison.OrdinalIgnoreCase)))
                throw new CommandException("NO", "Reserved folders are always subscribed");

            var folder = session.MailDb.Folders.Get(folderName);
            if (folder == null)
                throw new CommandException("NO", "No such a folder");

            session.MailDb.Folders.Subscribe(folder);

            return this.NullResult();
        }
    }
}