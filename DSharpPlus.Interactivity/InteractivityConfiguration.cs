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
using DSharpPlus.Interactivity.Enums;

namespace DSharpPlus.Interactivity
{
    /// <summary>
    /// Configuration class for your Interactivity extension
    /// </summary>
    public sealed class InteractivityConfiguration
    {
        /// <summary>
        /// <para>Sets the default interactivity action timeout.</para>
        /// <para>Defaults to 1 minute.</para>
        /// </summary>
        public TimeSpan Timeout { internal get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// What to do after the poll ends
        /// </summary>
        public PollBehaviour PollBehaviour { internal get; set; } = PollBehaviour.DeleteEmojis;

        /// <summary>
        /// Emojis to use for pagination
        /// </summary>
        public PaginationEmojis PaginationEmojis { internal get; set; } = new PaginationEmojis();

        /// <summary>
        /// How to handle pagination. Defaults to WrapAround.
        /// </summary>
        public PaginationBehaviour PaginationBehaviour { internal get; set; } = PaginationBehaviour.WrapAround;

        /// <summary>
        /// How to handle pagination deletion. Defaults to DeleteEmojis.
        /// </summary>
        public PaginationDeletion PaginationDeletion { internal get; set; } = PaginationDeletion.DeleteEmojis;

        /// <summary>
        /// Creates a new instance of <see cref="InteractivityConfiguration"/>.
        /// </summary>
        public InteractivityConfiguration()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="InteractivityConfiguration"/>, copying the properties of another configuration.
        /// </summary>
        /// <param name="other">Configuration the properties of which are to be copied.</param>
        public InteractivityConfiguration(InteractivityConfiguration other)
        {
            this.Timeout = other.Timeout;
        }
    }
}
