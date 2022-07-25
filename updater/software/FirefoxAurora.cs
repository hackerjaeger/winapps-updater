﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "104.0b1";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/104.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "ec147a8fa363ecfc75f511873c362b1577dea7a6ee8d1de1f7ff1eff5efb89e34eefbb4c5fb9227aebed366f60d216882989150ee530bb1ddc02a895e744a04d" },
                { "af", "a51f37bd7bc0ce00fc62b106ff826f8c38fc001687ee12295a0316577cdd7f1f6e8902c3e7626b7b33aecfcf25767313b95ac06a916efc37ff7752d9820605e5" },
                { "an", "b480ae10cb0d20680c65d1fd0b2dc5d113c74588076915147a25c87524361d4ecb5dc8c6ead3e8adce9a0955a626164c4628230e0c05748a39b43542a6be56fa" },
                { "ar", "9b12be553cb225000456285f75b203c586d326aeefebdccb3307336e4a6b01794b79fb90633ee32fa26d0a39d3b069272a13dd63ca51a00ad679ded2f13d7e9f" },
                { "ast", "e1acf66c43c2c4baa31a00a0e6a36beb8a3f2a341946afcc45dada41bc74156b2305b03d239c33f5885603a3d15f85ce911d2f5aeabd927f4bd41f339037d2e7" },
                { "az", "e4e850590095b45465cab53f0118219393ca73936fa95c5cb023f95e78e4fdd12b6222583a51db9c20b5739e18c5649eef55e641f4c31677c6f44f6c2aaa47ba" },
                { "be", "a32b8dc0fdaefe12952c50abcd8fb40d582392d72f21555c0184463672e6fa94910ee794bc1d43e555cbb3affab82b2e361f30298491db98ed7a98e180f6523a" },
                { "bg", "1ea6959ca89e47f476a196601a663f791c4527b52cbba0d5de706c191569231e392d89a7ef3b0cde250485faebcd9d8e6836276f5631740f38cdd7f50b417cf1" },
                { "bn", "7156be749ca9bdc6948df563ea07b2fec3d713f7df48ec5631857f7a15f5ff4fc58072c3744341cd33f48b24a809f9f3175aff0994bd2a7e05873edd158bd9f1" },
                { "br", "c5789bf5d0a4375972c4289053ce2c7d9cfad88f931e6e0a30c926567ab3a3ec17d85d073ab6906d122e937cc43054b96d1a908f13402773c4153856e891417e" },
                { "bs", "afaecd1d6e76666934f2286868123fce3117d7eafc51847b60807fc7c094678c6c26924d90d231e1d9e5b0b08d9df5fe135cb581db25c7abc3169c90751564ba" },
                { "ca", "06bfaf14b72088b8af2fadcf2e1578c381c216b0418c83c343d8e53a5e9b19a685f1416591d989e8c867b69efab19e0986ee75af16de70b88bd33b495e0ee9c0" },
                { "cak", "8ae83cb8326b099b086df8abf19177e98ce376b8545bd2e8589b63d52596bb6e694539a8716b9c6f8386999ea8349dfd406273d0f198a5c58ac05ee1a07cfdc5" },
                { "cs", "80116570308c93a746757f724838788d23d608a938315acb63198e3c110bc574474129dab3e3cee29c1f7c1860060b06ef219edb19d06ed32a697e9d5901bf8e" },
                { "cy", "86d8c86077fb8953c087ad184ae985426d0e3bd887752a583ca5b2ec7f57fd353221a5a35781c0f12b987be6853877fc89a26d60347970100efbfe86f78000a6" },
                { "da", "d98ea2fe19a7017e5a46b4eb88c8418573899d35089d1b0a9408c6bc9ef81afc3a065999f9747e24a842573b578c9c61d8c60efbf9a699e2c62bca816cd24717" },
                { "de", "32e54cf0e9ddd0b24e610a44dd1e7021b83915e5bad639a46ff6192cc09625cab178d3657df973f38d12e2a9e4048302ec9ddb97e9c0de7f23ac4f9d2dd1fc91" },
                { "dsb", "f02cf47bcc83009b995a53ffed98efe933ba243cb15e37a156fdf785fc308fd29c065381222472fba61d8341a6dd49fd946d3379e8b0e728b1733f1cf3e088cb" },
                { "el", "7fa86324fc35fdb0c659e5fd74932f74d512173e0a2d7be0c234c3e17d31a27ab0f4c5f6904c057d9d181f44666f35c10bce3f217309946115b435e9f7c7a733" },
                { "en-CA", "ae3a12754d9e40ac9ef63014b3c3b4564d8f1cd6d171981a80989de46e1e30aef012a1f16656f8167a25b95a8c9eb99adc353fb921b9e3104f6d32be4497b498" },
                { "en-GB", "ba633d2a495a235f18d6bb112a29111d91ccdce904af0960d3dacc2bddf52aa5183734a8db44472027573f6855a1d35d69184d754fc3850b298c09b931531441" },
                { "en-US", "136843a0536c71d456b26cbf585ff623fb1e7a8bef1560e15170d5c6ed15b15663423dcf8844a018bd18d93aafa89370ea3aa7142b8e87a7c352dd8787774139" },
                { "eo", "f0636b84f4c715dd92579ac6184ddc7d71ada35a60d34107ccf72056f399c2b796c39a5266d279894744829a17b29ae823a21178c63ebc003827d00878ee3961" },
                { "es-AR", "ac3c3dd465a2da288f12cc7785a157118feb0c5634bdec74773f9360fac0a2158be299f70c3dfe44dcd8341ba6dce6d71ebc4c6deea2bfd6530a79d44284a367" },
                { "es-CL", "5af44837256acecda14c2e11967595bdabd57ec04c1ec5b8e3425f0ad810171a85bb254ab4a7331657892f5440688d6e41fdba54d238ac63406ccdf832b5f86e" },
                { "es-ES", "c71eb16411e17a54a47a408269eb9c41707061903e3ee462309680e857fb455715e35601e9215f2ff2b6366e670e46dfea0a854a2e9ab59fffd0c9fee12d28d0" },
                { "es-MX", "37d08261d20f1f8599826b5cdc6c219e4c2bdccbe74755e6cee5ec9784395162f4208686cb08be5878639dba41b8ae87e3231697db7cc9219272355bfe5051ac" },
                { "et", "28acc8f64174448e62302df5ee3bb668324d950280d64649d2bf7ab5d01a866c5a80b83152e08262fbd1e22a127efcb6fca96d15cd5772638c7bc99833c45fe8" },
                { "eu", "7c3c07e6b18eba64763e4bc75234de36e9254efc64ed23c6acb9023b1520f712c36d1521fc2160711b4c164651bd6ca0d3af6275c1b4c25d95bd829af0cde1e5" },
                { "fa", "a20d17e1dc9cb701228dd77847a49dc68b1efb54da2caacdb43376f48e7ceb466a5f3f72f1bdc4cd688b98f8f391491c1abc0c3cd556edae3cd3aaf4231be756" },
                { "ff", "aec41061dd269881c52638495deaf648e697ec99e5a012b8e6cc678929525c1562050f05531ef2c2e696821d3ecbe9166270dadfbc36d7681320636cc5308439" },
                { "fi", "2eaf41fd0d6b409e71d97c2cb8e76d6f0b4a799b50c66280d85923375f564626fc09a68ecdeb1958ac24452debe74064cf0af5c1d1bedcdeea572ce664caeb5f" },
                { "fr", "e583b8f319d0efc1dcdb04df3fa4d91955c527cbb42795dde3c9e81bf085d182a0916cc1b12eb50115b182ea9bd53132f513073bdde83c06516000004fe45a13" },
                { "fy-NL", "2b017a67cba837e0729325c1d707130791c2f82e694d3973d187068e3eaa63ab9cad54eff9a246222091e9c5e7d2e0eb1853f96957d081161b45b2c6798eba89" },
                { "ga-IE", "1222d0652cbb80ff48220f199f04a0bc6539d461dcefc683aada57e9c3067d286ce8eb76dedf5ca60345eb125628e8494e940fbe27d48d9291577a030a097660" },
                { "gd", "9d93d81d1d4fa88084f3dfbb85161b307fc63a3467d8ab136b2869a4022cb83c84aaf1bb0469c2c50fecfa98cd524dc21ed1de642fcb3df8ba5cb5561ad6e44b" },
                { "gl", "474078ad7065d0b668aa9190a7d04d09ed394ce673df91112b03aadddba86aae46e317755b5517094d4d95154a6984e9e432d6ecef18d4243af2f5b06f1f7b6e" },
                { "gn", "dd6763f931a3562fbdfdc032811edede9653e0059756119d4722be692f0f49d4000c8e5150dfc8d11d3ceb3d3858083960f760a94ae9b2a71d99d7c363dd5765" },
                { "gu-IN", "4d131a1778c916f9b9ec106a627e328f029c9462f8bdc6076d98202d3f303d7380cc3161053c2fc1aaab71b8d73b3a5ccbc9a63523c7fe099bc8d68f13668394" },
                { "he", "8c1fce2215d7b61d6a989224be852de65c806734d90937f939e96e581e758a20b6cdfa2cc3e9bfcfe835ab7fa0d909820f148fb75eb2addd2d381c139738cb64" },
                { "hi-IN", "86a76a2299b5ebd819f74356fa197d3df755537fa39d54d75421e4fca635d2a0e83993ce66cb36a425b5377707c7edc6243facdffd3591c91d8ef0626727f634" },
                { "hr", "cce5386f49b0a13084defbc673be93dfd9fcb00bdacbef46780de610205f236d9cfdfd6c83debcb66c760d7c89273b5761ddf68c7b6af6f0a5c9113c34a097df" },
                { "hsb", "8f9ec4ccea1ebeee488fa2ee1814d0a501d0115c113f3965ed3c9829d8235f22eced93467e9e7a407668909687970f3718e973a9509ed89f787393da25e248ad" },
                { "hu", "5cc825b138af38152faf8c2e55a546b2b47cb7f0038540f8d742fcf0401d3fc687dfb36f43ace58fd9e90fd496ee3b737e0183152fbc6df1056986c603ca6741" },
                { "hy-AM", "2c66f77af023c343f6e39fc11ea53d2b4d701de549392664e022a171c392a78ba6f3c2afd0c9927d5dcb1c17e7e8bb3285c5dcd0b13e7f59a4dc8037acacf630" },
                { "ia", "b498b8f660aaea4c6d96a3a85e1ed9c0e54c77d5d7ef07b9d4c36c43b246f50bd7044b4725c5f39a2c714f775dd94796e0b77160ecb9645250235e9c8eb9714d" },
                { "id", "f6233e14674b3864d0461a6431ec4a1bd5b0d5d2fddeb33feb53dd8d43339fd23ca821f9ac8e06f1589ee0b7a2fe870cc82ac538109f30ca54731a9c1e7b59dd" },
                { "is", "485ca98d779228384f435cdc6b1ecc89f9b65ff02bdf2f71e45ecb6501b7079d2a4799204e3e6133fdfe1db60296748648ddfd68d0be1218607d196cedc596d1" },
                { "it", "f26b1cc47cbcbe539a12c9e6342baa3350d846b506f1279375eb558c50ba11acca7ab994a4449e06a931d7f618cceb1e59bba6f3ba4a8bfc346bab5ae5a67c1e" },
                { "ja", "e465d3af8743518f23b6c251bfa3e12824c0a46b80c7be4b2abe8f0018891adf59d47874fbba94d238b04cccd2924c52b9f8f9aca796ce574b2e7798c5d28038" },
                { "ka", "db097c08c844ba1fb12a9cfeb8b14f7b74ae5fe9dbd105cb75a2083304e1ff3f56437c08810c6c9239a9acb4c82d3bbca1ce6d808630946b3aee89c9062a41b2" },
                { "kab", "1e5f96a6113f26ab8b0752316196970f0a9a68e54d914ab6dc899f403cc7b4182def7b524de59bbabad2e4ff56926e0ac128a34648f903c83229244ff6bdbbfc" },
                { "kk", "5a8f52d589585b0e61a7c36895f55131cae23a2a4b54662c73b271116c3cb077120f719f0941fc58cc2ede98b780eda6794fb9c9a37a9d7ea13d5a366e64f1c5" },
                { "km", "f8cccf31fbb6779ff155a6296e9a2d8dc0f14eec46d81c2641eaf07596dfd2a7313d393770dfa43a80a1356fdd4610ba3495956d837cbd1c466a6f23494d8a3d" },
                { "kn", "3b7b5211567d54d0f2d05cff346d06386148a4a3fb862c10eb50da3e9576b59814089de0327d1924217aea848c09b282a9eeb3b6a5535b42f31bf9bf9bec3b7d" },
                { "ko", "be28187f511ea197685c05ab773a18828e40dc59a59721731a0b8d1bfc839a071f29a355cf517f9290773663897fcbde7d748ebfa29451d460164c2ec91b0301" },
                { "lij", "4c916b90a0b5feeb1668e0fd3261460224ebbfb075e16cacaae580cbabb5eb7507ed4e7254cb3abddc6d2b1a2d49323c90422c19b3c67f9ee5c6e171ccbf3a15" },
                { "lt", "e91f0997d4a33e9d3a201c1b793330a04ce63056c2819af019b721c4ed3455782b1b7c9b798a761a1012dd8544c993348c65eaafd653edb4cee69d836e83ca36" },
                { "lv", "55d10146b4c84b83282bcbcb32b4ad17722b7d4e78685dd671f856e233f1894c98e49919133cc030935cb4412ae4ff2bfdce26d9db24ea5eb03b6e3fcf360f27" },
                { "mk", "401c4d7fa4d124c3456639f1306826a935fbc6b394fb011f9de9a3efc1c3a9ce769c25af87a3ae4299d145bef78772380e79b9c92e57ce0e4ec795d725e1200f" },
                { "mr", "ec1ce83181db7f518d0947c979cf7db58fcda80d179592e8d8a18480dce1663fc3ee3e7bc5ab3e1a9a648d5b583189a3de901b0e0554fd2b3c1e872c01d9b67b" },
                { "ms", "f2636f9fa445948a34ba72b5f40e651fd68f3e1ba331d0aec5bfcf556534dcfcdbe4d8955b0aab423501bf3f89a544deb14084a7051d4c9cf036612241740589" },
                { "my", "5e4bab3f80f3e2dcff2e3c79e295122c8c55d2f16fd212c8630cd30901ab8769733bbe6d4d799b56835ae540c5401082330a31428f7a246860eff51a2eccd79a" },
                { "nb-NO", "e7123d524d2c7824677646de96f3e78d2cc6ee8bfa44f9f0c998198329f68a22ced2dcbe48867b2129960b30e54c3e28202948ffea98b7b4fd61c806595ce29a" },
                { "ne-NP", "d023ea9ad12fb3e78faebc3abe64554287ccc81be24e344ddf52fb224376f36479c5fc474d1e4c7057136b8ef79b0b6eab574a0dba2b2b82fe0162ab8b969e74" },
                { "nl", "d635045e3d5ae19aeeafb6dfa9c049966267efb2870a0d402db2f53bbd59091e73d5dc19652c2b2a6a26de7539ed734e41e2d1d324888d1fae166283d0ec1a3e" },
                { "nn-NO", "6965590a03d48f295a14dd89dc7050042276f77939543c6670ad3598b9885d42f9ac31bcec8d9623369806c9e2f0a31b4676ae061421c29af19e303962e2794b" },
                { "oc", "66debc945d91ab309589dfd15a473fcc6c1ddfacf36666764e69611574b0a49cab7087fda212a04915e6ba90a02ac136418516e3fbf60aa1deef99ec6bf0dcf2" },
                { "pa-IN", "e5403ee9aff0dc92624908d9d353f4137a5be275ed7b533f45d6df447119f6af0cfa9a877716d0280e5ea32d3a939f6d071664e4f50c3cb554f06a4fb09ad204" },
                { "pl", "a8be23cbeb9bc562bc2d3cb203c3f70231c970dbf5969f336e34e549ec9a53a6f173febc9597587ff297e4f1addc245737ed28c6d1b85d04ffb663b2366edaf0" },
                { "pt-BR", "c8a8a6d082f7c0509f44850943245d4a74a1e64b886d87b9711af82f50c9c300dda2b186098352707553bd1995a28bc4507a7202f82298900a714861a74700cf" },
                { "pt-PT", "2c1e84bfb359cd4a9c3893c03f23b29adb01101dcc899ceb233d334a667cc24e57a9f1dde9534a3c067d4ee2b990eb3307b7630083869e6c4d6ce94ea85d3e9d" },
                { "rm", "f5201d1c07d09f1f8d231d0c1ac91f3c83397e593dff3e451825be6bbe0b9003f917541987ba16af47e80628a8de12a1ec316d88306e18bfd3956ee168a1f44e" },
                { "ro", "fb981dac7c8a0048040f716f3872f91ca193f70bd7f3b7384b5e7cbdb6b58f136211accd4cc9f24e4a6a19b5ee15832b76b6eeafb42b3e75f337112cc8c611ce" },
                { "ru", "e536683dadf8c599c27bada816f7e1d7964780cfa7127bc9642d004cb0f8923712929c1e2c636fa8fc89f4a971c29baee8e7b33aaee25940f8de2a52f09bf201" },
                { "sco", "59b670c2289cae0fddfa718868cf6d2c35e0a0199c4f504057fcb5bb74c35e61349481e5a525158fe1a1e10936dc5a20f29d382f9d08bea0533ab35213b491d5" },
                { "si", "157e4a60408a35ac72d38f9e4693766f69b084d8d9ceb78b266011cb8b255f026352a0771e0b4ab01b3e5d9bf14d9829f650388e7206475c91527eb1e63d6558" },
                { "sk", "f59793b879b4c81648b545c286e2c8df8b9dc7ce3fc77b68e447b4ee9e2b694da07737a110efeb1f423cafe5022dbe157a15f20f2e0bccc4506082d90ca3fa59" },
                { "sl", "f2acd1bfc7659ba4ad0b7dd288e5ba1acdc13e3914fb4c7f9207282d2bb4cf95fef1827a26f5679ee344e704fd32dae46e915c4d9a891e148faa24e3fb453fbf" },
                { "son", "266e7bacf9588742886d4a961453f037983aab6abb04fa18952a25b66ec3648b27f0da44b7d3326db3c02e3fe4022ef44bd06eb4ad4ded3b681b74642ce87f8f" },
                { "sq", "1284ab7d1724f701ba0cda2b5d191b0bd846c8f65dff09168c8acc4158863aa576e0a0d40dc7fcee1e57bf7e32bc905960d82a4a98a3c1483f949fc26f27f145" },
                { "sr", "d4a06a4b82c90b828bc9e5886599def04172c382e1192bbf6abf4e63e2244fb0ee85d3aaadd59cde9e777429d0b3e07029bda152c99ef0a43e8ba3873e6442bb" },
                { "sv-SE", "e4267dd3227a5834ad26b8dc65bd4762857f0b91b5fc3c8c225c79dcb54246a5525cb9651261f968f27e3101a9d7f86f7008522d84c0c83e355a0e4b6f208fd2" },
                { "szl", "bf1acbfd9ec5ee8d5f2d8a7fe299d6aa4ffaaf7196c95f5686735c7dbc27f5a296a40962e76ba920a94f7046abee92853b5bd306100f2fe01b4cdbecb4062263" },
                { "ta", "a10d98892de37830330f32045628a2b08c15d5fd86e24c3bac5b21cbf2723cd0507db43e4f635c29c707db482aa53e3fccc66553416248a2b119a31004033e07" },
                { "te", "0b19bb978d633067edc6fa13eb3983726f04113b0224106f1fdbb1d49c333788a699f71f5af848098b6f305b8b8ecd2cd17c337f20854ac6f246c4262e888c4a" },
                { "th", "63a4a5b7514ca99f1758f5c2bbc48d866834e5904937139b402b88f7542ead60cc5003dd33d56783399f8119da4ada450e4a4de04f27917841d27599cae4f7a6" },
                { "tl", "3d9ea18a657a0a21834da0200f9a39bf0cfccb399ced04368758642f03b176b28db2ac4c90d440d8da63242c4066875de28598d1f8b7bc8915b94cd571425b34" },
                { "tr", "cf281b3044d984341e06fa0192094f4355e8dd905087d4384489c2d18736b885a35d032cebf8e12401f3d9aabbfcce7833f9446baf40e1e40a05e31d80ebf662" },
                { "trs", "24ea2529d831efc57b7123f3248742d56b0c8b686e6a1a488176af13c5d8b7a53b9a640a155722629fc12210b7571b1dbbc9734439f058583aef9b5a8f05ea17" },
                { "uk", "d7638e026bd211cfcca65d6b1b2623ea0409da8d31ea04d24f6bc7f875c51f78385c6e42483f0bba94e208e78edf64f09681b11da0d987fed5a637f12e78a1d5" },
                { "ur", "47b92b0921875b7f6c0dc6f3a2d793821fa634c13c9257ff2a8bc7e04435c8521059526d514f177c2ca15ef5d33aa2da4cc6a01aa3836fdfe653a9bd466b28bf" },
                { "uz", "e0d398fa43496184e479ae6ba7da2a1ef07bedbfe2953c69d7ce092f0ed37d3369e9317b3db9d9e73840037ec9d766c500752e3ef7e877d84c9df3daaf9513d2" },
                { "vi", "d3d19a0b711253e46120ed21793ee24ac0846274dcae21071766b019b908a9b877d830f3861e77dfa91d10b7f5258816c89348acdda8a983cea490d988fdbe1d" },
                { "xh", "a0afffaccefb7b01774bc048c885a2782f57087913a5c7917752927d12941c5748fb63b806191993e347f00765881c0f760acc5c0c439523b75586503fafc06e" },
                { "zh-CN", "0d38d6be4f3133e5dcf5b5adfc442e92bf999ed5577429d11b1460be3de0a1435b99d9b556fcd4dbec8335d9895db7ea35f34a6874dfbd721699317f745d7214" },
                { "zh-TW", "be208d291134a738be03fb01f69cbcf419e55c6dfe1d53a85a241eff8456104f4ed7f39adb0c2ff30bc4488784b3996cf974ed8cf88b4fbd8b33689f7e2e12d5" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/104.0b1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "73e873b48798f1fd3f7fea8b58ccfb85ff613895d26a1b3bbe1f6d541976cb547a1c2708d1bb1adde3e10a1554b5a8ebaf681c072c5c204d36ca8d5a09b1115a" },
                { "af", "12a5d6efafcfd6ab4aafcc071b71262ba5508cbcca126ea470c1c64e1056ff203e51379ad1cef33e2b3bf559ac78c4bb68eb72eb3b91bffd5fe1bde1d581d93f" },
                { "an", "c925440ad540c27eeadd3caccdf3a9acb7e2e1ba644f7716b424efcafb1a9f72f2642f97f3037449d6bf8fca101d259c7050dbc61b24bd0688ddf0e80f36712e" },
                { "ar", "7ec582212a037280cc580329fa7d863106bbe625d7576d5d11f060aaac67c19339b6b6c39566b1be3932a73741be94f5099e27c15117d70c9587ac91166d928d" },
                { "ast", "ea4f586f2af8fac61f0d3b0a8805a56d4746945d3d17b9f67d3594e4572a6ef1f05ba8681b5543b5627f93524bef317c47932f11d895a70535137786581c8c9e" },
                { "az", "a27d940d64dfa3f5cf8a73275a72e970e972a85b4a4156e0ed5c5fc94fa525f825bb10b57d098614bdc2f2c4b0e2bb56ce1961b103778595f30c2450ca14eca5" },
                { "be", "9faff612ad3e70e574223d3249533ecbacc55a52e3d8a13bbd3286af63f99292571f5f97ac488136f010c02b0df01223fa82395cda14381aa0e3692fa8e3bb24" },
                { "bg", "f704277337fe20e7f7396e4617f4504fc80e985907a68b20fc649297fbd4b07b9339a62584c4168e12fa09033fd07f6208d5af14b2f48ab6e24ad15f7a74f385" },
                { "bn", "40b836ad44599eed3489fd0f7ff4a302fae357c1726d2d8f7ddbfdc2a651846d3f39e130016efd64ef56b67ce7dbfe5fa0348b46d47f5d928d7d3f157faa4907" },
                { "br", "75c67186fb810dc2d0065b8e6f17bb19d4467284d92cbec3f33a5cc6ae330c08bef977dcc983ea874242eecd2ea2bbe629278bea54ca19a03ce72929fb8f641c" },
                { "bs", "63ac1e14f62f3b8f5471418d634d28a267d2629be0c4ec7443376323d4ddffa94f3d1f06fe87b198c23f7547fb3cd1e4414ac261a65667298aeb753c51a4ab9f" },
                { "ca", "fe2a16bd7e0bb25289147be151ea5653e245f567a0a5fdb967854af17b2fdef460dd42c0f3fc0805a14ffd2b5deebed86a7baffe385404cbc5418e005f8f41bc" },
                { "cak", "ffdc65125cfb22044fe6fd545b52a93d96175607e90fb02c2d9c110b613831d02b833783c29830e851a4c1ea841dc7f332fa04bf39e10e1fed6e28de14ee2563" },
                { "cs", "60f8d0623a88ec041d47f563e7887940926d3925a69ed25d738d86fe797f29ecbf107de91241a81dd1956461246701fadb5c0be754e8af16348b9ba5a4fd62cf" },
                { "cy", "9451b697c552a82530173163e1f9316326fe6db136d300fe5848fb26a82c746d2cf1f9bd2a40483a85b9464525b93edba946587e01628fcad6d54e1a402d888d" },
                { "da", "900af98e54afe04e48ba51baf7841abd5035543b607c450d9b4758916363f0c49b270c70d1949a19cb9e3a987c1b2928d925cb605b638af707282f201c5cf536" },
                { "de", "fb86c5afd0528bfb02afd94569bd6c266539a3b1b07ee7a73a6eac1c3258d08397b05c1779a4f32840ddf30e853aeb5b4b17531ca9b68dab448cee8b0e48bdcc" },
                { "dsb", "b782f60d3851fb67b226a62d3383cab0bf46a273cfa1d0f781d11460e57d5bee907562b44cab7c5dbddaee2dc869ea46e709381d712838f27111d3a08fe57d0c" },
                { "el", "8fc76b8077aaeb57d575766743979526bf90a905597db100b96805de537a14dc22086cbaba169018dfc48b6bbf59fb24b1c5e757a6a48353e9228215e2d51ed3" },
                { "en-CA", "3c7d7ae9d5fae6d54070dc232fb20ae34b347d810377bb0e31bba0d70a0a6d2b83817254d69eda66a1838a531ba95a77772a880b776361741a235ea5a4624e45" },
                { "en-GB", "678142fe7d8527fa453039bf45a9a01028f070fbe7c1016913f526a111233de662942ec9e3c6270ea861a04d477c30bc9fcdbd0f61cd9ba21f88e6da834c8503" },
                { "en-US", "3a7a829668a13ae61bbc73a4a3b16c123c7d1dadc482f2a93f358107175471573c22d4ea410b4d1279a270c7da7c29ac067ef9a18157edeab691e33373b55b70" },
                { "eo", "dfba7d8674e06a1fb61ad778ea85af330461c20f2bd032ebb4cb3c4cacd117f8d92acd06bec50b776032a79813e052e15449c6bd516214a5488c6220fc6584de" },
                { "es-AR", "4e9acf48d624ad9c0395f6f96fdd22cba7929283fc7503b2b37f5a518fed9c246ebfaf311d315e9a213f1724a77db0e7b6f21871b796f2b188170b750a841e75" },
                { "es-CL", "08fe4e6852548f95732cb61a16e8bc9aef67e0528229fc8e27b11f790086ce243ae6e72d51bb8c12e8d49e22587181a0e161cb2765f7dd7cfbba49fe9f9035b6" },
                { "es-ES", "d268b5477857cbd7de64535aa783f44f5eb5b78b31cf2d56a3d78abf61e956288d8bb1330e651a6cc9413632384980a7b78dd35158d14057aedae1afe2249114" },
                { "es-MX", "ba6f8d4797b98226123b0ed3a865c5b76aecdc75bc6ba3d60667eb17e2a7f04142fcea4bf6850ef1b61c09a9a8047f70158100e4df390ff90b4deeaa677b7fff" },
                { "et", "891eb99c7b0576f26ff20b96cd2bcddec8bcb8e190533f6376f7660b4a19884e243ee275795ed68180844932e46daca7d34f5a12f231a7ec4fa869e315f945e6" },
                { "eu", "a8826dd1077703f96b1b1c015aafa0160d7bc3ef5a51e54b3ae8187633e09f26cdad15f1d41965106f9dcabc925a194f805bdd33a77b08e0ae2a070d9b476ea6" },
                { "fa", "87f847ab92cbad980a7d4b8ff0aad031a10f7fef9e4fa657891ddaa1c9337f7a5eedcb45483e08de53d6cac1f98cf90637a5496e42c961c11df0546cc9eb5361" },
                { "ff", "1633d23e3282c685032226f1cbcca725242fad6b0954c4bcdb89744bf4d5eea93c0dfc2265e29c79ec7a04e10651dfdc728d422997bb2ca1fd9aa37ad9895686" },
                { "fi", "ce40436b8c534c0736bccde0dcf6bf5ff5b411f642715791a08ccaeb690ba4428e1e5695f735caadc36a3a3eb73c0a195366893ce7e33304ca48dbec72a335ed" },
                { "fr", "d9cc98dcb4ed7de66e9bf96c590e2fecaa02c7ea44c8ef97f6b880dde6b2ab8498f8d499154f700cb525a716d0022110623a32b8e06cc12e0a1c9615ccc3eaae" },
                { "fy-NL", "c8f83227a62672358241520611a1d599cb6eb02e235473e3cc5b4c8d95852bef6db8e5c2025d812fb085610896c626db4cc1c351bc9cee654afc4ba5b82a2429" },
                { "ga-IE", "d086ae4068982f35e099b54d5c4fcf70dbb335bd7e183ba39a021be0375d889e27f130ea27d7d131a6e053cd62e0b51b38a19a1314489d6cb7bbbb8f689b7dc7" },
                { "gd", "59e7c6c86f5516a5a9da9d5c8153be25e2fafff7f006fcc1f1065d2284956d1338ea627d203d2987e6d28f06639c742a2bd4e260db7c6cdc7a885d871a97df56" },
                { "gl", "21b7ae4adda43e52512b7abd5226cee47c48d726ddb09c0b8b6db6f7aff5a4bc7fb362ad2ef4e048010cfa2a03569683e4d28ba3490fc20f34d4a743efceac89" },
                { "gn", "d4a2c01457d40a50bf5fc853e65a8d80e57a4826762e26ea4a610070bbd519284ac502cb17a83ae2f602b4ab4beeb15117fd24e3f87a90c82989a704a4e2dba3" },
                { "gu-IN", "06297a93b943e2a3c26378f837005c868f4c4df5e546e2ea293639c0d0b023733ad0cf6a366a52f7114c93f361252e7817685fe09ff65b3bc06c73fc80b1ba60" },
                { "he", "818684da8ee1cd15326deb5883e1ecb51a2950c5bfa6da2492040108a120ab6dc7bc7f2926f7e2ff6cddaf0d57f7dba8e8ccd6f9a0efb4d88ebf7df33ab56b45" },
                { "hi-IN", "f3d0fc9d4fe754bcc463d319ee2098d603c6eda551eb2bc65a1ff41d922dd5772cb11763552dc13037ab8bcd6fddec835b716c0d7f5ea398a90cf21d5b1df0c4" },
                { "hr", "af34e14e664f15bc57d7205e403a5cad307342397059ab0ab7ae2672cce7240cdf6b0c1a7e057f0a53fd1e93efd86ab04d2a953c788c02df6ae344e1c6b4eab6" },
                { "hsb", "bbb4cc0bbabf969ab2851b0b3db43c9b544eb70ff19a6bbccd713ecb9226058ecc37a475e7ed6fd062989d2aa1c86162800dc49211dd7a9271165d08cc9cadf5" },
                { "hu", "d9437ad600863ff19bb9ef870bcfc1d0bcdb88ad749af50782868024e37d217150586d3e73f420233213fb170060ec7db842b26f783e35d8d5fff41b95d8a83f" },
                { "hy-AM", "93097b7b85f1eb6ce85ec34053fa0b7ea5d7d8b76f7d55b7632918762be295bc3edd6f63ac53c7847291f2ee5fbed92a7601c25e0f2b6d30e4102307a3fceb50" },
                { "ia", "e3d4dc09a4c59020c0a2f6503dbf21ddada9908fd45ac3949af852126d060e2727c1123842720430bb6fa2c334c20708f3cbe9397149cdbfce719e0980cf4a20" },
                { "id", "310661f7c2c600ac1667be940595859f9a8ca17460afcafcbf04afc88804f6ad66a99d7648f52424c305e9c75067c8b103e16784b85107f5f58e61398897ae71" },
                { "is", "9eda32a83b166a7efcefcf71225d62b53425138b9d28f3247ff3ba91b95916c2408b6df7e68cef6b761e899250215decac8a3298b804a5ac45108b80e360a521" },
                { "it", "28342b964ecd37c973dced5e26d02e4066c781a766ffc35048c016292ab24b7249d9d3fe157822af47170a422ddcbfbda50877079edb534735c44c3c7581d1ed" },
                { "ja", "38100a87634b4ee30e6064e6707d05f1cf92a981e0ece281748d3ab6df4127b1ed2a8abe946fdec69c691677f1d89c688e578ef2093359a36ff2b47b72ec8d18" },
                { "ka", "34b7b21f0f1df03f683ec65c65fae004d87a1f3b56ace9327a4a93b00987e655dc33b5c5177d3cda52fc86ad793469afe0dc39c0fc8c94359b4676a2c680fbc6" },
                { "kab", "f2ae1d015eedcdd521a681644cdc7a61e9f763cce843c8ffe6fcfbffbbdc58f9f83c2aa87aa4b71bd4c2db15841f388924322ed93a863565aa1f9e04aca73d71" },
                { "kk", "34008f40bdfa08fc835684e4e3c371ca48efa63179c5089b1a495ebde0cb2148b48ea6bf000cff32963b5b019676fa6f1265d7b4af39b470a2bcc2219ce432ae" },
                { "km", "d93b4eedaafb80c5a04e5a9db09c108eaebe274ed172074fd028a56f973b2c5948a1872ee5304ca6eec1ef6c27eef5ab26dd40ef6c3c1fcd91cf768f7abcd400" },
                { "kn", "a1714cb84fa2ab88b104c0a0c10eb2982edf6e7ce6d8aeec84ca1ffa381e2259d63209d281ba721d7a9e8494aaab0e2e866c6ec3aab4a65bd177164b0a9b6215" },
                { "ko", "b2c9838c75f721986f71e7387eb92918d46216a0b1176a4632c757afbf6a23ec003b63e340768026c95cdbeb4cc0535d298216f7f3284492b876b7e73dddc80d" },
                { "lij", "8eaf293f9fd99e5dc423f0831809261fe0b9e463ec8a8b297e4c2ad171e6cc28347fffa95cbed2f34f1b4350337802f8865e2f19214716504b4937110d0eb908" },
                { "lt", "7d6ebc505c12365d3ca1d2650e8185b3bdd191dfc3618c92458a8395905f591bc97e1a773d8dd742c0756306508eed1380cfebdc3319363f4a77a57f85af49a4" },
                { "lv", "7480fc2f89c4640bc8a7939a2665d8f0650dc9a001437864eb347eab3b1b4f7841881115250e5aa18458b7a227bd2ac17dfe3a0c47686498e7820a40117416a2" },
                { "mk", "e2729d113f535b0e51db7804b9d6cd8c484d4a9594eddbf7abac2927a58d19cdddbc7030acd895d454dc515c29d6ff92ec21c9955fd110b68a2c94acec1a42cb" },
                { "mr", "7e65c1f20f32afb6a13dd32f7af295fbf7716e97a8cea4a608d3165da54ecde44a478f5d1d80c9f4da7638fdb4820e5ce6f97b70658610316879e0af6938b950" },
                { "ms", "158f5b759545427cdc66e5bc1d6eca889c4c75d7fb53771e682b9cb1408d07975411bf921953618f4d29713fc8d10a6d64a00144ebac7584acf6d50bc333b46d" },
                { "my", "13d33921d2f5096f41e363078f9ecc7fe230bfe778099e2a824d7867215d223580939f17d72be66953039af45ebb77269f86a71acdbab42159f12cf4051a66ca" },
                { "nb-NO", "2dc86c4617efb793152e10eb10ee2303ae2c65b8871f11e61b4e0bb8ba1c08b1b4c2d415bca0f4f9e69d27604852e02c1de2e0ef8cee157dce0b53ab47b63ecb" },
                { "ne-NP", "e020ef153c33076d63df38f3459bf84b8066159ca6249ed7bb5ecd65adf3a2886a2493f4621c744bdbd3dc889e32dcd786cce400b83443919039a93bfee428cb" },
                { "nl", "70ac5509b02b839c2d18acd10ffee5d7d4d6d48e3bda5f59246f6257b00dfc5a1ed1d447dafc2cf448e479523b7d8b16a182ca73d4ba20101f0976d744bd1622" },
                { "nn-NO", "e06b75dfa149de80f0f16a791972bcf4e0cbea73431e84a649e30a21fbc08807d5e8fb1552e5868175f186d34ad1ee77414e56cd56ea3be621aa6a62827f6dda" },
                { "oc", "d3cd6e93d3ed659ba975ea274bdbcb7da828d5f86336a8339d6ff220707f67764d49c6370f116f3dab2459c7d9e31cd8e65c348049e9e3f48089fb63750c95e3" },
                { "pa-IN", "34d558fd8287a593a9478a463dcf73cd5771ad5a90de4eb01b19fad85ac50ecbdab870d050b3fb37042780286029bc607825ad55e81622b25ea0fb27f3797a8e" },
                { "pl", "8dc298b8e662b4179119d50457880095c69a992db37a0fcc78e75e12f6d963b00d7cf85b97111c3702a94c43a9a912ad59e7f713c0ba41a0db464cd70070c000" },
                { "pt-BR", "2a28c9b476a1944ca945be53085d674ebac5f736784e8148b107cf0055900b0ad19702a837a6aa81f9d3068782932a741c9a881ffa5efc11a55bc770d17134ce" },
                { "pt-PT", "f6c6a8f5df410896f43eeb72ebef0175148fe6b711665d1d9f95dc95376f4c7573abd78b6b377fa10d88b009d9138fe8bb26c573a4e0c672552444a58d8338cf" },
                { "rm", "e2426851a5fe8e2c4c6b3c878993e9d6ca9265e432bb71fca1f62d097fbc4892aa63e36e6405e1abf2365756cec00d629ef9489cb0f1937b807089c461ecf53c" },
                { "ro", "21fb429a876b01fffccf8e8729a8ccd2111bb93164bcd7f054e8b0ce5efeb118553ffb369166110b7e4734101df37bac67b77e47bd436f2b8ac4dc991e794a0e" },
                { "ru", "c3a1279581a21f6e8919f8272f0afbef2f265fd7bb0e808db4c49a03c4a71994df51239b0d6471443c0fcf0ca900f3ea8d272118ed6be9950647411497f2b167" },
                { "sco", "2e61af33bd37124fb963824894abb4911f4564c09d859cd9510d3fde32c525f4999e0c71d101ddde69297f3be2136297da42045e7073b487358e604f877319c5" },
                { "si", "d8a0cb0fb1db66464d5082e96f465e67138a1aff7c8905fc4018d93ec83a23e4144f01959ae11961d7669d2e035101cbca7c3b270779fe71d4eef5b133d779d6" },
                { "sk", "ff415748f21e15d7e85f76dc36a8c572d64a5897f1d16af88d8d3d95c8270e37b835dd492c38d9b851dd49ef05e0e89526e62fd1bd9e0354f5625323bd139bf1" },
                { "sl", "587ac2511f012e66594c0b93c48c686be02ee9d8910e97555d0c6c81ee930042813a2da832adfa1fcbf467be6a7d79f1b52142bd2359d2c5a58930e8928bbe53" },
                { "son", "b9beb784559bec21ee850397ab36431b0e74e42df6c20ba87b36653961b3367e569072211313a50a4238da1ca5b332daaac4f614a4f480581762bd9ff33dd95a" },
                { "sq", "13103041eb65b779f3db860ef7be438468b32f92d9b94031a07112a45ab1dc106b76df6b781155c4046ab4ee4231e8b0ec39f5bdc6a2fe884bd58391a2762d81" },
                { "sr", "1fc142c3fb85a9adeeb14fb2109f453f78b74b58671fd1079a2bb803b3c02c5c5c33323b50d75ae8098d3a548d9557e15029ff3aeaf27f74163a703c9f5f7b1a" },
                { "sv-SE", "5389e74e9dffb42629bab4fd0184e9e47da9db3f4257d540ccfeb1a0ca8808f49f1f1842d376b9a81f48997c20be2878e32e7987fa2c28a4b90c143739c09c92" },
                { "szl", "64b343d995f05ad97e393bafdb54cfdcaa7d5d47a520aebf6fc551cc19443237270134f5303872e2243e8d2bba1f62d07e7d526e9565607f8277b3e1ef6623f0" },
                { "ta", "57b7e9be5bd8347dd33515b3dcccfee0231ea4b53118833106c346b23250f90840688307dcd877b8981315b77521f94d6ef77e386a3880042927b7a769ce2f84" },
                { "te", "f3110b2e4a0a75308a5847911adf68a80d598a942605216de1441401cae13cb2ea842a8b3e16ae86ebc8ce471fc3cac33ff53a7e09b934ac9c5bfcdf789b9bed" },
                { "th", "72772c153632b6bd4b1378b06e10739dc5f273685280f65abcc6ad7f5617c9e5e627e90b4d761f87475a79970e13ecd35bb3031dc54ace0259b3197fc8921e86" },
                { "tl", "3ac17dee6237a37990b7144981d574ef9a5fbdcb013efe749b7982443e552b59fba3b50375a64e3e3d9e230d70e6ba50f60a247af5da89087f52a36bddad4569" },
                { "tr", "d7c96613b982a2aaec46d79aaab1973df37ab714d30812b0dcefa8a829db0f27e658a9cc6b7ca9c70db8e1bcf83ade9ce5ef6324119c2bb18bb0fffd706765cb" },
                { "trs", "9daed33051a709c7efb0b2144732e9c297bd983b2ac28a4963573febc5e674a1ac669834be4f7eeedb1c6f5fc621e6ac8d249d1251f4507e31b17946844b238d" },
                { "uk", "46a96d667237782c2652d1ae6b39d5df0f9239367cf922bce35ae6f89250ad2faad003c43e3a745ccbe9871f718cf6e8900dc5c9342c809d6b4a198fb512e5c8" },
                { "ur", "cf14484b2a0355678f629fc4ec3f43baa601adf8bfbfbf7f8280caf6f1ed3b3f8fe655ecb4f153c84e935e80cdea8dfd70faf2346a87eff14f517d4790a90c6b" },
                { "uz", "9bbf05f9b0223448660a55447694b01c430aaed6c9da77ef6fb1afbea1d4b2b6e9ec8ac27721501468bff07dfa1d6ce9cd6742350dafc4cdbb707796ab517ce2" },
                { "vi", "09361c196044ef7f77ccbcd48b7546b798cd290d98cf4bf6b7a6be4714c9dc165c4e793a870786ff7d0727efea738a4d9a986e9532ede42f467e132ed53d7bce" },
                { "xh", "a2803b7ebba50aa9045130ae897a12512ce504935a78a646fafe118be8330607d624d6345e227666c64b22b8d79c423c4a39991a79e1d10dc013dbaf1df306f6" },
                { "zh-CN", "3cf7e9057997596e14c001206f155bf50ea332a29e095d8de481c3e7a1f4423ab628d9d61248514f6e5e922d744e3a43fbae065ef92af2b738f2086304f5484b" },
                { "zh-TW", "d550b7c77a49d3ede94efd87893ed5db37b5dc21bd5c907d889d8d06b11d0c93858bd4672bb1dc4055f7e6134ad5cf5af5386d995b05e0333a7549f587cec4a7" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
