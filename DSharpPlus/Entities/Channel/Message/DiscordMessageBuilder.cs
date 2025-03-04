// This file is part of the DSharpPlus project.
//
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2021 DSharpPlus Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DSharpPlus.Entities
{
    /// <summary>
    /// Constructs a Message to be sent.
    /// </summary>
    public sealed class DiscordMessageBuilder
    {
        /// <summary>
        /// Gets or Sets the Message to be sent.
        /// </summary>
        public string Content
        {
            get => this._content;
            set
            {
                if (value != null && value.Length > 2000)
                    throw new ArgumentException("Content cannot exceed 2000 characters.", nameof(value));
                this._content = value;
            }
        }
        private string _content;

        /// <summary>
        /// Gets or Sets the Embed to be sent.
        /// </summary>
        public DiscordEmbed Embed { get; set; }

        /// <summary>
        /// Gets or Sets if the message should be TTS.
        /// </summary>
        public bool IsTTS { get; set; } = false;

        /// <summary>
        /// Gets the Allowed Mentions for the message to be sent. 
        /// </summary>
        public List<IMention> Mentions { get; private set; } = null;

        /// <summary>
        /// Gets the Files to be sent in the Message.
        /// </summary>
        public IReadOnlyCollection<DiscordMessageFile> Files => this._files;

        internal List<DiscordMessageFile> _files = new();

        /// <summary>
        /// Gets the Reply Message ID.
        /// </summary>
        public ulong? ReplyId { get; private set; } = null;

        /// <summary>
        /// Gets if the Reply should mention the user.
        /// </summary>
        public bool MentionOnReply { get; private set; } = false;

        /// <summary>
        /// Gets if the Reply will error if the Reply Message Id does not reference a valid message.
        /// <para>If set to false, invalid replies are send as a regular message.</para>
        /// <para>Defaults to false.</para>
        /// </summary>
        public bool FailOnInvalidReply { get; set; }

        /// <summary>
        /// Sets the Content of the Message.
        /// </summary>
        /// <param name="content">The content to be set.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithContent(string content)
        {
            this.Content = content;
            return this;
        }

        /// <summary>
        /// Sets if the message should be TTS.
        /// </summary>
        /// <param name="isTTS">If TTS should be set.</param>
        /// <returns></returns>
        public DiscordMessageBuilder HasTTS(bool isTTS)
        {
            this.IsTTS = isTTS;
            return this;
        }

        /// <summary>
        /// Sets if the message will have an Embed.
        /// </summary>
        /// <param name="embed">The embed that should be sent.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithEmbed(DiscordEmbed embed)
        {
            this.Embed = embed;
            return this;
        }



        /// <summary>
        /// Sets if the message has allowed mentions.
        /// </summary>
        /// <param name="allowedMention">The allowed Mention that should be sent.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithAllowedMention(IMention allowedMention)
        {
            if (this.Mentions != null)
                this.Mentions.Add(allowedMention);
            else
                this.Mentions = new List<IMention> { allowedMention };

            return this;
        }

        /// <summary>
        /// Sets if the message has allowed mentions.
        /// </summary>
        /// <param name="allowedMentions">The allowed Mentions that should be sent.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithAllowedMentions(IEnumerable<IMention> allowedMentions)
        {
            if (this.Mentions != null)
                this.Mentions.AddRange(allowedMentions);
            else
                this.Mentions = allowedMentions.ToList();

            return this;
        }

        /// <summary>
        /// Sets if the message has files to be sent.
        /// </summary>
        /// <param name="fileName">The fileName that the file should be sent as.</param>
        /// <param name="stream">The Stream to the file.</param>
        /// <param name="resetStreamPosition">Tells the API Client to reset the stream position to what it was after the file is sent.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithFile(string fileName, Stream stream, bool resetStreamPosition = false)
        {
            if (this.Files.Count() >= 10)
                throw new ArgumentException("Cannot send more than 10 files with a single message.");

            if (this._files.Any(x => x.FileName == fileName))
                throw new ArgumentException("A File with that filename already exists");

            if (resetStreamPosition)
                this._files.Add(new DiscordMessageFile(fileName, stream, stream.Position));
            else
                this._files.Add(new DiscordMessageFile(fileName, stream, null));

            return this;
        }

        /// <summary>
        /// Sets if the message has files to be sent.
        /// </summary>
        /// <param name="stream">The Stream to the file.</param>
        /// <param name="resetStreamPosition">Tells the API Client to reset the stream position to what it was after the file is sent.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithFile(FileStream stream, bool resetStreamPosition = false)
        {
            if (this.Files.Count() >= 10)
                throw new ArgumentException("Cannot send more than 10 files with a single message.");

            if (this._files.Any(x => x.FileName == stream.Name))
                throw new ArgumentException("A File with that filename already exists");

            if (resetStreamPosition)
                this._files.Add(new DiscordMessageFile(stream.Name, stream, stream.Position));
            else
                this._files.Add(new DiscordMessageFile(stream.Name, stream, null));

            return this;
        }

        /// <summary>
        /// Sets if the message has files to be sent.
        /// </summary>
        /// <param name="files">The Files that should be sent.</param>
        /// <param name="resetStreamPosition">Tells the API Client to reset the stream position to what it was after the file is sent.</param>
        /// <returns></returns>
        public DiscordMessageBuilder WithFiles(Dictionary<string, Stream> files, bool resetStreamPosition = false)
        {
            if (this.Files.Count() + files.Count() >= 10)
                throw new ArgumentException("Cannot send more than 10 files with a single message.");

            foreach (var file in files)
            {
                if (this._files.Any(x => x.FileName == file.Key))
                    throw new ArgumentException("A File with that filename already exists");

                if (resetStreamPosition)
                    this._files.Add(new DiscordMessageFile(file.Key, file.Value, file.Value.Position));
                else
                    this._files.Add(new DiscordMessageFile(file.Key, file.Value, null));
            }

            return this;
        }

        /// <summary>
        /// Sets if the message is a reply
        /// </summary>
        /// <param name="messageId">The ID of the message to reply to.</param>
        /// <param name="mention">If we should mention the user in the reply.</param>
        /// <param name="failOnInvalidReply">Whether sending a reply that references an invalid message should be </param>
        /// <returns></returns>
        public DiscordMessageBuilder WithReply(ulong messageId, bool mention = false, bool failOnInvalidReply = false)
        {
            this.ReplyId = messageId;
            this.MentionOnReply = mention;
            this.FailOnInvalidReply = failOnInvalidReply;
            return this;
        }


        /// <summary>
        /// Sends the Message to a specific channel
        /// </summary>
        /// <param name="channel">The channel the message should be sent to.</param>
        /// <returns></returns>
        public Task<DiscordMessage> SendAsync(DiscordChannel channel) => channel.SendMessageAsync(this);

        /// <summary>
        /// Sends the modified message.
        /// </summary>
        /// <param name="msg">The original Message to modify.</param>
        /// <returns></returns>
        public Task<DiscordMessage> ModifyAsync(DiscordMessage msg) => msg.ModifyAsync(this);

        /// <summary>
        /// Allows for clearing the Message Builder so that it can be used again to send a new message.
        /// </summary>
        public void Clear()
        {
            this.Content = "";
            this.Embed = null;
            this.IsTTS = false;
            this.Mentions = null;
            this._files.Clear();
            this.ReplyId = null;
            this.MentionOnReply = false;
        }

        /// <summary>
        /// Does the validation before we send a the Create/Modify request.
        /// </summary>
        /// <param name="isModify">Tells the method to perform the Modify Validation or Create Validation.</param>
        internal void Validate(bool isModify = false)
        {
            if (isModify)
            {
                if (this.Files.Any())
                    throw new ArgumentException("You cannot add files when modifying a message.");

                if (this.ReplyId.HasValue)
                    throw new ArgumentException("You cannot change the ReplyID when modifying a message");
            }
            else
            {
                if (this.Files?.Count == 0 && string.IsNullOrEmpty(this.Content) && this.Embed == null)
                    throw new ArgumentException("You must specify content, an embed, or at least one file.");
            }
        }
    }
}
