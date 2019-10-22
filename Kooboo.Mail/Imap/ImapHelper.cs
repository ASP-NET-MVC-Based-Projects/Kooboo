//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Mail.Imap
{
    public static class ImapHelper
    {
        public static List<Range> GetSequenceRange(string sequenceSet)
        {
            /* RFC 3501
               seq-number     = nz-number / "*"
                               ; message sequence number (COPY, FETCH, STORE
                               ; commands) or unique identifier (UID COPY,
                               ; UID FETCH, UID STORE commands).
                               ; * represents the largest number in use.  In
                               ; the case of message sequence numbers, it is
                               ; the number of messages in a non-empty mailbox.
                               ; In the case of unique identifiers, it is the
                               ; unique identifier of the last message in the
                               ; mailbox or, if the mailbox is empty, the
                               ; mailbox's current UIDNEXT value.
                               ; The server should respond with a tagged BAD
                               ; response to a command that uses a message
                               ; sequence number greater than the number of
                               ; messages in the selected mailbox.  This
                               ; includes "*" if the selected mailbox is empty.

               seq-range      = seq-number ":" seq-number
                               ; two seq-number values and all values between
                               ; these two regardless of order.
                               ; Example: 2:4 and 4:2 are equivalent and indicate
                               ; values 2, 3, and 4.
                               ; Example: a unique identifier sequence range of
                               ; 3291:* includes the UID of the last message in
                               ; the mailbox, even if that value is less than 3291.

               sequence-set    = (seq-number / seq-range) *("," sequence-set)
                               ; set of seq-number values, regardless of order.
                               ; Servers MAY coalesce overlaps and/or execute the
                               ; sequence in any order.
                               ; Example: a message sequence number set of
                               ; 2,4:7,9,12:* for a mailbox with 15 messages is
                               ; equivalent to 2,4,5,6,7,9,12,13,14,15
                               ; Example: a message sequence number set of *:4,5:7
                               ; for a mailbox with 10 messages is equivalent to
                               ; 10,9,8,7,6,5,4,5,6,7 and MAY be reordered and
                               ; overlap coalesced to be 4,5,6,7,8,9,10.

             */

            List<Range> result = new List<Range>();

            if (string.IsNullOrEmpty(sequenceSet))
            {
                return null;
            }
            //A sequence set depicts a range of numbers between start and end. The starting and ending numbers in the sequence are separated by a : (colon).In the example command with tag A2, mails with sequence numbers 2 to 4 are being requested.If the command is prefixed with UID, the sequence set is a range of unique identifiers of the email and not the sequence number.

            var seps = ",;".ToCharArray();
            string[] values = sequenceSet.Split(seps, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in values)
            {
                if (item.Contains(":"))
                {
                    var parts = item.Split(':').ToList();
                    if (parts.Count() == 1)
                    {
                        int one = GetNumber(parts[0]);
                        if (one != 0)
                        {
                            result.Add(new Range() { LowBound = one, UpBound = one });
                        }
                    }
                    else
                    {
                        int one = GetNumber(parts[0]);
                        int two = GetNumber(parts[1]);
                        result.Add(one < two
                            ? new Range() {LowBound = one, UpBound = two}
                            : new Range() {LowBound = two, UpBound = one});
                    }
                }
                else
                {
                    var onevalue = GetNumber(item);
                    if (onevalue > 0)
                    {
                        result.Add(new Range() { LowBound = onevalue, UpBound = onevalue });
                    }
                }
            }

            return result;
        }

        public static void CorrectRange(List<Range> range, SelectFolder folder, bool isUid = true)
        {
            // correct *, before = int.max;
            foreach (var item in range)
            {
                if (item.UpBound == int.MaxValue)
                {
                    item.UpBound = getStarNumber(folder, isUid);
                }
            }
        }

        private static int getStarNumber(SelectFolder folder, bool uid = false)
        {
            // See   RFC seq-number.
            return uid ? folder.Stat.LastestMsgId : folder.Stat.Exists;
        }

        public class Range
        {
            public int LowBound { get; set; }

            public int UpBound { get; set; }
        }

        private static int GetNumber(string intstring)
        {
            if (intstring == "*")
            {
                return int.MaxValue;
            }

            int.TryParse(intstring, out var value);

            return value;
        }

        public static bool IsSequenceSet(string sequenceSet)
        {
            if (string.IsNullOrWhiteSpace(sequenceSet))
            {
                return false;
            }

            bool hasnumber = false;

            for (int i = 0; i < sequenceSet.Length; i++)
            {
                var currentchar = sequenceSet[i];

                if (Lib.Helper.CharHelper.isAsciiDigit(currentchar))
                {
                    hasnumber = true;
                    continue;
                }

                if (currentchar == '*' || currentchar == '.' || currentchar == ':' || currentchar == ',')
                {
                    continue;
                }
                return false;
            }
            return hasnumber;
        }

        public static List<ImapFolder> GetSubscribedFolder(User user)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);

            var allfolder = maildb.Folders.All().Where(o => o.Subscribed).ToList();

            return ToImapFolders(user, allfolder);
        }

        public static List<ImapFolder> GetAllFolder(User user)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);

            var allfolder = maildb.Folders.All();

            return ToImapFolders(user, allfolder);
        }

        public static List<ImapFolder> ToImapFolders(User user, List<Folder> folders)
        {
            var attributes = new List<string> {"\\NoInferiors"};

            List<ImapFolder> allImapFolder = new List<ImapFolder>();

            foreach (var item in folders)
            {
                char delimiter = default(char);
                if (item.Name.Contains("/"))
                {
                    delimiter = '/';
                }

                var newfolder = new ImapFolder() { Name = item.Name, Delimiter = delimiter, Attributes = attributes };

                if (item.Name.Contains("@"))
                {
                    newfolder.Attributes = attributes;
                }
                allImapFolder.Add(newfolder);
            }

            var orgdb = Factory.DBFactory.OrgDb(user.CurrentOrgId);

            var address = orgdb.EmailAddress.Query().Where(o => o.UserId == user.Id).SelectAll();

            var defaults = GetDefaultFolders(address);

            foreach (var item in defaults)
            {
                if (!allImapFolder.Contains(item))
                {
                    allImapFolder.Add(item);
                }
            }
            return allImapFolder;
        }

        internal static List<ImapFolder> GetDefaultFolders(List<EmailAddress> address)
        {
            List<ImapFolder> result = new List<ImapFolder>();
            foreach (var item in Folder.ReservedFolder)
            {
                // See https://tools.ietf.org/html/rfc6154 and https://tools.ietf.org/html/rfc3348 for attributes extensions
                var name = item.Value.ToLower();
                if (name == "inbox")
                {
                    // Add Inbox
                    result.Add(new ImapFolder
                    {
                        Name = "INBOX",
                        Attributes = new List<string>
                        {
                            "\\NoInferiors"
                        }
                    });

                    // Add address sub folders to Inbox
                    if (address != null && address.Count > 1)
                    {
                        foreach (var add in address)
                        {
                            result.Add(new ImapFolder
                            {
                                Name = item.Value + "/" + add.Address,
                                Delimiter = '/',
                                Attributes = new List<string>
                                {
                                    "\\NoInferiors"
                                }
                            });
                        }
                    }
                }
                else
                {
                    // Add Trash, Spam, Drafts, Sent folders
                    var folder = new ImapFolder
                    {
                        Name = item.Value,
                        Attributes = new List<string>
                        {
                            "\\NoInferiors"
                        }
                    };

                    if (name == "spam")
                    {
                        folder.Attributes.Add("\\Junk");
                    }
                    else
                    {
                        folder.Attributes.Add("\\" + item.Value);
                    }
                    result.Add(folder);
                }
            }

            return result;
        }

        public static string DateTimeToRfc2822(DateTime dateTime)
        {
            return dateTime.ToString("ddd, dd MMM yyyy HH':'mm':'ss ", System.Globalization.DateTimeFormatInfo.InvariantInfo) + dateTime.ToString("zzz").Replace(":", "");
        }

        public static DateTime ParseRfc2822Time(string timestring)
        {
            return DateTimeOffset.Parse(timestring).LocalDateTime.ToUniversalTime();
        }

        public static List<Commands.FetchCommand.FetchMessage> GetMessagesBySeqNo(MailDb mailDb, SelectFolder folder, List<Range> ranges)
        {
            CorrectRange(ranges, folder, false);

            var messages = new List<Commands.FetchCommand.FetchMessage>();
            foreach (var item in ranges)
            {
                var dbMessages = mailDb.Messages.GetBySeqNos(folder.Folder, folder.Stat.LastestMsgId, folder.Stat.Exists, item.LowBound, item.UpBound);
                var seqNo = item.LowBound;
                messages.AddRange(dbMessages.Select(o => new Commands.FetchCommand.FetchMessage
                {
                    MailDb = mailDb,
                    Message = o,
                    SeqNo = seqNo++
                }));
            }

            return messages;
        }

        public static List<Commands.FetchCommand.FetchMessage> GetMessagesByUid(MailDb mailDb, SelectFolder folder, List<Range> ranges)
        {
            CorrectRange(ranges, folder, true);

            var messages = new List<Commands.FetchCommand.FetchMessage>();
            foreach (var item in ranges)
            {
                var messagesInRange = mailDb.Messages.ByUidRange(folder.Folder, item.LowBound, item.UpBound);
                foreach (var message in messagesInRange)
                {
                    messages.Add(new Commands.FetchCommand.FetchMessage
                    {
                        MailDb = mailDb,
                        Message = message,
                        SeqNo = mailDb.Messages.GetSeqNo(folder.Folder, folder.Stat.LastestMsgId, folder.Stat.Exists, message.Id)
                    });
                }
            }

            return messages;
        }
    }
}

public class ImapFolder : IEqualityComparer<ImapFolder>
{
    public string Name { get; set; }
    public char Delimiter { get; set; }

    private List<string> _attributes;

    public List<string> Attributes
    {
        get { return _attributes ?? (_attributes = new List<string>()); }
        set
        {
            _attributes = value;
        }
    }

    public bool Equals(ImapFolder x, ImapFolder y)
    {
        return Kooboo.Lib.Helper.StringHelper.IsSameValue(x?.Name, y?.Name);
    }

    public int GetHashCode(ImapFolder obj)
    {
        throw new NotImplementedException();
    }
}