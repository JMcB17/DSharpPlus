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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;

namespace DSharpPlus.Interactivity.Extensions
{
    /// <summary>
    /// Interactivity extension methods for <see cref="DiscordMessage"/>.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// Waits for the next message that has the same author and channel as this message.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in <see cref="InteractivityConfiguration.Timeout"/></param>
        public static Task<InteractivityResult<DiscordMessage>> GetNextMessageAsync(this DiscordMessage message, TimeSpan? timeoutOverride = null)
            => message.Channel.GetNextMessageAsync(message.Author, timeoutOverride);

        /// <summary>
        /// Waits for the next message with the same author and channel as this message, which also satisfies a predicate.
        /// </summary>
        /// <param name="message">Original message.</param>
        /// <param name="predicate">A predicate that should return <see langword="true"/> if a message matches.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in <see cref="InteractivityConfiguration.Timeout"/></param>
        /// <returns></returns>
        public static Task<InteractivityResult<DiscordMessage>> GetNextMessageAsync(this DiscordMessage message, Func<DiscordMessage, bool> predicate, TimeSpan? timeoutOverride = null)
            => message.Channel.GetNextMessageAsync(msg => msg.Author.Id == message.Author.Id && message.ChannelId == msg.ChannelId && predicate(msg), timeoutOverride);

        /// <summary>
        /// Waits for a reaction on this message from a specific user.
        /// </summary>
        /// <param name="message">Target message.</param>
        /// <param name="user">The target user.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in <see cref="InteractivityConfiguration.Timeout"/></param>
        /// <exception cref="InvalidOperationException">Thrown if interactivity is not enabled for the client associated with the message.</exception>
        public static Task<InteractivityResult<MessageReactionAddEventArgs>> WaitForReactionAsync(this DiscordMessage message, DiscordUser user, TimeSpan? timeoutOverride = null)
            => GetInteractivity(message).WaitForReactionAsync(message, user, timeoutOverride);

        /// <summary>
        /// Waits for a specific reaction on this message from the specified user.
        /// </summary>
        /// <param name="message">Target message.</param>
        /// <param name="user">The target user.</param>
        /// <param name="emoji">The target emoji.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in <see cref="InteractivityConfiguration.Timeout"/></param>
        /// <exception cref="InvalidOperationException">Thrown if interactivity is not enabled for the client associated with the message.</exception>
        public static Task<InteractivityResult<MessageReactionAddEventArgs>> WaitForReactionAsync(this DiscordMessage message, DiscordUser user, DiscordEmoji emoji, TimeSpan? timeoutOverride = null)
            => GetInteractivity(message).WaitForReactionAsync(e => e.Emoji == emoji, message, user, timeoutOverride);

        /// <summary>
        /// Collects all reactions on this message within the timeout duration.
        /// </summary>
        /// <param name="message">The message to collect reactions from.</param>
        /// <param name="timeoutOverride">Overrides the timeout set in <see cref="InteractivityConfiguration.Timeout"/></param>
        /// <exception cref="InvalidOperationException">Thrown if interactivity is not enabled for the client associated with the message.</exception>
        public static Task<ReadOnlyCollection<Reaction>> CollectReactionsAsync(this DiscordMessage message, TimeSpan? timeoutOverride = null)
            => GetInteractivity(message).CollectReactionsAsync(message, timeoutOverride);


        /// <summary>
        /// Begins a poll using this message.
        /// </summary>
        /// <param name="message">Target message.</param>
        /// <param name="emojis">Options for this poll.</param>
        /// <param name="behaviorOverride">Overrides the action set in <see cref="InteractivityConfiguration.PaginationBehaviour"/></param>
        /// <param name="timeoutOverride">Overrides the timeout set in <see cref="InteractivityConfiguration.Timeout"/></param>
        /// <exception cref="InvalidOperationException">Thrown if interactivity is not enabled for the client associated with the message.</exception>
        public static Task<ReadOnlyCollection<PollEmoji>> DoPollAsync(this DiscordMessage message, IEnumerable<DiscordEmoji> emojis, PollBehaviour? behaviorOverride = null, TimeSpan? timeoutOverride = null)
            => GetInteractivity(message).DoPollAsync(message, emojis, behaviorOverride, timeoutOverride);

        /// <summary>
        /// Retrieves an interactivity instance from a message instance.
        /// </summary>
        private static InteractivityExtension GetInteractivity(DiscordMessage message)
        {
            var client = (DiscordClient)message.Discord;
            var interactivity = client.GetInteractivity();

            return interactivity == null
                ? throw new InvalidOperationException($"Interactivity is not enabled for this {(client._isShard ? "shard" : "client")}.")
                : interactivity;
        }
    }
}
