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
        private const string currentVersion = "105.0b7";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "6ed504ed63f93a83fdc4709d3f9875042304cbfcdfc02a6b1cdb0b3ed97fbe6a6ac79ae31d42e11d9a4f069977aab5b76bf67c0ec6646060178249e08ce1031e" },
                { "af", "10a73192244f90b70e5d5cbcc8a8ce37175f260b5bda79e3884ea549621c46bd89104c94da0b3e89f748455c8f524bcfaaaa913efe40f221f65d7ea49c4b8286" },
                { "an", "3ff48a2b9b02d106396753489d199caaaa699ca700972368c218a0940c9430f02a15397f10192dd765ede5663ebe35b9525e1f53f076dc5479c7f073e5cf68f9" },
                { "ar", "3b8f63a480ccaca57ec2547d9dc2ae9e254bf3a1545c2cf78728aa906c310d0ed11a53ffd6272c818fab24605c1fca2a4ab5bcce5f84c3dd76aa0272e446b2c4" },
                { "ast", "ad48cc4527750786343f988e6cca00528cfd261320318bc53c54096a6f5bbb808b54f65546b2f7f098abd74c54684e7981596600a3d1ec93817c45739d64b023" },
                { "az", "5e644e91724edd7fa79918b7c715232497129b6cbf976c73acd8e4d4094090660655ed1fa32efe6fb3b5e420e0f017420205ff8544c23912cb1cd0698629cba8" },
                { "be", "e6e67aaa6d552b7a961ff6d9eebab940c8e1d5c99a0ef6bce55190711fa79e81c96d6fcdb74d5b8763f404580d1d3b883ebb74379e219e7fa2950bc770d6af9b" },
                { "bg", "3bc4d853c53dce92e1c1f7f68f76680702b44edaa6c2c164769dd9a6513940b22aab3a6be884695b90c1cb941874a10532e1bc174452eac781f5b5b830ffc82d" },
                { "bn", "93baecf851443e1b61a14373be14dcf02c0db572efa1a9fb544381028eb7a7b1c3b93fb807c70a76e935fb8296c0428797123eefb6e5a8434a76e951cbd19c9c" },
                { "br", "70b09abdddf70c52cd02e4bee6c7940e5f0f4a0a80f13ec3c01baf500af8f63fd01cdf1a6dc81b512db1ba5266259cbf9a5b0650e3dffb03cc24969cf6348bf1" },
                { "bs", "3af7c2e4d45117aa684cababbbf570b0eb1e15c32d103ea90bad2c3692ac870d13d6636b791b5e4b0fadf46b4b563ac64f66f3d7266744dfdc780ee84ed40450" },
                { "ca", "1eb2d7a631642544085ad11e1d663fad9f4e32edd0cb158e1705d39ea729db8e3341351bd7105d62499e8d76c51f100d06ae2352bf259223d391c3accaa0084e" },
                { "cak", "47a9cdc59d31c744c27989eca7497ff35b80edf8af2cb988718bb99bf4d9279ca834f7d883a9fff63cdaa186e93194593a0935181af1bbfe502abdef0624f6c3" },
                { "cs", "09ed1b26e219bd26100b4a09b61a835ad241172a217b964421d97eab54423a6e096388f7b6e917644d8bebec4ad665bcf3d74306bf26aa15c4a6d8e796e9bae6" },
                { "cy", "1546fc3bbacf2639d0f0f438bf9bb1f0112349be0503d2b61be22632d69b39c1aebff0dcc5cd0c8aadf699a20a84523fb8943cff6e69d3125519aa6db2bfcfb8" },
                { "da", "fe519ef5ea96993955014e1703d5308f46efa82607451d9b458116cfd610027773c282d84d2203a606fb0e251da0289dbf65cc69ecac85e6108761b0863d1041" },
                { "de", "8d71cf3ca8e87ffb9dbbb93c078d25226166a5a5f6d80b3b536797a16a13d0932da453b856a7976c9e8d249a6a5a3e9b70669172ac7fa09995d86d353f0c5ea0" },
                { "dsb", "6cd59cc9d0c139b9b56ecd86a3a6d73dc91e2464e6bd27e41b8d4f7c48a5576315e6dca1708baf66881cc29db973a07d3c7652e5de41216c296d47979b297a64" },
                { "el", "e3857df3f32e80fb647bb09d823dd9650fa48f3ba15f2b6136f5e1786e51e52d8f18d58ff4335d65a876ebe38aa94344b9769c55a5578314afad3597131cca82" },
                { "en-CA", "9d3e229d17cd715d30bec61c6e51563105c441ea2f7d4a0e3cea095a163d66339b34be666bd466eb50096b5f9af9bb52984f712162dd0c27c846cf60b2f53007" },
                { "en-GB", "446fa972ffd288f608fa417addbd8ebe72e43ced5e64934cd5cddae2ffe8158bbc94b701f888bdabda764bb42b0f77221d1c8687cab0cd60390bebc411206efd" },
                { "en-US", "cabd05df758d38e4959cd349b4f263b394613d5482d970611383b1b82015f33f6ca9e27c6c156629462d04c7afb5dc063f089e0aa78fe6d1ba4e680784ee48d5" },
                { "eo", "5b8c13fd659f844cd0a6cdca632c12d8f837ec4ce4c6a14990e1eadd73e9f12e47b4b5d763793f9fe62376e0fdb648adb28a12d2c9bef781b1e38cecfbddfe17" },
                { "es-AR", "f30f0dff9ffd9e86076fd2da221aa93b4f52f62b7b5e5243fadf434664a3a9a33849ee937947c1ac421747f5a17e66663e89e9b4e55c11fc312d080fd03d9e71" },
                { "es-CL", "bffbc41c4515a9d5a94b4830c24be04a4a9ba6b1b368ffd99822e1f5b30120b47b5fc3151d7904aa0510e7ecd3f7c3176a95fcc271910f207675ac2ef31e208a" },
                { "es-ES", "81c37cc41d489ebf38a910abecab34c68d8af6c801a23ac5e4c35642a07e7ca93c3891ef793d923809ba6cd4c70f15055e171863d6766b6171deb6e60c617c79" },
                { "es-MX", "0f5cdb04469fb1d2af7b78c1588d3dc4bf818f807239a284364e9263a137a460380a94feef1c5625075e6951f57be221e7a267e91e583a0e943aa0d9d13c870f" },
                { "et", "4097f5e5656ee015a0a5ad564c8c6bbedce976acb23e0cae0e551dea3d064d95ae18e635c38bfc872501a30756850870a4df4f2f76da696544b58642c33a1657" },
                { "eu", "47b001c4ba1947d190f144f11efc8584d0cc40c0378e3f6c1d8390ffa59b2988ae6d80b552c4eae30a202a0d6ca496bec66d3fce53e41b4996d629aae5e8a143" },
                { "fa", "b49df111ddf008edfdfb9276099e65d4680891f7cd2b0e0ee71ad2a735e7dcaa832d8e1e6574023e84dcde9ead1d4db21630c50e179ad7387dfda397c5b5f936" },
                { "ff", "0e9a4126c08afb98e8a50284ba8d410a38c6e7f9f402efbb8c4e14105df9972762b062dca06349027b03603255f3a8c00fee52743984c3fa562643b3df624fe7" },
                { "fi", "e74b228978e2338cce7bf0c57a12fc192feab667d2e05c6b184e66373a514816acb066c3b0c6e5fba11d2fc7e9cf7ca98fc936098b83c9f32d329512f9406b43" },
                { "fr", "2055cc1005f9f2cdab58fd2d865980f61a24bd7c66252cbc106757087b50170ab20f138ba415137b6c256fa744100ffb12f29436c9d97ba5535098379158d244" },
                { "fy-NL", "97b5a412672428f1fcf2149c6f3e07de324df9e445fceb6de1212cb9732d201ef0fd492fdf18a889ac7b49b6d38dbcbb09ffadc0134511d82966a322a0790206" },
                { "ga-IE", "d177359890c04cd17631ba93af84760decb95b2229e52f1c9ae4b704dcc7c6c0bbdecc4813ac04f8b3f9d5f76fc3e562a9a8901453caf851115b71131331582d" },
                { "gd", "e9db49527df82f3e7c42393146e6a5a726c9a979fe2b1acc260987e74819aee2f91551dc787cbda90cb432b587376747dca26eef476f394339427b2478ddfc6e" },
                { "gl", "0e07f021cb12eca49fa017766d34957f85914c59ec16bf9c707731cad06689663c7117e76687c732a29cd8bf5528e62ee58f9c4fcafcbb5bef0e11a48ed0c66e" },
                { "gn", "6b3b9e7b20d76d26eb571812babc2c9bfb2db5a1ebd6875d88e75f7bb02015b6fc01103788288adf7b4d525b59f2be697b75cd726fe340b0753f158718653bda" },
                { "gu-IN", "4eca3f8cf4f6f3a8cc6cb03386ab04865d16b6f14468e68ef9f815c1a81e34c68bda3ae1ade321c52da7bae8ca1315fa3e4b25141c095792b6e4166d8a7c97b2" },
                { "he", "be2f5c54c759e0ead93f762035dc55ad64830f95648977bc71086011ee77d07b1ab55625d0aab274c6e7f5d3dda25667b6efff618a11823a0f4279074fe99db3" },
                { "hi-IN", "0859a908c63616413066e71d07fcfeb819e025634797017390db5c8905a4cebb00759f45a0d39e9bd0aad30c6033991afe5e36c26aed0b6b06172f0761729056" },
                { "hr", "b03b4295ff6ea98f1632f1c2730ba0f7554289eb0aed39462defab783dc6a44ff1d9d39430968bd250f07514a407ccae70b999a7dc38386073763666043ada3a" },
                { "hsb", "9ba5b6e849a3bcec0a41c2f6705e0765ce08f042ce64ad7937eb978ae5d8dc0191ccdcb875dee543d8db279918b94c95b1d5d06cb90a80ec4fd904c6e9beec3b" },
                { "hu", "0cc1007d6697b5dad4fca36c12e75ca4962bcc27752de946c32786e0bbf6e549f55ae961a2d524412b8ba6178f00830cdbe56d291d184de6f7b5a4ee4e0ff673" },
                { "hy-AM", "f1589e01eaedf0ebd52e3696dcd8154605b1cb78af5ffb3f27b284a1643bfe3ee9ebbe2fd4beb03b9fb0e02c2f41ec3028f6916b3386f4f4f911218757fd9e7a" },
                { "ia", "715c008d87557e9a8fbc87463a8114c37dc541b2e824751aae9b0b5b68bd135516f9ee6facc106584daca1db475d1e11bfb9748d1e419a1e925fe64e6eeb3505" },
                { "id", "d3b856770af9ae8c8e7bd45c84ebcc479725598cc1958ada8dba9b3ba3ea70ddaf3c2c7da8a663cca76a635b245b137e06da49e2594291b617a91fdf5464890f" },
                { "is", "342e4caf6c95279936bdbff40f21016e6346adde4f06ebf4c6248c2bbff846981970eadc9223f91e496545969bdc936357c36ed1783b87ac01e3e4a242557138" },
                { "it", "50b1b9335626cdf5a6d21ca55e9cad91d86176659123e554bad3ff975344ac51d8c6aaca5123a1139173ebdc1dce4f97ce43967107a3d888a0b3d40ee46bfcb0" },
                { "ja", "940377665b75cc782def8e2258a4582b8e9858c010f2339fa0d6563f9abb9aeba1fb662dc7b0a4794ba0803147145c0b4afd7a3bf8f7f25bc166de389cd65c26" },
                { "ka", "6ef325b12cb67c14ab8ec61b8a3016da8fadf73a290464ed54d49e9b0cb58f5cc5b21e6598eb85997b1db5d727d88d26562e2f72af92dda9cad70ae7368de676" },
                { "kab", "ab922a8fa517a86059a0076e0b4d6e91355edb47323c1fcf00a414ef470f3044e6aeebc34dcc14199c7ca4af7b0003c0b61aca489edf87ab94bf8d0752706217" },
                { "kk", "f038c680bcb18f6c9b55ee36275150c9a55712c3bdbe3cbb08dbe8151af89204760e2e5dde1f690c4a71bd53542a623f4bbddf4cc884c2e6e17cc2c7f8d13cf1" },
                { "km", "0933b666331ebcb824d3319e14220c76541d8bc99101b3006444420a508a8318b8d84ed8e78ef89021cc97191e47221ae948f238c378387963074a370e464982" },
                { "kn", "c0760922c7855ec1d270fd60d4806e15cf1339968a149bf2b0bab460b1bc789830f77ffd00dfccb418ed07602e7d67f86473215719b0a7258364457a58a38c16" },
                { "ko", "c623a35a244f4ef0cfc3f462f18ae3a34bd51f69d15fcfcf160956302105e7ae089f1189e32f62067fee85636a5ed7a1dc75e475402ad3f33b8f908bb61f00cb" },
                { "lij", "738db12971cddae0c87c5f4ce6589458a15725a7ac6a06086a65d879aa5fd05479c287044e1ec90d2168d729520745e0e7a088c06d6982ce77049811560e3115" },
                { "lt", "e2fb5efb96989cbb6e5e927ba73c9cf5da14ff17a81d3f456a43c50541e238e9df314c9ee7c418106c36a4e791e42deab03bcc818c2863a84f34b1656de6a05b" },
                { "lv", "eac9d9430ca2198d5453c198a7467df8c4269ffecd85a9c9f1ea714569dfaed31994a950dc4558fe4b3f992018be6e75cd7b22d15729a2aa71982c3a9ad19242" },
                { "mk", "c671b1e0b88b50e68bba9295cb812744bb587fc4b49620e02fc9d5186ec5de10c1f28e983e3ae7f571b92bd4afe683c0c125af09fedfc7f71d1093b06e213ae2" },
                { "mr", "56e015c7ae6fa0502eaabfd07b2e41ac7e0c31b912e3253015e6b7ddbace162df2fb5f8d996c848b478c94995be4afd1155a1208ed81e08ad8f7b333b0550104" },
                { "ms", "f0361529b96a51f37365566cb2440a5ba672581fda33a4d72ac886ae9abc101ad5afda0594e342bfec281efe0941e4f0e16204252ee00667fb270be350347b4c" },
                { "my", "daa1faf39a1d55a5ebe380d591d9ce7d5c0e8840951444bd9d05a9735aa807df2a4d25e97bb463bf4e5ce017da068613390d75162b526f1e5f1a5e03d1ab861e" },
                { "nb-NO", "341d80363fa3222fb0250554e1faa065b1ce2b74b802467af37fe218237e8171c7683836a03649db1d9a03b5492da496225b0320ae170f611134733d84f5e692" },
                { "ne-NP", "00f552e039b8de00ce63679b8ebc21ada40c50c00f37f9984578c3027a30bc31ca7a89b5853b779d77502f657c9216bf360c43f8fc63ba98f113ab1c1fdb0fc3" },
                { "nl", "ffcaf101233be50e2e70f213cf419b0f1e210a003790c89f606b58e37b49d300ca7b930bf5c26002ce6d87b50244de471b59679c28dc3ef97f09702d8e5508ee" },
                { "nn-NO", "973053abe1240c8b7b7bf8eebdb68ddc90a3d64df65cd4c1a1cf6e33e278b281372c99bba6898307e315024f2ee26c21b4439952438a7a17abe8a3c21949890a" },
                { "oc", "a2ac40648b8af48d5bf03c410e5fb54cba6795c737635f3e958950c9e0bd34d700f271baade71ac799d498be1182a646ad777beedd17692c7a1ea36674ad7897" },
                { "pa-IN", "d9856788bd7719fcefbccf953ed32061449b4ab6611ecbc0ce3a53c7836d8fa04902ee361ee0505ede05cbb6d258da5e7d80d7e5cbf9bbedbf3a4cf4243c7535" },
                { "pl", "550ee1b9958e962cbfafba418079d2f4cec6d8b910d147edda9cb2b381d063dc2013b8a648c5cb9049de78f96acdfae43d9e6c786e1966007cd580a7a1b5cc8b" },
                { "pt-BR", "b2617a54dfe180ea6cc50c3a891cfab5b0cc38ae26c4918121e8a439c9f18a489b14bf41e3806a30bad87becba8b8c23cd66793aa1d4311b1555bef44ae8d331" },
                { "pt-PT", "5c80af5e979690b1e5a595cda1298f05b3ca28d5ee2c98b8dffedc93b3e1e9b5f437f1e865b9779e1d60bd5f34c88f5398278220ff3d93aef3543e91d880b746" },
                { "rm", "0f34372b9d11e55914f835265a1caa7659bf44467fe95cc1af623882c370a1d63e62c8f65a691a96eac77767e55f2a1aef89d678e2eca819d06ef252794526c2" },
                { "ro", "5c14f7141e7e56556477fbbef744c356c4ac8fc27c7a74f6061fdf66f8932ee8be3588eb188e91e8cfb072c3bbf1d42250528256eb07b8eabf5ab0ba6733932a" },
                { "ru", "48a118bc0260650c7f7aa5c6006a7fd3e03a4a11e05d5f42c0617bb3826928ef0241ac40a927956c51f2b18c518e58fe9eab40db7f1c147b78d4ec84d30fbd30" },
                { "sco", "c4fe7a4837df3ac9612bb70fcc2d6d32ae544b74a2e80aa989f68171b9442b72ddd36bc8f0006a2a42d133a965ca070561dd8a5a0c730cf65e7163918950499b" },
                { "si", "e5ef9d180428ab37e47129f8160d0713beb480e3b753c4427fadf93de08be32ebe9a2306afeaa22b62d13f919a07ecefdbefe60afa4758f1b4acbc0d20f1833b" },
                { "sk", "612218228502e6f82db35fec3a160dda81b01a056eb01f14eec1723fef49707bb15a9c4402c7d9cff3034c6fe80946ba1e09212c644a3674bc9a6ec25bc60109" },
                { "sl", "6d9a577e479c516a455051daa3303a8c772ce21387e10a4ba61f0f25af869655580533100355058c436ac7525e3483670f0c9bc3bd8d4e620c7d184d57850b05" },
                { "son", "dca6fec06e3ec36201f7e87e406c8cdc64eed937eb067e90daedebc3babac3afda58b0b9b20ec528023df8d835cf6f44f2668bf8d5d0f71a21b37124ff321fe8" },
                { "sq", "c72888ed48fb701a18dd16ca4ea511eb594644123eaf6c124eb7a7d9d0234b02d9dbccdc78b075e1dddd1f16695fc30a5937c5dba10a51830f9e78df933553a4" },
                { "sr", "a2d6d38169b5212db6bed8ea1c44b3359ab1afd801a24a4e654e1c1548deecfa941973c60909aa5382afc1247162b0393020fa0a131c3da36e4b6e60d0fadb14" },
                { "sv-SE", "a589926a56375a793965c91f4298a782206876566892fbc3a5527d63eba948f40ca174f7ef0afeca968e1c25324cb2b15fa8931eb8c2b3158b5c6e6c553d0182" },
                { "szl", "3965ca4f52ca32ebaa8bda2a86dbbc3a630e66b4e3cadb2aa951292f39d440b489ea0ff8b0cc26c0aa3746e5b4be8f860344a13c3ddef116de52d4ee12cdc4fb" },
                { "ta", "4c8b0a79e5bbf5342bdc06554188dd5cfab2e6fb4a3f3517db63b22722d21ebc16ef8c2d23b8d3d00ef04a5fc56509a645e24f7556c89abaf1982fcf1ec1dd47" },
                { "te", "18f46e519270f34cda38317f8961d0ab2c6400816f2f939284c2ba50d4df4630c28f5f7e807990df815bcec78c44192dbd220a19b72db2faaa8d610a8734ef72" },
                { "th", "f16ec3c6b64749fdd08c25116be045bfe57927801981749c994c6b837d7f93cc15b8617ea9605668d62e9f09d40ee3a82c3f2e1331dbeecbfdd12045af0805fc" },
                { "tl", "97df2bae2061d1b8b8382ae59fbd4038670abb8e24e56379815d8cb41ac01f65a1b3c87ea7c987158995b6d4a084180ada3db34c9de42b0b67d2f1b61922ba78" },
                { "tr", "3474e43f573fa4e8876fbaa588f56c19443ae72fa63502747b8ca1777996478a783433c17017006de33dce57c0495c87a16e500c48cbc80141f73a88bf9bc978" },
                { "trs", "583871f89610fbdc5b7d2e9c69847051a43720d1bef1f76156c3254d57423e70e281491adf4614c5fab2e2ea6ba13eb8f47379a01a91d9914de7304cca3f2bdb" },
                { "uk", "843671887c20928d8bb04390d6ec3be89b021b997c0e52943a3970909a9cc7242fa6744c169bc8163e12192f6403f5827549b705e80982f6d52bfbca0c90be72" },
                { "ur", "58acc5952a8914125283ee9722e30b83d3ede5af6733e64a62f4705cbd7c17a06147341f50903702ec956770ddbbcdad44af226f715cf69f6623122a0accd130" },
                { "uz", "35e264f44466604b73903c5100cdaacca0533680e6ae5d633b969550ffe1f62753cb0b2a07c25328b817b5b9feed8caf35872e3eda65d5e03c19f15499e242ad" },
                { "vi", "b17c18c2b862b7a88e429de236830c84f6b6e213916b9c62ad3694fec7e2de088de9322f49ca155a2541c4430ade24bdbd58e0fa4247ba92035e2a8173e61d35" },
                { "xh", "8e66d1b6e3a1b624371bc36331eb8cb43a21cf6d23d81e71c51655efb4150a761de56f6aabf808584f34e37aab8f600f0fccd8083162430f29c297781bd3bb30" },
                { "zh-CN", "34e7114234371aa9b36f17ed1089eebcf2e753917b44d64a32c2cd61328d05a29335ddeeac1b306ee49020d02983a960dd196c4bbe474d6c7eef52a9da86050b" },
                { "zh-TW", "153b35da3d7060e42cd943501a31974ad132d3295037c9289ab099f611b6e2aa6cf70a0e34a9812ea4244cd98fe5be3275e21883178fd29322c235298f7aed61" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/105.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "2607c69f7ce6bc095b5e932f78b536f941508ee5efc9d1f2da86517c748ee0c00949d06c203bcd8ee3f4c3ec63e1d1f251e85e7c9cc74bfb0245133f5c5723b7" },
                { "af", "19d7e6d41ce49a21303dd4ab26d6336c467160af7044cf231d80fb0ebc4b52edc8480ad355377b00cf2190e822aa91ba9c83513cb5e99fd676fd6dc854ba19bb" },
                { "an", "516f6eea54396824f3d8a2e83fc226025997bb6f92cc3ed70a9db69cde9210960c7e5774199c3caec6ddf49b78ebdb71b27c6731805f8351216b2e4091061109" },
                { "ar", "e20fc166c3332ffd6c9fea5d1c9fa43e246dc8be1c468884e4a53f53b80c61a56193fad4ffa4a730a036561ead9143586c4f523124d799023cf4e036a59c09ca" },
                { "ast", "505e77b8c943aa3665990b1a3adca56e8c824a4ff39e7dc5c369ffef7e9e49a468e94baf7172ded6caa34a6fadab420f11706aefd811132fc67d761f24adfb73" },
                { "az", "7936ce60308cb3fe4d3c214fb23b92847985458e8e7bad50060359ddce320d02477e3f666533731ee5775268e47c337287e855e2187c726e318005b719b1af42" },
                { "be", "e8d05092e1e222b334b9e58cd3a9462edd688fbb040ab693b3cbcf2751a3756d46854aeb6b8822927451233b26ed1176bfd1318133c47bc8737773a212663879" },
                { "bg", "5a1a88de19e91cae68b909f0c28425954d50b552caf55c9cfeb82a77a47f704b22d93182040f45a99492d329b6dc622f929a5f44c7020314fa4c05d5ba482227" },
                { "bn", "89283b3d2b9c0da0834d36dc3f4267ccc2366b7db560fac1cd8227c3c1b8ed2d5264033651d6ac29b62b0f27e41d29cf0f43f185ed2a15d1af42c34eb9ebc3e8" },
                { "br", "fda9111091ab847173abc5766546f54612710a4441176cdfcdbae1a6389caf6ad8478295cec2fb44defcd83ecbbd258f618c0432d0de9028a90e765d28beb83d" },
                { "bs", "f1bc84df866c41c0e726a7d85da9e2c31f6c5de2f283b975271d8b89d22626aecbaa264ef1f2e58df4aa28cc62a5a9f9782e35849f170b044bb486e7a0eed682" },
                { "ca", "0f9d1f4c32b71d4d19ccdfd314cbf9db80321e7eca6ffbc93169552e8e525de8d606385c1529f38a816774df82537ec3880c988b71222c30aa1c0e2d4b1badc8" },
                { "cak", "1da9e2e517cb894f18109b3b83533df90c4cf6f71cfe6c9dd826fb9615a59ab1cbb035e859f6947adcf56b43ecbfc3dbc1bc788358084f201b73ad633ddd3972" },
                { "cs", "adf796b27076eddd22fd5a580f473c04d2823f9cf1e8aed97c5cceba53d851cb0ecc80d84e87baa118bd11ddc81b718b070f69a8a42cbbd42b23dacd3cff591b" },
                { "cy", "22b0adff46a0c244aac6142f72f430943b97fa69298c4eab6cdf444fa30b2821493190c1c6d22d08a0a311a4b7f97bff41676c70cd056625ef423061db7bc4c2" },
                { "da", "70d8143f190899f29095e02236f93687c436e343c17892f02d04c279b8991f697d14352b0f86bb902f82c2f05c53f57b0480c7eb95339d9be3a426e62c6fabb5" },
                { "de", "977f830cb28498e79260289f657c7bd1bda50dfeca15e66e39e5927746f59261d728ec0c2d19a4e9523c93d56b33ad6f868fd4af4e4be373c98da4e31e965d39" },
                { "dsb", "580567d0e274c7961c39a48381ffe6f5dae94fda3c8ad2e7a258356cbde8808d520b073890487fa9c9ba0283836798b81a674fdd5817d360aa65fc38df7a9986" },
                { "el", "999b91065c9ef6b61a443f9b35365caf5b505cd2545b9d9ecc23aabacd67c6f0b8703a1c8803fda9042e86906948107c97d5372ba99749088d2be723d11946c0" },
                { "en-CA", "751ded045364fa852b28ef5e632f68a619dc5af9cc3244faba7303124e14e19e7ada35a129924dbb34e8777cb5c6ddc439ba60f2f44b431ef4618fcecdf2343b" },
                { "en-GB", "123a0647990d27a9e0a4b0c2bd3fe9493c0efda3f70c410df9c48d5d5713ac5653d9ea923fe255aab7f7db37e861175785a877b038e2c7eee1734f7a08f56d1e" },
                { "en-US", "90b3e0978a3d7802bcf3fa35f3993cd524c2891f524583a53eed3064882ca61958eec2946ef6e3b04bfccfd2c35058128294de50e29d96b15aed6aac974874cd" },
                { "eo", "b5c99c8a817d4c5475ef50688f01c1f1d554d660d9e510212860588e07e207903143550c93d7c4d69ebcf67c970addbce3e9f30aae1f0ee8ef81b23aad4f26b1" },
                { "es-AR", "423bb20a840fe9340039f53ce46fe9035f745df0652f287428feeabc65c05c5d8904bd87180aaef74b69905d12a1e6a90babab6b81397461c30233d8c2327cda" },
                { "es-CL", "a186d6411d619c675afe1a5a68a15c5dbd7f18e65988e5638fb82f1b9d288ac7f21fdd362c0560c657110fd8efb92198a7554e994dc908b4827406e3d12d1f57" },
                { "es-ES", "8e7f3594c9dfe7a1f44433f4907833a177d5008ef69fe2690c29183fc6c1b6504c023fb50717101a31fced9cc7b0206c70cd866e1fea0a6727619d224cb6782e" },
                { "es-MX", "6e49216544e4ccc7f79a187fca8b311250d955c4ea8a1c0dfc5736fca6de21b555b2a4cd6ebedbc68938fe67523459193912d282a57b58e02982f5037a85fa14" },
                { "et", "3070625c350983a6011c44b6219703a65b539d987f8deffda3e0a19fa7e21e9696fec2c05b592fa08b961a13289f3a2545d677598bdc6461f5ca307b30bf32a3" },
                { "eu", "56f18d30eacdc12e5abb02b8d9be6f20a3a8fc144a587d376d20be82926a878577b791e9e65b7e462b9619d5408340dec1e4f5f16edff7c07be62aef9079d6f6" },
                { "fa", "792c4b9b9f7c01e2806d3b47e0a05e13b5adb08aac251b6b28fe3731f3e17cd0f75aa7e94a9e98d5e7888c2d5907514eef1859ab0e15a0f8fb747ff424b1e7da" },
                { "ff", "fc0170c1f5852c342ef09e0ee8d8b280c92ab7d1e4db6ec5def5e99adc876c57e266738f1e8e43ff110ef88eeee0e132408612721391c1a5d7cfca69e9894dcd" },
                { "fi", "557b55b6b1c3504859272424ea6c730af66141dddde334377990fe3491d9fc5689111ab9c4be7fbbfaa5e501bb85dedff3002d4ba1e2209519f34cd93b699e5c" },
                { "fr", "49cc6f8fae7b568d7f4f8a2684e39a0572bc3e4d1eceeb21e207de00c011ad52814ce5a20efe86e8cb333a4054841b2d3ec27b82b16f7d7fb0cb1a84e0198922" },
                { "fy-NL", "3bb554ccb5ecaedf3c6e7e3332064290e095c489ec8b94f7b6996426724c52cd52d5f512697917586722c06b9154a40bfd25249a36eef89c449d822f663f03a5" },
                { "ga-IE", "a3e7041985760ac61acecd6d29d8f08ce6139d23711dcab79e43069351a468420323c571cd04584a49c9a62b406aef280bdeb9815387a24e9d95e1fdd36b286e" },
                { "gd", "db26bfc6ebcd4f30c3aaeca7f46ea233f6c933e268311716bc9433265cc1e608feebd49799174580e6b59641b45c82850720a3cec74a165cc8cf61f2b412e7e4" },
                { "gl", "0ffedb4016b9510622f2a1ddc6ad9b7ca70636e271ef0737e4e37a8b2423e61934fb47dec83df2246431180345ae55fc16db58dac54b578ca85b2c3bc4e65ff1" },
                { "gn", "57eb29fb736b74e66f38d973e08b6d550005aa5ccc79fc51982656bb8cbfe9b27085d8d35d3ff8502285ec1aa819258e270d999f66add977cfbac1a19493d12d" },
                { "gu-IN", "0a26164ba78567a0dff2c0acce70ed648df32ad70a6dae5e15ce152216faa56a9cd7ec4ff531014ff533107bb3a7ace4e3cfbcdd0bbaa138a3eee7c5516b7492" },
                { "he", "78d3bf85907d9794116ab9a4cb3212ce00c31bfd3b051c7c99a42e6035cc206c2937abdb7c43153591502e0e4b96df0cfdccc3ea3be99f9b04c1dc27cfff88cd" },
                { "hi-IN", "6647f7a66ac5c6b75d6bf7d87b935894f2f6e3852c640cc57c52d20015475430a4e1ca2cf07385718eea15c8d1eec83e21240b7ea0b1879f5837d2fd87286a3c" },
                { "hr", "ef2613a2d1fe175fd0ae3b4316a71edf65c189d71dbb8c45b9774d5ef461b851ca6d273e47d772ab49459f3ffaf50840db5c8aded00796cfcc5ea8e6fcd8a00d" },
                { "hsb", "e2481d636de45be73573142b7e9755c42a21a1ba24052bd82ac30d545f80d3aec5c849d644e65aed772e377518d7a88370697f62e39471bcbed486a9c3ac3ab2" },
                { "hu", "152f4335e52fdfe6b8678504352dac0dacb873b04ad1817222f24b5bac3961d755fdc13f6f0c7615dbfe5268ef407d2e2d7f80baeae101644cd46c8fa7819f2d" },
                { "hy-AM", "3ac28e3fa97ce5dc0700f3dfb9982472ea46aa4c96c43b90394c2f7b742fef262e647d1295c3264de866262b86b4c7acbc77d3a42e6470fd12224d5e2c154364" },
                { "ia", "c0b3810e6308df1ef581a06cc465dad72401ab47831fc01a0f63eb3445640211acbd092dc89e83bbbae108e27655349add09a0c54d4cb26e70994b09db1094b9" },
                { "id", "49f9f0f003672ee63ef2e73b28853bc299674a91372648f6cae1f218b8732842abf9c3c3cabf866a527387b8d4603c218b227740fdafe1b0cd09bfe198741e12" },
                { "is", "542808973160a724bb357cad2ff3ca42d8894f0e0f024a7908a409fa940146d4d6bac0f7d9107a3e26921e2ef1aa793602ecb7b9d37f3ce8855a745ab06875b8" },
                { "it", "d3ee75aa96d12ae1041ae452b410ec46d23d6e4641635ddc36b1dec8693b3b24c0ca9de769adb332825bfc42dc1e1dd7463c53c73f26a296f5861b6fd115e2c3" },
                { "ja", "5acb4a19e5db403a888a71fadc35b6bacfafc008cf7a4960d4c4b44dc0081730f0d9b0ce1b7f9781c192cb8ec9ad4facd4c3e5a0cdbeb68231bf20985dea3b10" },
                { "ka", "4016c72d2c9f7185be2aa1e4a1baa70974931bcf7c0a516db351e931ebc5ba983b724a8ba53a0f532a25431511c402fc631523d21b3f69797e7b466bffeed2a3" },
                { "kab", "f775f2bb3c8ca55ce2ef76f2ce2cc537a5c3a5902c14bf386c6f3261e6254a25b2b2b16af2b7ecb98ae9a33bebb49133a505f12f34da4a14bc746d34c6b39fe9" },
                { "kk", "06ed7f80458f798a947f42add4e3f7ebf75c6ed39f1829be129c5679a60b40993b1466b0ec09fa350d83db6922463955ada64666383a96bab8d624a26d680a26" },
                { "km", "61137f6759bcc421f18cea2b9c6ecf4d28aea05f866c39f13e840c167e684c9fca0ba328e6d5c6f223a3d2ac2b4e1300e21ca26865317034dfe951cd6d3b7e20" },
                { "kn", "fd18cf22df8dcd9b31f60604995e9d5691fd03d8824624bd7ce1b26e3e74396d491db2fa14cbae4047b4a5c477bef594a8cbb1b6903947efd3c50cea500d3020" },
                { "ko", "f3183ac5f6775df906e44988d4596415c7f2e33f0a08cee4858ae4c47e08cd45ec4cb5853a3fea816644fb42fadc55677ca4b261c470954b6ed9a1f781a0c515" },
                { "lij", "8cee4b653628ab8019173a1b39e0def45eda9171ac4acce0860d31cb1d8e91389d5ddb866cb12492f70d90a914ad03fc9b4dfeb53244eff0665d8b49f0347342" },
                { "lt", "67975515761c60bb6350a4230a9b95b7dac591a6024a7bc7713e87c334c4b8f7057f80cc18b7d3bb8f1edd492c604a78550100fb2c31f4e5786825bf88644efc" },
                { "lv", "b896820510efdecd07741f29052eb968acf7f49514fd6013a5aa444e8c596af6556d7f0300bd3c46f4a18fe0654e6839177e6d17ad475c2f7c789f8f67be3a58" },
                { "mk", "5d973e53d50984568b3b59fc9a03022f0279ddd828721cb30e9cd0e34c88942cebe9175cfc51edf1ab4ed379c123a24d032bec2d270a3fef9cfd14fa8564efab" },
                { "mr", "7c78f2f5b0cb9ec7d54b7e3f026844d4586e25fe477738c4e1276d25170c827f02550bf9e77f8796eee6a5047498b7d42bbababdd8d3c6b6190c66a86e0d9414" },
                { "ms", "bdeda8c261603808854b8a6fce5174eba1f174781c589839870e7443470c6258679b7ae2185434a992b88c3c73f4ad1ed7a4fa722d6d13494fc4db73cc4c9b16" },
                { "my", "12af452364415bb3cbde90b30d11105ce54df7344233a533b15257bccf5506c10673a9e893fe5c19b5c64acf8d6c0c60cac1306e8b3443aff6b15af9d5f840cb" },
                { "nb-NO", "25256f66ca4fc6bb9361a3a0a438945269cc97eadfc8de51563d2117069c1116dd61780d338f6428bbd0848f872252e87e1e1c9b5d986a7946571d89d30e9867" },
                { "ne-NP", "8e774f0acf3cafd720bf5528dfdfdd6edbebfee4145884d9673c3fb600d7eaa7520f4b8b9e3e5c83cbbd1fb7fe1ac7b3ea2a6a61eff1561256e7239de60f63df" },
                { "nl", "558a0a69ac00da8a10a1389e9570b33ce53db5912796f79ddbee9674ddde1165279ddebc58cae2bf3a49ba983cbdd4d9913de351885467ae329b1b48ea7b92c5" },
                { "nn-NO", "66e470969f78c45bd7e1a5a1134815297b1912ffde634d879ec007e0afffbfc5e3c95dc08d3c3a92cecc1cf40156513e330bf801ffed83909973a6406ab89e79" },
                { "oc", "8dd994c0b9debc9b1894449851a8fab3c65d44e53c53ccaa4ea8e60d9b4b95e20c2f3118ee0bdffd3ea57b6cdeed5099153074c0056fea7f6ad6d21879c2ec9c" },
                { "pa-IN", "7c41fe778b08b3574d89c712e01d1f8ceabeab5516167a9ad6585493b1722f08380c03aaef58c99a85fe77806ad92339599842816261489f1fb740aa0de8dda2" },
                { "pl", "4d666ec5b5f0238f1f129a15265865d68b116dde05080b99b44f98290030b476b366d859db4e0c1a7d154eab00f178ff7aa920cbb47db6be72a8e6f2110a047b" },
                { "pt-BR", "d78fb00af41925460d17eb390329ab1399b6e756ac8c6829f4876de09bfdef457cbd5a7d9679d3c57ba6ac920d61210323c81d82e9996f3886b5905cb6d920ea" },
                { "pt-PT", "88f5c4a99cc22c2b36d82ea3b9a56a8ea6007565b6d8d135c09ea125c09ba1b7cc7679b833a3e8813a01889fd1d7f38cd6159d787607709f1cb74fe51f8d263f" },
                { "rm", "f2780fabe585f649964097594b2652f29f8868c8f6133a4ea7cf208457a56b00e4e427eafcac560556699fb7c48317920b167af6b8b862411e83570c1e5b35d1" },
                { "ro", "2b845570f8dab6e66536c951fcbaf7f6343236c36546963ec0a6c61a57b7f0f19ca84ee5f5cdbbd51b77700a59a13f6ee1c99c627ad24ce59c43fa922c67ef62" },
                { "ru", "599e8625288997493125559246448f075f56970ef1aa6ab4c74aefa462cdce59de1cdcea867d47f985ac9b6678e7c294a0e8a1ce1166adf07d42be75edf89256" },
                { "sco", "a6969dbbee578558259664b6e8341891558a37919071b129c021a1f4feaef6eb995b273ff5e6466ade631708a528bc11cc8100112c7bc291a72e26b417415b7f" },
                { "si", "35af97bc03f147756e85c74d63513b8bb18f92b6867ae4d86cd02e4fa9d8970487607aa1294298579b10ee6eba0351da3766f2e5cb432c0934839de110201536" },
                { "sk", "2367ce4f030fe94f9c7a7062dff094a9523acec058a4e6d2f004f9755029c184ca1557db59f98a44759aff57b79de2713fb1ea718dba771c498d31566534513a" },
                { "sl", "3e3a2421aab70f20db5163bf4ad0261904c1da79776b010aa21e96547733347d977b94fbdc7931b9125a4eb0067bbbbb381911524d317f4d9f6c9dc618361044" },
                { "son", "a0b8c585fbacc54e573e7304622058fc2478d03ac2f6743a97ccb6825f3fe4fd27b78625e80cab4eabb15556490beaac679f81ad719dc949fe2540b8204cb947" },
                { "sq", "5cbfdd1a6336dcbf80800dc1254c448e3ae2a1ab20e983e9f51a4983fbf5f79f9f9c3502039bcd357fb69be1d5048a1e10b044b8de5b75e9f0d8f97aa8c8b210" },
                { "sr", "a913fd70464a4745e895ba32f23c4aaf1f6948ec880a8fa2b852d66e9c9ad3296afdb7be1d644b3e54794644f88c63735719c992a5a882ac6d83225880659624" },
                { "sv-SE", "1f2630a46d17d3f1e5954b7c7458ada0c06651190d238105168e91abf43c249bbe5b582b64f68460dc229d15b3303af4dec68ea7073bfb2e606cb533a087550c" },
                { "szl", "ca5e21b19ebac5b9acb88b9e62ee02bff5464f3995e6d253bb4da6081d25a266194b971e020908bd788ffd3bed44562f08c5347e0653d7ebb62fad4e3e2d640b" },
                { "ta", "7b4a9dd32c0984a7eaf3d5ebb2d2c27663f5f88c02194f9f96efb22e7feae8306ca885ff68b6f644d4a120c49a50ca8b6531d1b9aaff5e4256742db82712a423" },
                { "te", "4b5fcf50933968b50a657b71f177b81b05860a65432d7f27bb31293ee2e83ade1e7502244c69ad849d82cdc4fb94678ef80e543d29c28ba4974f9839edd07599" },
                { "th", "ff18e29891e3e9b24336f43f7ddddcfc690f8a1aa6be4f9abab4727dca6bb5bdd71bace15fa8a5d4e24ece556fb570c9e2c67d0b12c52e7adf900eb30aca7396" },
                { "tl", "d292913a955c39909d0665ff8c67fe4116b878ac51a539bdf6ab1db7eb159c9dddcca101058f76ca17808ce9d25069b7ca6671014ecdf35a117e455610b34021" },
                { "tr", "bbed69dd472a50ebe745e72da05eb42e31b64a5c678b87dd9f19d9fdb26005e2b47e80714806a6d4b5debb70d8ca4f95e8676b72f6465d6cc52ce6759fc13b19" },
                { "trs", "f140d0cb6cb531e8ff5e929bd4b38fc6e15b935a5e8889fe24ad1354ad8fa8a4fedd2036d613055dc96a80e27f96259c0f3c88573b67f3493ea1ee25038ca5ec" },
                { "uk", "ee955bfe0586587774d6784f88615b34602d52c554ec73bcd2224cb267fcee52dbb67c39723a40fbddd0106c00b3e30a669d1ab3958b57d1a8b7973c92672a75" },
                { "ur", "9fc7d478e2a05148b10c240c9943bd6d3f38c80a3ced236555b76ab6378cf9512bd48bb2c80bfed04fbaa3724aeb284f90f9141d82725885f83a4480ae437f92" },
                { "uz", "a33c8454b8a7ea34ff6636b08948235223f26a213ada729a770242b0cc2c432bc83886511dd86b715860bc5309053de52cd56bf03e98a2927fc9af65870825d6" },
                { "vi", "40d02aa4f4ccba9874b778d4c1465986487230eb1c1299433977660280b06ffc50bdf33056da56006e004ac7ce9dcb7ae39098189cdb64b226a0ed6b10812953" },
                { "xh", "d2810577f3f8e007da6e58b795e92667393580672c08cae111e97f74be4762a2d94f2c45da13894b1f8683659502c291881ceab95c40c807ec05ac77be50c2f2" },
                { "zh-CN", "5bad4cc8b8137a4c881a42abae3689ae7a00e48a52d094ef134278ffc44d4d0e70522d81c459ad8a7480dc0894cde0790936dc1e171de05c4b101f576b8c8c5d" },
                { "zh-TW", "27909d756fc0c9a7c2fb635dbc7eb1fca3bca958a141637e835a7ffc8fc874c438e1c83308f947ae541a300412cb29cafcf5448ae0d68d320ef99a38a1370a8b" }
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
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
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
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
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
