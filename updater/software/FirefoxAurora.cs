﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "136.0b7";


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
            if (!validCodes.Contains(languageCode))
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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "b89012aa32c3a473356fb67e132c116e136c2225246891be95b8da52279cae5a981b40e5dfd736fecb01870bf5f75a5dcc11f000c9668de513529d064aa67c3f" },
                { "af", "a294ef413211804e5faef400bc162478ae3d9208e365f377395686d1a74adea1b9bee9e1d0c7e23f54b7b4c30c4fd43fd536f1dcec96578a10b876946469041c" },
                { "an", "8e109b515e9028d434caa1c1b1cdd5bd260ea83491bf7b130646d1d276bbdd7c660a5f92712fd148c076f06323fea32c00655ceef8c40d6c2804756b878e5a51" },
                { "ar", "35d9d8fec93d2e2faafbc00c32110cf62c76e2adc08e65adf7aa1c145b01a05310127331ad178d3f8f78fcb559e9700e4e980e006fed849ec7fc7970e51c7630" },
                { "ast", "3d622360ae7315ee0a0ef7d6a92d7fdcb62b12fe1bedd9c5b9b43d6aa30bf6c3168a3fc6060394fb60b78efecb5baa061b0ad2c1190c0872bf4eade115a0673b" },
                { "az", "d0716073b06703912807ac98c61cfb73f80fb412c27f49eef2fe6cfbfe465c87dce3a7e12a4af33952b79cb9da2e847e0adbc735862a8b901cc559d332e371ac" },
                { "be", "f775998d0a5d3513c2685ffacd1f4e8cfe1a3e9811f67e75d1e4cf0008ff58bc84a509f72ac6c09cce017b060b7d8fcff077335282b4f8cc89ba94bd20c50876" },
                { "bg", "0cfd2261d880e167a79087dd4c49c19382731ed23f5ee52188cbba4d7008791a16f01f6971e8e7c63c478dc2d25036569cecb6a8f4a2a7ca53ad89268cbfebaf" },
                { "bn", "7cd125b0adecc9848e5cedaee79492039fe8fea7243137398da3806777addd60556d309dd39e5a3b09a636f1485fe58893dfbf84be259795231ea19c669d5b0e" },
                { "br", "0e581cb0eb37fe3f007c469bfdacf3f1b2cca9b5240ecabf13a5349019e5f39825af6c81b4c51c1960aabc27965516f7461d8df8fbab3670601ef2f80fcbeaff" },
                { "bs", "3a8af2928449b0e80607dde51bc21b21749df263ab11a4cf52f214367aea47320255c94819a3b10d777af736b9f6c30e89127371874a75c61e3ba7912b80f794" },
                { "ca", "9e52d6b760b7d5a7e27598d11848a41f60ad2c677b19a4072c04fbeaf65e07aeb1df9158cc9efb378561564a18a79f8f00ca5c222aacb11fc6b262b6d537122f" },
                { "cak", "118ec3dea724192655812ad9eecb922e2983bd5d1b36644f00f21feca506e5bc358a9a33c4bed9e70023076acbeaf3117e21e21bcd8c38fbd7f5ecce4b5d38b1" },
                { "cs", "5f600a6b4916bf36619f65bced82cc0760e476e6458dce41a01406393a52c038555b0fcd6d234e0e36dd5e0f26348cd818e49d24d10ebb04f8adfc6e0be75f51" },
                { "cy", "5745f826ff3f58de1687db8640abcb93295f043819b766fa58b3bec31b6934596ca1dbd156abd586a9947efe1d5a69974122bdeaee1efe53faa57986fc2e9bc3" },
                { "da", "d00ff30c81396f723d24c19c42dc635c09c2ae09137d6807e3bbab3234e0d066e9a20b9c523f5091264d86509c36b52825c5ce8dd37c5db3cbaefb5d6d3f176b" },
                { "de", "f36b33f16753a3641adf68b462a42c85a1ce1459e71e84fdccf0ddffbf2b1e3c6aa514998abaa63c1cb3a96f626e3421be857d5858cd01e7e135970a61e8796b" },
                { "dsb", "470d51b170473a4e2e9222dca0a10e2e67005b7f4dcc1216404e0b33e66ccbf2264e231a6ddacadfe8cf882c499620efbb8fae79842454bcfcb3497cc16e5ab3" },
                { "el", "869bf4a28954af21bf4ef1c66c8270b66314981763fc9ce2a6c5a3e4fc6da0993997597628ec169b512758b3e54c57902734846accfa618619b0b4ec2cb9cfa9" },
                { "en-CA", "a033901d4c34757fbf31b7b1f3d5073bf2750b9fe659254b67568e4679cdaa7d873488825657da0da526b209d765ea5a6b96486e62b346773958a611765cdf25" },
                { "en-GB", "bd8135203c36e4c7093791fbf185717c56b4abc929e93dd1e8c1d5ecc14dabc4d7842c6030d532e79fddbf2d957cbc2ed87718a05ef1895be616aae525bf7758" },
                { "en-US", "4ed98520fe603cca81584744a4b133bbb44f9e5f9d6db0a6d7cf022da45d96f8e39c1309be5344ae23ca7c3ccc0e7d3ebb05a7c5680b73a80b5e53f5d50e224f" },
                { "eo", "a2fd3325b32d4037bab58ccd196b42cbf4edbdf2559291590e9596829f80496b4ac288c3d2d869db4d593af6d3d46150abab7e15f665de49545540affb704633" },
                { "es-AR", "1cd5c32266d48d3e05c4cffc5f960b3ae997f9488b204291827da7db79b789a869fc3c20cd70c53de0f5146bba0a894a3d82a076edf4cd8a605672a14a4193db" },
                { "es-CL", "f715b907be242d209aff3362ad62c780a07b67006fc65753cedb80722b899ad1bb82fbe2052608637595f76d47b659bf23253e030b3c9c1e039bb486ef36f552" },
                { "es-ES", "80b1e41bd1175be2ea9531d6fb9c10175e0552f19f6feb5ddfe6427e2f3fb2ed9c88b1125e6eacdb3ab03d3027d8a2f0d78c3509896c69be7a2f962dbce748a6" },
                { "es-MX", "93039f69841bb1705d5b35a39806694a59078cfe599b398233889cc58a079db8628109e8fdfefbfc8b6a6c356cada56583f3ca989f4f4f2445848b6d5ec39929" },
                { "et", "e36a5412d66e4eff79682b8d5a2a3d826bd292a6f5ea90617b8c70473d191ba9ffb4004d11504aee86b77da2d73ba4c593a6caadfc55d6b91caf4289ba0ac39d" },
                { "eu", "0d79127110892a9d5bb8681c54fac5a410decf65b18a050bf2b86dcb7be9178b13ee1cfb3f3136de56312724055cd51cbc595b05028a6ba4044e9327f5e4709d" },
                { "fa", "5492e78e12ae5c7da7d218a3a30177f74a0049b8aa5ff775ed586c45123f866d7b644aa6f4ee0d4e1b8adae51161e643a80885070f4b29875b114a2e2899a43f" },
                { "ff", "82114ee12900bf4daf601203cf40a7e2cb20280c9424db6a7a198dfb562e81d6e72a823c44cfd475af8367b5c2323da996ed96a5d905f97138c7d62219cc6e23" },
                { "fi", "10ca8639810ff30591b8124ea527b694260e6ed77a7abca1d984023cf6106c23b5e362d487e1666926eb06e1a146ac817f8f887ef3ff8a38e2fa75f2231bd56e" },
                { "fr", "12c2fee107ffe44256c77b6f36ecc741469befaa025d739bacd3cc69a01b2e479515fe30dd80be31e0cf80bc20e26d3651c29f67b7d3b4abb8f416b2aa675796" },
                { "fur", "b24ecbb654fc3e4f1294323d8bb7980e40ca533e586fdb7d175f5497fd050c1f855c2ee35a30cf39afccf02667504cf29181589b5f6e37a1e393e3e1577bb7c8" },
                { "fy-NL", "f02c0c981dd0063d53c7fc0e86ed8049301b41b57f0d4c30fd6c4c2eeeb5347b8b33ca387170c57ea145107f1a433cb8eaa4742af6c3834b65a8021bb0a19e69" },
                { "ga-IE", "e19212064d7f07dc8d2ac1a54dbf8bde3dc596f9a481b317ca50e0b4776debc761701c89a2de738f9624c88962e8a1f508d0be611a8e34cea00c82ded73b6341" },
                { "gd", "b3c8a201c3eeb200e7b6148d1f197e686613472cf98e96dedeb56f1a58a743c8f8b64b63dbff27688dc232e4ea96ffb4772069205980b90749045d8ea892c064" },
                { "gl", "1a3439fe17513a2ea9949c95bde93b3f5805d827e62f7481b9479220b900eea2e26af36f820beccbcd06d053c82dbd0c83b9307809c4ce2bc2b9545c4942d974" },
                { "gn", "047befd0ef8d4f7ca2e371b9e5ce391c06019c7dfae3e1de9e922cca59f925a0da4eaaa1fa4754965db6fb4a8c9bebcf3c0c28d57d50fda7f1315353f16be336" },
                { "gu-IN", "021c4a98a945044c6057a0be75467113c013b2e096a8d213874aae6585bd637d9167184f9172d5679e9d333a5ec987e2bd0d364f252fe7015e90155b6d3120ff" },
                { "he", "df2d51c1cc26f30d1753a21222dcf6ea4b368557be05ea14fa6b8505b84695de9e15886ef8442db16947cc81c637c36a1c14344df4bea83ee3c6bbdfd1690254" },
                { "hi-IN", "bcc4ed8b3d7f881138425767bb17697dadcbd57884fda704a6577e121c8b8ccbf7c0cf231fd721aeb06ea1f62027de1fda739f4435ca65331ad6222efb1380c7" },
                { "hr", "dea06ad0d5804077b42bd65b5ff148e9437e3de1d6e4a17d5afff3d6fdbd070f8bdff9575e0b930b63ce34558d44a00190c15cc71be9e577734759a4383221d2" },
                { "hsb", "33bf13d382794d572a0b92d2fd8820c1bec047f9334b2a62f9dd70fff8c7aa0c42ebba30b0aabfd0fc17931f4f91f825018224f1d69489866e9e818f4863f6b3" },
                { "hu", "3a4de32f049377804213ed14a6688fc042075d483e24914d7911eb6a47b49df5b132bfd63e8499f2e9992718c4cbf18c8ae59cecaff3980290ca3467c6259370" },
                { "hy-AM", "516b2249317344a3c6ef3c925534696c8b5277ba41a61cdc4c565855d7f39104ba0fa7c344144352a45c79245192b8d493ed11d76d329352e7ce8f1a58d9ca63" },
                { "ia", "bbad8fd998358fc170f1ecd748c3512eda525008e727d641d5305b67d41e617df61c0d927baabf0aeac0763f733a350a154d18611fa6676e836e40ebdbf592b7" },
                { "id", "e4ce7732604f55f233ca6d7f1135a40976c4582a77db7c26ef0fb474ab9c9562e946f03894383d90f616c021d77cf9d1f21a70b51b64cf0ac3c70e4b0a9440e5" },
                { "is", "27bdf7f0c3bd9e6725d0c0704e7de429ea6250748e4a6571c57c01f765a6c91a620a193e8e258c8a92322dcc73675d0dd24b3e32817e23e52eb83e75370c0947" },
                { "it", "af91dde703bf5ab93b319d742cf0c5fec51fdfed400e5e1b86aed7c05024afa707eeaba42b365453f0ff79aec337dbdd086d2e4e110dad9251ce8abb84bc6ba3" },
                { "ja", "e405efa1e7d8961abf078dd6f4cb18c606913ba4559366f15093020bb399d42902a39c2739b26e79348c2dd46f376d4105da332ad69ff3dac0b69f18276a4dcc" },
                { "ka", "0e23e0692a1cbe4f1a6c64e00a4dc015132492d65e8e54c196fe7085268b150097c86180306d813dd3ef63d23394472e70a2216d61b9998f5dd554611673805f" },
                { "kab", "256ef187f2e03ccd8ce776e5cfdd590440ad37ac0ddf65649ab594b52a60df8796715fc302cb1f3675dd373aa9fa7741e8c8ed98c0c83fa115c1860533125db8" },
                { "kk", "b6d39eb54daf761d0ea9554734aed75b425efec9d54fc6a851cc43e4ed311c802c34bc6ae8e3af2ca7d43c0e633aaacb408664736f06d3e7cb4853e549a03f35" },
                { "km", "2371dbe228ac5733d20649826accf8c27fa0e1796158a6923afa5ea96e9f7813a7d9f02be19c3aea357dbdb7a1e1d5796df4f0d3d1e4ead7ffa86f6e090a120b" },
                { "kn", "cc33e11cbf34b6225db3f02541f478d63d51924e781b145fe2d782ce8e2591ed8d0fc361abf6f9c80be5370e715451d10f1e9671d9c5cbc4bca24e3066d09db5" },
                { "ko", "c19768a7721ca2f67d5889f846f858ec1e40006a7b731665d89552aab13e475c4bed4605198bed4d9d280218ac267c441f975c084a8fa2187560f4921288b724" },
                { "lij", "5fd4d40c0833f5f71024b95fbc17c231028d118afaa19a2d961f2d6ee6f5a88fe11747ce8ef3be2560c22dfc626e9079179cd5a2691ee92e8d91128bc5957572" },
                { "lt", "f94b64a399b55950940e966bcf7f58d490f6bb6b486b98f5be103d6eb3a7288e5d446e0d46c291b12dd510f2fd1bab3ac43528564fe1bcceb3e60700dfac5d5c" },
                { "lv", "f84ef0b9fc72cddb42a54a8c6e59eeeb867be35bf8f87018cb5978df06e9e572e1b55ccfc26f9f7abeae21cfb6d7d6cb2030f326eef801d2c0e5c44dee9b020e" },
                { "mk", "418208cee6501750931678488474f364c9ebcae4ee21b74c06a89a13a27d21caf0dc12ccd670472dd070999559d09b312996aa964090bb72e9cfc0c1311ca1b5" },
                { "mr", "85a0fa50e404cb95f7c7dab407dc8ebd2ee6519a1a82f2cd9e6a827fd37a972b949669b40921efa1b13c799b9a7c5f753e77c7c417ded895eee7a9018b80f815" },
                { "ms", "72025f7c04749a199b8065bee958196ba8a96031a9ff510c204cfdb25d0980ae8a3aaddfa2db24c1231a4be0d2ccf3952e1c36dd2a7c0ae707225a4829995183" },
                { "my", "58f9976d5be93b49172c7aabc90134b4152e758dfe5b85c6a7dac9d89c9c3f154f6164a01b642c5f0a0a86463338d8e5fda60255193caa3618200b58c78fa6be" },
                { "nb-NO", "7a0ec315721e7032ac8acf58bc477e69feabe8758352a5e66b7dc113d8d6154efab965040f37309785dca6da532743db2bb7508d86261d2777454649a6bd9798" },
                { "ne-NP", "cb8fed543f160d0962371d01146c6ab8b51f4c0e9a5693df406ede64cb359446518080b0900b9ea129c25c60f2452791abe06495ce86e3f200e139cbf936bd8e" },
                { "nl", "1eae0890aac6f482b3cffbfff064e73b6640493d69874c673f2f4e70b042d8169dd21f9c88b7ad4bfea2df0cf18b0fc1896db6cb0a881026e322a67380411acc" },
                { "nn-NO", "fd1f728b8fbb55e5d8840d93211da24915700b855ad6483ac817dc446a39a4cd39a30690ad1d6c6b90ef53aa0f57c7207bbe1bf7ed71e7fbf5cd620b7e6add48" },
                { "oc", "aff899ebe49b950c5ed13f313f9646b81964c58f78873755ff034ea5168cfffbc96619d88a08503e16e54aa1084a92246e7816a89de7f0cb55ba708b26754c03" },
                { "pa-IN", "3c62fb9f0cb743f0d95307b002d61411cec5858873c8d77717e409211ed8157676d7fadacd42e4ed5bbfbe132ac01aa233e20171a574185dcdb2e86c09fdee95" },
                { "pl", "d89479d4499072f216408610c8ae89068eb863f1dd6b2fc0db5c88afd5a0d347ca87a426716e82f39d7dfb95d203a8644f62e6ba2797f0bed863bf93264e5c5f" },
                { "pt-BR", "2ca2706f2f7fa47c86f45782e129143f9e3668345cb4c7041ed6dc9ed4762f5d551845779c51081d165d52b35cc1be5dcdfc6a66c7e3ff13f5cff81d56691031" },
                { "pt-PT", "1e88621c9b633a09633ac549e75a89fd28d539d5a59cdfd070a8d0f89cb3c545f5d87be5a8af5a890b541a5b9bbb4dc5417ddb3e5415d9ca0378caa469bebd7d" },
                { "rm", "b1a5c8c7217b7d0f096992487a3a4b88cabda12036181fa9487d9f8a3b2739868051eac455c5bebf41a085c365199dd0a896aafc5bb3ee106bcce1d522d5531b" },
                { "ro", "1c3cca41bf15cde27a3c6e065f677cb641b7d02aeb3dacb1edcc8418e7a2830dbf038a667de4b7692600313ad63e8a7670132e53abbba48e886d8dacf4165c16" },
                { "ru", "0d091ebb59b6815ba87d3fc00451cbc8fddb83ed7b201efabb32e011d54ab6e5dc4e1e92354c6220bd58a03343a82b981e98df811e558681308c7f1a78bcf454" },
                { "sat", "6b36db7a1e0e80034a401b7f44ec3ca42d3997fa4ac98fd93fa86f87a91a45b1ef4371204da96234494ed148964337f8c8f104999c7274f3d4b05e8c045dfe38" },
                { "sc", "bc1b1508caba4be31eb0dd612c2aab823da60d1ccc0d9d4ace29da50918a098d0cb97ae2fcb113a9f29e31fda2c3b09a574e015ac0e04bc50041a7c87eebc441" },
                { "sco", "2e7532f1a18b2302b79d6bb1640d1abf90fb08648554e49408e0ca5c99a8961d94e65f1e72632e37ba5fca0b3e1948e40a0aade2662086f876526a11300020aa" },
                { "si", "194e11eb6abf7cc1328306c1e9259240b029e62e0443bec336bba5facd23ff54f3dbb6b9da89b82f6bd82ab697e352b6c08c47b58bd64fb17ac2d4864c5561de" },
                { "sk", "062910beb06985816a6d2abcf15c6d15d1ecceb9d93ddeef1968ae2c3ef18ef39b1220dd07a4c791671291b1c50e893783545bb01912a58611c4e4ce44bced25" },
                { "skr", "16abaddac78223e9af1b528294d3da81637fb8569ea8ae9d5d7917002c827edd3dc290cc2a5f339bd3b42fced8be04be790e1f8a0b617cbb377f13291f2e282e" },
                { "sl", "9950d310f0cd8320a7ff40c9e6b094335236628946a04a56ef1a565f0c673370943888d3e3e0a73d7786aae70a43806092028b9ed08e8b8b82ff5cc130cc184a" },
                { "son", "2f678a66e3223d7294a0aa6bb442217d51eb2903ab22d15c8b4f9f51e948124367d447502544d6fc081e1d6529c8c5ee53201268a0f60f157ee7e0f037869887" },
                { "sq", "7ab93f9328780772f204644782999674369105fb38593aee6d59636344d1281874fde5ca6c1109756e34d6eb3fcea4724d8208ab5b83239ce5a1e818b629a2b2" },
                { "sr", "6f47a7cda6727b850e0de286894c3d82d8d1dd7b63c39d998ed4b64178f07fe5b64912eca70385852f2158e3335c83bee27fca966b3499767740e9221781af9a" },
                { "sv-SE", "3d8668d8bcd8f2dca17ca3823be5739aed5e1e9a107dafa6bb1094f76417a54be4be9985afc9ae9aa1ac74d0b25f8006b0561634be0d293c87f785538a55f4bd" },
                { "szl", "63c013f048a5ca819080c1ea0865780fb34881e1bab8a3007e5bc4b3e7da7527e3737e8948eff42c2c548fff2c1ca86f612cef959fe396eb68069ccd8daa9cd5" },
                { "ta", "1ebe41f951e7e17f71a6217d081e13d09696af52219c6bbe0084fdb7479339f3dacec9ef404940160da7efe2a89a635cd11095f082174cd6ea89ad4e6b2edc7b" },
                { "te", "43c25e335fbafec4d2a8cb1061b76f246b07edfc5f17f7ab29f6de4038712705688eb8d298383692bcbf582018c81711045ac7227284a4cd6b71a4aa7092c3cd" },
                { "tg", "5749e9aa78a3fec5e487086992d6bd838eb41c79737fb42a49e3be78f3f50b1c43fb3022725f2a45a67c3888509b72bdea07c846c87b8efef4fd8ed0121e46c9" },
                { "th", "fb44b8a3dfc9739a7b5966b94ca58c00492c0f1e30cb85516668929debeccdfb6db19be8b5a99488551ab92a1710ca0747507a9a73f5a63fc74d996f864b7f41" },
                { "tl", "b456dd04c3fd00bde22e763134db3f9d0235a33f1d8731529ed339bb709423471ffb060e9970e799a04b9ce89dacb67a334b515f661cc4eab0b9c965c96db1fa" },
                { "tr", "2d8de01aad4037cfe46a04f187b4624f30534da708f78354b8358c8cd4f1c4d388b28aeb790800f4a7ac65ff8840b605a5bf9f5c6e48095ef4734d2f9c326a63" },
                { "trs", "20158047ddbc8bebe31c04eb8f35450e35c2c4d55337ab978c490abb30ba72d92a8702ec9988717af1c96a2371976a9f6644f97b939da08bc62b4fde4171333a" },
                { "uk", "ce61c0fb5a5edec6e6a712f3056976c09d97f4ae1f24297c72d31423b43172b4de1baddcdb83007cf38dcb73add612461302e295fef9b4dfd18309192572909d" },
                { "ur", "860ea7eed95b766c05f1f638bf7ea4355bdad58b39140184cab91b0617bd10bb39bc583b4914f2f82710b212cb154f18c95351972fefc044fcb1127e631a5d46" },
                { "uz", "e0169f8b99af5915d7444e56919d99e9d012b7cff03fd83bccad1aef9786a6eae659ed5a68806aa2c0e4bc793f2264f57a3ce53b3946daba2ef212151b9e599d" },
                { "vi", "51b9ee60d898bb15a18652733bd639bc747636a8a7c428854d6da3c2cecfc9336f29ab8effc2f36612ae29957d0934f5691d849f2264765f45971bd590d641c8" },
                { "xh", "cab8bad329104aa0bdeb8aae87c2b80004ef9f5bff47c462e4b589cbb5bebecb1e287b4f6d30ec9ef9a03b672edbd25e231f70278349cef1a196851fe7cf24d5" },
                { "zh-CN", "62663250b6c3df5d229173c93b795ee37fb810b91098cff999838b81a8c126a6f7245a06bd21313b45d76c992b6d65413ed700eee75e060cebd98dd66f38f559" },
                { "zh-TW", "0f69c67caeefdffd9bebeee7f9b5689e300c6cc9afebc8269f062b87c6c9577e93f5ae0ae47eb101b160cd5313bb011acc21bab3738014f9d1a062bc83c438b3" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/136.0b7/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "15c56ce189edc1c5af28c14c3e6dee9884e3eac5eba87c4fced5e4f461cde145288877c70e38b32c0092e09a964d679518103e6a4564412983e7ff1b43473d22" },
                { "af", "7293acfe1c497a5b4b2a73bbe5808cdd8a175722a1e39e831bbe5e1d5e54b27ce8aa453dfbd865c108fa1206d37d3eefd5635bdfdcad9292e5fa2f9702db9feb" },
                { "an", "3667f5771565e786d0b6f39f6668de23d6360341e044e838917ec8c7c0163d17bd0fa9e55fa868d7e3a173b38ae7db453467d3ad626bb4ff512f931cc3f03713" },
                { "ar", "0355a04999005529743c5b7e8a9237deb79ccf3859fbdb0f8733ae2c292bbc075378f3dd6daa6da69160bbbb809bf3e1eda88f3a20c94ccc78010727771ba5c0" },
                { "ast", "bcbcea799a82e8a326169268abbd489a9b8fc19601539bf48eee45c0b405a088fd640c8841a076cea64d4b0a730f76673842f68e3a53fbb65c71e3e096547031" },
                { "az", "18bba4c9943fe09ba7ca25ab966988dd3c0144723fb156bdadf9568c27772be812379abcc0b37b15b2824953a5ef157cd4c83ce9ec7557b0f9eea5ff4c0be272" },
                { "be", "275304f0f3d95bdaefc860c113e3a22983536742416ea098213175e3f189cbee5aaf8f0067094deff52c56f3df68512983bfcc0d31bc3527406e916b2de8a550" },
                { "bg", "a38d483669203034c5168f47ea8fef1d108d36244662170c0dbc3719984c7e834963f140e381a8ecb141a2577a3817e25abc550d931bc0756cc3e2b5a4be0a11" },
                { "bn", "11a77b19b432564e5d274fdb8ad5b8b0934229524e1dc967f30d5addfd674df3df2f8fb0c94728280a5e29f15d16d00e4da75ecb1a67da0d61893996da1c430b" },
                { "br", "c083a7338bdeb8fe859965f3b21358751b2795c82f69068a7c58fe42024a5037f9a8da1d9f60c0f5b36f092789fccf6e6b04df4841ec1da348997d6e017b9edd" },
                { "bs", "03e035d266c001509e00b7994bb3cfc906cf90f9d9cefe105c4087579fb9781f36cf61bf76728743ca8d4d810fb32cd0569bf7b5fde5e78ae3bc11fa805a6696" },
                { "ca", "fa4913ef670337a7267ce5d3fce6e53f3fc4e148ba4376689e364df460e2a21ffd53d2a98108a7525197e28830acec5df8c2ab2918f61d3c20e3054527871026" },
                { "cak", "6e041929db4c2844149294b7bdba23c5704c764f7da94e0c2fdc830cbee9192c02012fed66491ba93c1273f3856daa84fd528d08255df98530089a980dab4dfa" },
                { "cs", "d4dab3f89ecfa2c438eb514e7b7f33b15480e0689050bd3bc4cfa214ed7de10c8b0d0a85ee2dd240c72ecdc23999f680aab2dc563f6417a38c49c333b08cff38" },
                { "cy", "cae6372b6156bec840fdd1cf30141bf6513634e3afbf5c17cf3fda4e853a50f68eb76460676163252c0b03bb1eed23fa2fda2bfb594be4ee3b6b3bf5570f14cf" },
                { "da", "f7900995c9ae2e7cae1e9c2149610c7faccff4500e27e92382e5acc2ab0085e1de770c2ddf0a4d1c545e2de62087924e171cbfcf7d598d2da5c76fc90a5cbacf" },
                { "de", "dc4afb1bb6abca840d920dc3514999b82b771c297bd76c91ec980262fa7e3e54345bb36a02fafd3485b0742409b72c9b130948fae0a32855142688a41ca6a03f" },
                { "dsb", "309c6bf9258975018aae7426efa9c6547ff0d93bff000dd24b5dbebd26ca2b7807813b0746aed6e2c81811c0a152b0fcd5d351b0a58857a547475040605ff709" },
                { "el", "65b49a41a3e3c55f2301c9a2ff305380d740ab2754531a3e375d57e1b829c770ae4b11f33d04f9223d33c49c93052c89ebad7790b501f0b8bfe6988ce229bc4c" },
                { "en-CA", "931fb11373052c093dfa947bed0702dad31193005fbfcfe8350961710d2085f8b296b30571881fc4d5d4900da8007887dacbc8683cd1003ad1ba4b339aeaff98" },
                { "en-GB", "c9194fd3886ff39fab43b205467832e1a85f95d1fa9f7e3bc587a20a914a92c30fb4fe1b617f951e109b7fd358eb20fec77f37a676e95497ac017fa7ef58be3f" },
                { "en-US", "ca7e25e36ed7bb4e498cb3ee29e9f20d4aefe943427f7c0d1bf689d7a15ab194ec32ae33fc158f5c06d63b3d6689c948bab57859f716d36f46451df7a5c3f52a" },
                { "eo", "f7fa583f16eb968a36c47ab35552285c14700dbe31da35d1501578b3e65c4d6b70473f0ff3e2d4cd582e0441e2379d4d59ab55c4f364df3fab54d2569d84f1f7" },
                { "es-AR", "a125f592595d08a3563b52f3658244b1d077906ebb86e0677c5f2bdd2f10964cfadcdbb8d779b630ef773e87ea3c2e29297b77e809959df09165620b4ebcf4f2" },
                { "es-CL", "7e6ff7e5e00f848041d352d462780a49fd684c71814bbe808743c1dfdcc9b9844eb67b57754e4a3581c2705a834d58438c22321a0d35fea168d0443496dcdcf4" },
                { "es-ES", "a4b6f951c8274dd597d627d45c4fd68899d372201bc894a4a890148088164f318f91685df79e45e15f653fc0f308c908d5ebf5acd431fd1f88d42ec35b2c25bc" },
                { "es-MX", "bfa2a504f2ae1c810b98c4c85e527828e7a83d88ee8e2d990b0e1726bb4cb490881a47739cb60107af609099bc4d0fac1a0629c930fc47ff040b0e4a3d52f879" },
                { "et", "f173ea518043ebc77802057d6f0fba71698c876be6308d33d44e7ff61d5c4104555a5165345a9145be5f58360232958a10c5d5eb3e0321fc2200fd57561e5e49" },
                { "eu", "f5b462c8162e107f543aacb140172193d8b79e1bd67c5462d905571ddd9977cb315c0c5190020eb42e5ecd1d69ed2996bb988a11c6a8614ccaa6ffee8a212a19" },
                { "fa", "83e546c2b4577602ad57891da8a009003838d4fa2e5674dd464499b2f00634485321ec0bfa3afbcf720c1cfabe4f9f165e3405de458b5e612859916fb1fef728" },
                { "ff", "0a80605165242de23625b6c1f01c43bf5282e7a632f46727341240d932b6e692b238dacf139032c89df96bb6778be61d22bb7b52ec54c90347eb6a5a71392ae7" },
                { "fi", "e1d1d6c727f480235058c0aad530194d281b95eb3519e847661e388a51d49b4e038bdbf6660d5d1053dbcde6cfd192f55769d7ef8e6c1047cea50b3f5a283d8d" },
                { "fr", "73aadc9605c182de179c9e1e02c77f342ad2e65725eb8e9962c08399c4340e0e0ad63043816adaa98a005edbb151988898d4b0dcefeadd8b9fa52367538e3591" },
                { "fur", "66f29275ddf619f2da7c7312b73bfc25d5538c685a35a6846f2751bc675ac6889424a017308a770bb7a90d7e13ce27b5e048bf7f926a76bc675382e876307391" },
                { "fy-NL", "2be0c2b544aa7607137778155d2cc90964895243b6d4fde936f9c5aacaf3ffe792dc62144c2ad4d5567a9bde1c1cfd72fc87806b463961970ea7112c0be17694" },
                { "ga-IE", "68c8284c8ace6876b35640334d55a4e224e2ab2aa91505447a6641b1730c16db8f7599241dade9b268c39b99f74093f979cf3e49c40e1bcb28424ce97e41e12a" },
                { "gd", "b5e323a8a2bbe9486f3667e12db0e0cb3a8ea39e9d8df35e1e9387291b3789e01da0a754c2f9d24a62804c418915b86fc3c0017c31c0bcf431526843423015a2" },
                { "gl", "a2bed6153a99c894cd3ab00e3abebb7327dcf43890df5bed8fa5211928dc14d1abeed388f957b405482e4f42bff5369f1dd142616fbd503ee8ff11e41fb38600" },
                { "gn", "6845f35f1ee6ad87483bd4af5f346a39709c2030ef4617a819cd7446ab3044392f8e04a48ca749675ff7b6997975c55bc8d35f842a6f860701966ab927c8ad9a" },
                { "gu-IN", "fa317b23eb00a8a43a80fc0278b6311b2a6f834d7b845c7c802bac9bcfc787b366666ee119fc234e0e63f3242b237f794fe675c8e52599237d99afaa8d23ebbb" },
                { "he", "4f5f0f65063665dc8b1fbec58baf2d37064661193b7f5de1a7226993cd6041fbdc5249a200b5045343488dd13a15519ac4be0f2b10e207362b7049250c9f139f" },
                { "hi-IN", "a25d3759a7fc8c321af4fd4386757a332edda8bf8f55d497b0c8ef442fb6c6820f2b161b5ed73ded897717818b643c5f5e267d8c441838ee0a5b70c02587ff1a" },
                { "hr", "9d38b9e9852d44e31a59b9ac67c0b5aa5838ba92b742cf6e528193ec6f9633d92039e5ea462ab9d1149a767296d076ba621f2c50418d71e45b98f21ed9011b29" },
                { "hsb", "35dd9cdf31996fec389590dc6d62118a2743a69f5be9fed6cd67eb69c9bc6cab5f682d0661e2262b764fb62e8bc919f27b9422eff0df61816ac68ad6810a5309" },
                { "hu", "e20d4dd77951830b17a847eae8124cf7c8b5bf7bd9450f8a60b0edb32607f4f187add85d81cd48b6531dfae3ba4f3cdb697dd06bd9404094fb1ae0a104b3eca8" },
                { "hy-AM", "c42f72f39df42820a27edf4d037ec1aa891d478852a20f047b1ab1a7629475398692b08ea72cb69a359bb2937e9dac6ffc85110dea2c5f867d6379b6ae6fdd6e" },
                { "ia", "fbac5e6d8418711ece4716f79e8a88a98c2e59a4e15f0b1a0196d804a1f6d04cffb301e761f47ea692304c41378b6ce65930e8c79408cbb9c330b13b001fca77" },
                { "id", "98b2f87ef742d6dd07b699178f6bdd72693c39fe7fbf02a7990fa96f6cb248f6620c987caef6fdf8c30502ac195e912372afc7e91a91214f0300c227495feb79" },
                { "is", "3f7f8ce6775d9cd6e7d321f1e6665c9e146521708c98dda4699c7d71a24ee03f3f1e793142646f4d8a037f82bfaddca443b43f2a23f257085e14bd77da42bd56" },
                { "it", "1924ee3efc15f5ede8878aa2abc110fbf17bfe5acb59e19cfe49116ea4aa72297abd47594fed1c192d9889c142fa96b4d7cd9bc2a704f46c68bc5c47316c7946" },
                { "ja", "d9b85e815ff96ddc087a682d3222f1b5b3ce366d87c69ad1a03a188c4fa8579adf404cdeb42798c30ac347f864af21f6bef58388373ae5434f8f6a40afd8d561" },
                { "ka", "c8705b60dc67964f005f54141b809954de8b49eacbdde0a3718475cc3df8fcfcc1eb05cc56a9ebdd2bfd4c8b00c3260cf64507796fa248afbd9b65327b398716" },
                { "kab", "d2f5aa5bf9ab28e9075bdcae2f5c323be0b64689eb9483b35362af167fc9d8ced1dfd91dbbc6e770c7be1e255b715a08506ae51c11f1f9ac70618cce2fe3c5b1" },
                { "kk", "efb0ed46940ef51d31de51b51cc5234e31b881ed450b38d1ae4bd7f317e5a5987ef230acae829c07197c7ba3813b66378585c495bc3191636b032d71650919b2" },
                { "km", "88422f21dd6a68f8d1f0117f74ff17531e04991105e883ca3880e2e7b71d4896299acd2daf7415550c543198ac58097b8f66566a1d947183f34a39758587d13d" },
                { "kn", "1da0ed315a539fb7d337381275b351ff32152bab7d81e4a1c9b021aaf28b85a7647f640c8d8f8b68a706fe4a929a740d6b35202c34c6e90b025c1ffebf70666f" },
                { "ko", "a7c83d83ce233bda55e82275b96dd4e628ca90f44a62d66a53fef5fea379899157f31f4086629356146a19f2bf5a5d902f95f954b5498cd81a5b85f09a727629" },
                { "lij", "182f1b37612ac903b44b5eb72c38fab0028291b31ca3c3f6f2299a46f4fa79241c19d152c1d7673f7993ca548142b4410a1ae42c497e6a6d16a93e7f6baff61a" },
                { "lt", "365efce5e6090cfccf0c846cc7aaa16b963f37b2f4017f349ab6b9f959f0aa18671a5445d93f909ee6301746025fd686ebd56310d36d1857d5c129c321b8e8a2" },
                { "lv", "d3f6a549185c7a3a06019c5874c304e5ab62097261ec18c26d35e84d435a4803c7fc204400612ebfc85d324f650884c1d36cdbc5fe8b1ab35ac707b8b5b1f154" },
                { "mk", "3d480b0f0798b1a7b78bde9f6f729461f6b77f406611ef6cb1b77a2931e55719a2f9d03a58ee3dead732bbca2d5ef31611a65169dbb6e9989efec50dca21b705" },
                { "mr", "9bc8ce1080108bed492902c3defadbb37e1a4a86f5760e1573a863ae06c00d0ee286acf14ed654b39bb47d8c580f2eba1a44924d1a46f8459e8c5b7b4c8209da" },
                { "ms", "57e86377b049393b5e5025e130dde8025fe779d1c5baac2d9936bfea5469861aa3bb91c9701eb73303ed2932a01f50a1b24a70a5bb3f45f29680c0bb53c3df2a" },
                { "my", "ad0513bfa04dff711892cbdda5d28e3b9648f87fbbcdaf4e704d20c2549cf890d86d4411a2313d29a6a8a9d67497f69bc7bc9f9bdc62388e86793e129e2b4c5a" },
                { "nb-NO", "ad4f1f56c2a8480b599da5cb66c540eecda7ffb91f988a028309b0247d21b4439edfdab5eb94c3fa6a94eeda8d10be447b4b9c74ebe352989b618451a839aa6a" },
                { "ne-NP", "ec0b10a9e26d95406a8ace6d9a10640556d7c328056fb9a1248ffa8fb3f29580872262fcb0ba75b6a5291c35c9d430014c75bcbcbd29b9e2808fcfd1f393ac96" },
                { "nl", "a011b64247f5768b96ea55aabeb100fbfd91ca92cc74b7f5b34796f55c2be753e7275884485b48aba9c83af31a8291db4d34d388482ab800739e9c1723242f61" },
                { "nn-NO", "4e7cc871e82119ad40d9fd044476250157f945ae9542a00576397d5d87b5dce8c250369db4e2c5b27e5003ebdd1d236d257ec240440491e78ba342e9f0257a45" },
                { "oc", "7640ce8de0e1835d6c67d7672a4ba29b71e7fce091a8c4ccac1f149555743122cc3144a7cdc80d3561527d59f3bcead6855906cfa7907ea4812ab5bcf51ea68b" },
                { "pa-IN", "491e0bca7bf66203972263f5eb359e1e4999a38b62b8cd1d4cd1d28b69aae339351d8554d6bf90b09d0e3b62f061aa5f7ec188537442507353709e54198999f4" },
                { "pl", "7aec64d51240cd7370a655076c86c10fc7033694300665b9bffb6f52610e4a7b5c71678bf7d210f97f5d13548774aeb0b085779f46779d1c78f6e7b6a0db9e75" },
                { "pt-BR", "b400326c835651481b775cf41d5bc9d146af5f53489b24d879e86597807fb9fa49a7bfaf8f9ce569b42ded4494b4cf0ae8c0a9717e7ebff6d4bb656fb80b63cf" },
                { "pt-PT", "cbdccd55c827aeb54a6cc9ce7f25ccf20301b6b22861142f5b2e96468fdc3108033df315cf0715e16437dba1907fcc0a3dbad914ac4de1c0d1f77a05f04272bd" },
                { "rm", "8faf0695e600a552e82abb1eb578531ff4538904b63f792d7834bddae0b46ae26253a38867cbbbde3bf464d1783a7981da2d661b8de0a0174056d0ec840f8d24" },
                { "ro", "f824e8b0808491a86a5ef9fb27f17ebb59ee81718e6461769a2b78651b9819fd32329e660093408f3057a7195e323f974073f18583d5de9bd7df154348f5e2d2" },
                { "ru", "ac88c41df8a5b233a526f8130dad34fcb798ac6f33d05d28db63a8bbac135026bf4fb3276e42c6a602df5af3cd191ed01433c57db59c1efed8898ba3fac92331" },
                { "sat", "a5f88e98c5dbde1c06031abc7513208bd0d94f004f814f1b13b09777940fe554395f08723a11a534cf41f1e4d0e81c27f3552b96e94b9b65ff76034b5cc58e76" },
                { "sc", "e99d85d75becab82fb4b8d05a8a99d99f708ac1c2842e41304323c1b8984b78d1470447dd603a596b99b033bb7cb4d776a90b6eacac5220d74b3ba2561bda75c" },
                { "sco", "45b649b70350b47d17d56701a281e29e68b990158774abb129897d4d3d745f8a57e5141beb26291633b68aef37e18658d057c9bc493c80d246c459e902086d8a" },
                { "si", "601cfb0bdabb414031c81cbe76d70b00404ca870015449a691066267a988f3ad64b439b4a7c874be076517b45e5e656f1c285f6d12be665e14e291d1433ffe94" },
                { "sk", "1b351e4fc0b4705d5d1ed8ba53d8f896a0c1dc127663bb0be334a38373ea95c9d6235707f62d922668d6f2f71d6343716c93d3cd9f310a3a46d4e568dfa6a559" },
                { "skr", "7a87b375d99f9e7605172a5c46431866bd230c4052bca71a2cad31e5c5bd6b24a9cea9df6e1f48b38d25d1ebe0877f6b9198f6f444a89e9c2c93003d37803267" },
                { "sl", "1191f4be47c576009d01681ed40c125d6db3742fb62e507872bd350bc61fda69497c1fc570f059c13332dc5dbc4ce85f02f4bf9b1660463483c6af9ffab96eb4" },
                { "son", "34447a8f7fb82b3e9e88687a6b79ad91cb33a8697f40effda58c8aaf3017666b7cc521c40dea03e087bfb65e7f3c515034ab9d03898105804663ecfe323a034e" },
                { "sq", "3fc37a4357f3e1a498c1e054146673439be1f88c01c51b3dd720141e0a9d1e187718087d0eae78e839bb012ec69c0cb85ae572045f5e62d8499c2999519f204e" },
                { "sr", "52acc32d064f202f70226151cdd585c5551eccda962a20d3f65793cfd350f9cafe31df47d0af79f847573af8514177eb490510568d0c6d8782f743ac9ee197a0" },
                { "sv-SE", "f67bc5c3312f58f12c8770a5953c85626b04e774d436fa13a67f465bbb60814d6c274bf7e4bdf84c12ff0a6ca1110392a9f4652e80edbe95ffacffcab39e090e" },
                { "szl", "c5f3dbfe438fae1cbe7d758558daf2d343e5987bc68b8cc9055a5e8e3629e3cde38b0cd291283e510260c93a88c2d4cd6e814e89b9b47f5e94d6fc34a191e909" },
                { "ta", "48125d6aca1b8f51d3b6221ecd8a337da6fe52b0264b646b1972aff40a237dd2efe7552282c0f53f60e5e677b50b70131c22d4a05f49a9176fe1c5683a21e769" },
                { "te", "7b882aec04d842cbb5a2b569abdc4c0a0ceaf7fb33d28c916428f197b8bad49d0f813402791987d70a3c7bc3784a3f347d960668f6752c489508c7a3c26877c3" },
                { "tg", "ae8d1975f720a9cfe9349b6afefa4b09a27b08e117c0d6360804d078ccfa6c12eddd19a77dbae99aab8587f93198e3c0f58d3bbcbc10ce09b1e598fe8c2e5fae" },
                { "th", "0101f58c5d13ea28062a0a5e73e39d5760b64e4fa4e0fade8e9ce9bda351710315b48dc819d41a6aee54544fbf3379df50192963aed9fb09143304b49b3eefa8" },
                { "tl", "2fc5c98b87f91ea19cc8536574ae6657a563bc08ecf880e4eb43fa2585a5451170e7a29191568dcd3f69daca5db2c07fb26bdf2864780a6acb4fd4821b20f52f" },
                { "tr", "d32f04fe50b8d2cdad96618d2bbcef2ac19bd67f756f2929d658a7958efafd850ad28f72fc75bbc6978dd7cf552c3fe9ab9ad7ee50acdde9d329cd859cb64764" },
                { "trs", "5abf39aa0671d0fd63f5355be84e916263c8e323a6be0ad6cff3fb7af7148b1fea8dfd181e7cb5cb5bb2b09767bac3c3aba87551c8e90d7cac162f1a662a2521" },
                { "uk", "16edca221f0feeb0159c9547cb9415aedcea9fc8bda320fb58e489e6c559e05cf2393a018573c26c819c3f99d390092e63caed7ac222afca4dd49d34cf3ed4c1" },
                { "ur", "e42db2eeb82dfca9c348ebcf9e529c76c5c8f6d332287ad3b63355422c5d28bbc38794c22ddb91dca6619f81eafa0a0bc9c3a695eb652c4e6f882d696f54c68f" },
                { "uz", "47769323c087f7b90a646763f743218a984a51f797199e85954061db9f1970e2516d02e19ea4e62305a9e719cb7577b7601f0284bee2ca8230aafa0e88419484" },
                { "vi", "9b08dcbfdc224b86a9a0cf317b2602bc5a10b9877117d9b4d581f1242aa409bd8515fbe62de5b3e00e73d88f18be042223dda1ff7f77945889185a5a3f7f188a" },
                { "xh", "2af290a85500d5763c5150cb5e4fe7dffcd40aa419aa2f21ea2f3c1c761348bc3ba1f6000062073271c8d06ecf85b963016bab9c6250a6531c3f5d214ce35be8" },
                { "zh-CN", "ee4755decfb3511fa3b8b59918b0838978192c1e106c9a33912816069eb0d5a5d688e7613c6ea8421a3fe41712b2b1359611f640d099472b87e7e3ae35d63330" },
                { "zh-TW", "8731e22528c0be71edea16204e92a1ba690ec4cece264ef33f827225f239372089b048ac4ce3b258f8f89f06f0fea224052ba2f5f2fe9766324ba591fdc8ae01" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
        }


        /// <summary>
        /// Determines whether the method searchForNewer() is implemented.
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
