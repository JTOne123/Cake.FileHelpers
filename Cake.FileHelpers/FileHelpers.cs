﻿using System;
using Cake.Core.Annotations;
using Cake.Core;
using Cake.Core.IO;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Cake.FileHelpers
{
    [CakeAliasCategory("File Helpers")]
    public static class FileHelperAliases
    {
        /// <summary>
        /// Reads all text from a file
        /// </summary>
        /// <returns>The file's text.</returns>
        /// <param name="context">The context.</param>
        /// <param name="file">The file to read.</param>
        [CakeMethodAlias]
        public static string FileReadText (this ICakeContext context, FilePath file)
        {
            var filename = file.MakeAbsolute (context.Environment).FullPath;

            return File.ReadAllText (filename);
        }

        /// <summary>
        /// Reads all lines from a file
        /// </summary>
        /// <returns>The file's text lines.</returns>
        /// <param name="context">The context.</param>
        /// <param name="file">The file to read.</param>
        [CakeMethodAlias]
        public static string[] FileReadLines (this ICakeContext context, FilePath file)
        {
            var filename = file.MakeAbsolute (context.Environment).FullPath;

            return File.ReadAllLines (filename);
        }

        /// <summary>
        /// Writes all text to a file
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file to write to.</param>
        /// <param name="text">The text to write.</param>
        [CakeMethodAlias]
        public static void FileWriteText (this ICakeContext context, FilePath file, string text)
        {
            var filename = file.MakeAbsolute (context.Environment).FullPath;

            File.WriteAllText (filename, text);
        }

        /// <summary>
        /// Writes all text lines to a file
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file to write to.</param>
        /// <param name="lines">The text lines to write.</param>
        [CakeMethodAlias]
        public static void FileWriteLines (this ICakeContext context, FilePath file, string[] lines)
        {
            var filename = file.MakeAbsolute (context.Environment).FullPath;

            File.WriteAllLines (filename, lines);
        }

        /// <summary>
        /// Appends all text to a file
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file to append text to.</param>
        /// <param name="text">The text to append.</param>
        [CakeMethodAlias]
        public static void FileAppendText (this ICakeContext context, FilePath file, string text)
        {
            var filename = file.MakeAbsolute (context.Environment).FullPath;

            File.AppendAllText (filename, text);
        }

        /// <summary>
        /// Appends all text lines to a file
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="file">The file to append text to.</param>
        /// <param name="lines">The text lines to append.</param>
        [CakeMethodAlias]
        public static void FileAppendLines (this ICakeContext context, FilePath file, string[] lines)
        {
            var filename = file.MakeAbsolute (context.Environment).FullPath;

            File.AppendAllLines (filename, lines);
        }

        /// <summary>
        /// Replaces the text in files matched by the given globber pattern
        /// </summary>
        /// <returns>The files that had text replaced in them.</returns>
        /// <param name="context">The context.</param>
        /// <param name="globberPattern">The globber pattern to match files to replace text in.</param>
        /// <param name="findText">The text to find.</param>
        /// <param name="replaceText">The replacement text.</param>
        [CakeMethodAlias]
        public static FilePath[] ReplaceTextInFiles (this ICakeContext context, string globberPattern, string findText, string replaceText)
        {
            var files = context.Globber.GetFiles (globberPattern);

            var results = new ConcurrentBag<FilePath> ();

            Parallel.ForEach (files, f => {
                var contents = FileReadText (context, f);

                if (contents.Contains (findText)) {
                    contents = contents.Replace (findText, replaceText);
                    FileWriteText (context, f, contents);

                    results.Add (f);
                }
            });

            return results.ToArray ();
        }

        /// <summary>
        /// Replaces the regex pattern in files matched by the given globber pattern.
        /// </summary>
        /// <returns>The files that had text replaced in them.</returns>
        /// <param name="context">The context.</param>
        /// <param name="globberPattern">The globber pattern to match files to replace text in.</param>
        /// <param name="rxFindPattern">The regular expression to find.</param>
        /// <param name="replaceText">The replacement text.</param>
        [CakeMethodAlias]
        public static FilePath[] ReplaceRegexInFiles (this ICakeContext context, string globberPattern, string rxFindPattern, string replaceText)
        {
            return ReplaceRegexInFiles (context, globberPattern, rxFindPattern, replaceText, RegexOptions.None);
        }

        /// <summary>
        /// Replaces the regex pattern in files matched by the given globber pattern.
        /// </summary>
        /// <returns>The files that had text replaced in them.</returns>
        /// <param name="context">The context.</param>
        /// <param name="globberPattern">The globber pattern to match files to replace text in.</param>
        /// <param name="rxFindPattern">The regular expression to find.</param>
        /// <param name="replaceText">The replacement text.</param>
        /// <param name="rxOptions">The regular expression options to use.</param>
        [CakeMethodAlias]
        public static FilePath[] ReplaceRegexInFiles (this ICakeContext context, string globberPattern, string rxFindPattern, string replaceText, RegexOptions rxOptions)
        {
            var rx = new Regex (rxFindPattern, rxOptions);
            var files = context.Globber.GetFiles (globberPattern);

            var results = new ConcurrentBag<FilePath> ();

            Parallel.ForEach (files, f => {
                var contents = FileReadText (context, f);
                if (rx.IsMatch (contents)) {
                    contents = rx.Replace (contents, replaceText);
                    FileWriteText (context, f, contents);
                    results.Add (f);
                }
            });

            return results.ToArray ();
        }

        /// <summary>
        /// Finds files with regular expression pattern in files matching the given globber pattern.
        /// </summary>
        /// <returns>The files which match the regular expression and globber pattern.</returns>
        /// <param name="context">The context.</param>
        /// <param name="globberPattern">The globber pattern to match files to replace text in.</param>
        /// <param name="rxFindPattern">The regular expression to find.</param>
        [CakeMethodAlias]
        public static FilePath[] FindRegexInFiles (this ICakeContext context, string globberPattern, string rxFindPattern)
        {
            return FindRegexInFiles (context, globberPattern, rxFindPattern, RegexOptions.None);
        }

        /// <summary>
        /// Finds files with regular expression pattern in files matching the given globber pattern.
        /// </summary>
        /// <returns>The files which match the regular expression and globber pattern.</returns>
        /// <param name="context">The context.</param>
        /// <param name="globberPattern">The globber pattern to match files to replace text in.</param>
        /// <param name="rxFindPattern">The regular expression to find.</param>
        /// <param name="rxOptions">The regular expression options to use.</param>
        [CakeMethodAlias]
        public static FilePath[] FindRegexInFiles (this ICakeContext context, string globberPattern, string rxFindPattern, RegexOptions rxOptions)
        {
            var rx = new Regex (rxFindPattern, rxOptions);
            var files = context.Globber.GetFiles (globberPattern);

            var results = new ConcurrentBag<FilePath> ();

            Parallel.ForEach (files, f => {
                var contents = FileReadText (context, f);
                if (rx.IsMatch (contents)) 
                    results.Add (f);                
            });

            return results.ToArray ();
        }

        /// <summary>
        /// Finds files with the given text in files matching the given globber pattern.
        /// </summary>
        /// <returns>The files which match the regular expression and globber pattern.</returns>
        /// <param name="context">The context.</param>
        /// <param name="globberPattern">The globber pattern to match files to replace text in.</param>
        /// <param name="findPattern">The regular expression to find.</param>
        [CakeMethodAlias]
        public static FilePath[] FindTextInFiles (this ICakeContext context, string globberPattern, string findPattern)
        {
            var files = context.Globber.GetFiles (globberPattern);

            var results = new ConcurrentBag<FilePath> ();

            Parallel.ForEach (files, f => {
                var contents = FileReadText (context, f);
                if (contents.Contains (findPattern)) 
                    results.Add (f);                
            });

            return results.ToArray ();
        }
    }
}

