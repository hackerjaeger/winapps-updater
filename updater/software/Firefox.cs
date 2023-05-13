﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/113.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "666776f80e5e96159caa4eb6dd95139763eb39fe2c0818286c630aa6a96da49895daebbeafb1e9333ff898cb2319cdbc96526a887e316b83058306e621272a04" },
                { "af", "1119b7b136517bcdef8b98641629264b882d29d99917f06c8f6bc53f0fe9d0d1bc0e91f05b0a13c7cd04d9644d2428f7a2da3fbe77c4aac8b1e99b07f91320be" },
                { "an", "220e7ebe72b34d328d461d8c53b2aa373bb46b9da1ef6e01b11ddf4865e7c30801d1542bdd448ac1ef3ae765e5ee7f02f52747dbf550011061d30905c43cf8e7" },
                { "ar", "92b4808d9c68a2fa9f053ce47839878c8908c264beb9a64807f140c5855483a558b16b079b5cd1eaae591c2d20da2692bbb0f8bfa364c45d91f69c1ee3666c71" },
                { "ast", "09e18c2fc3b6830da406a259d87f2be6dd9be2ff0fb842238c2c77759b62516debd1537bd9e0b8c8f02e676ee06546b90b7aed402305bafd99128b473205831e" },
                { "az", "574e1f6582bd06dabeab22dc7b461b70621c663373b12638711405477f677b2e4b700f26a647834abe12c9db189bf2ad14c1f13ec8cb975aa1cdb2e1a039dbdd" },
                { "be", "8e71467550405644516667294e6d695ca770eb5c3f89520ce13f9bc9a27844d492c4277812004d377301769d8179cba35515db7f500ce8319f28580bdb3664ca" },
                { "bg", "bca269c35b11bb379cd55daf1db5c4d7a71d53c86679daa9b023425ea864ef02b38dcde96921d2a21b482481b9bd05d392c05354794de84eb634cc91ee303eaf" },
                { "bn", "5597283b6098f59ed3a62a8acf8e5cf63c9eb25347d22adca09eec6e60a78454018c14225051a64b48b3f1bd6154ca634e7ac2fbbec3d87753ded3b3a8f6e320" },
                { "br", "fb2065183c2b08f469e135049d639046fe83c77350c9183f052b769aa9f35e2134de4fe0ca2c6ea3726baf015b45539888fd68d672653077e904d3fd75211cc9" },
                { "bs", "9f1dcb141c65eefb3a57e3a8c210c15fb04098d5507ba0e8def328e589db41a2a2ffac4f890845834e60f3d5a46431bcd1d28cf1dfa6ce76a2ac82038d158b1f" },
                { "ca", "6addf0d31f14914c1ac1992f07b39fc561a6139ceb7f59091f8995fdd9aa41683c17a20996b8f6c15b5ffd4f3af106649fbcb44fc71125b6bb0861d0b3beb315" },
                { "cak", "ed3e8258d46bba95979220573f1188507a3fc235b8f3f464ef779a1994e3021993697d63b89f7e940d5c0c87775d2f008d676abeb55dcf8a9edc47eeb5a634ca" },
                { "cs", "03096d9adbac6504fd92eea4944711907bec6d48789e6da3de2876f04605cc2c2fc5bfbb4cfeb7caa9cc6ae990a0a2ed80acf4e67d485f85659e738dce7df397" },
                { "cy", "5800b7fa4edb5ef9884cded63ce11d30ef646c2a403f8d3188c53a3fab2b4e744a28e3dcf548e0490d7b0011b2ba9567885ba822c1e4af077c3b576ee81ccf10" },
                { "da", "feb0438a37c85b0b38b34fc643c462416d65267c71985a09e7325e8ad28be092c9153f884c98a23829618b27ad8d7799346fbcc23abb564c88506c462f35cf69" },
                { "de", "e1c3809718b4d762779794b10f9ca22e8057cc55dab2130cadeb10f9a6253b80d6209d29c653440515727c152a1f1e1d0a3e8301706b4dd31d957ec2fb262c31" },
                { "dsb", "6c1ea3ca640964b190b713763dd047f41ccee98b801af69c39d07391e3aca6bdda5e77685b42ed2cebc934b07ac7172a3fdab1108225b24a750428a8d3a15417" },
                { "el", "d43d53694029f7d6cfd557ab7b69c1291eb1a0c81cf718fccfaf2f831196ce86ef4802c76e5a0b87e25151d00599dabf5f5f178a0600c438e0d8cb9e0a1157f4" },
                { "en-CA", "509bd33c5da74480a0c157713ee89cfa40c3e8e26dd76d8d968db63a631a3168fd104c6b3228a5cdbeabc1c762e5fe973d849b2ebd4f4d529f2370246c88909e" },
                { "en-GB", "3cd83cb474a72edd0849963fde4e8ac6a0541f3ca234d712a9a4f4e6420c4076779a477e6cb9663e9f1368056f149c126edb64efae3684c019b048ec7824d50c" },
                { "en-US", "67267d655104b5e21e8fbeb6f3cfaf9579a12c7373e027d7c88c71a3fc773049ead45ce0addc66f1301034c85dbf2fc413b90abce9a9f7ec3b62eeaa81fdbdb0" },
                { "eo", "d7c7b40c16f171f5e898c86db6e8e453c7c64b0fe46ad662bbc8fadf67b5995dfdf9d6ad632d032c31897f36aa97e9f0d69609daf7bdfecfc5413a1cfa8c3fa5" },
                { "es-AR", "841245ee62129ed370041a213ac27d312467bcd97c464c9b61ec080a0af2c64d51c0121bf946f7b1accfab64f63219709090ffd11183470687dec15a440b7ee5" },
                { "es-CL", "a085366471d2029b3928b4cd47f9d2746aec89a4e31b4c3e39bf962aa1dffe64378f1da95dcebc6890665c28543c432008fff3c286ed15fe9933f803193bc531" },
                { "es-ES", "92ca288da093bbb53c1a55e310b67a59c022c1d9fc40ab94df82e00c3b526ee4f7e39d80b32470aa8f73d87319f3df28ff95169607b8dd7c56308728ea95ca6c" },
                { "es-MX", "7b2ff6af698261d6762093d603fac0dc79e6aec8a4b311dbfc077289c17c6e25d2ba5b0733d138aa135b1ba535b33b749ba9a5f49739c980d5bd7356b0b69ace" },
                { "et", "6a823c041948499078fef723ed9542a0d837614ad283a138d399560111f1ffb1852ebe09eb2f2f7656e2c35714ef1df6dcd9bd866ae3c86b0ce33c7239160561" },
                { "eu", "877c70b8eca2a380783fbe6a74d57475f70653d484a24ebc7acd9d5383985b60f0e733d9ab95d2e3bb6b068798d6905b0882e8bbf72b5ec786f05464f44d883e" },
                { "fa", "579bc52dc5e884763c8bb2d321bd945ad8f2ee9c9ee2bed3bf0df78fd832d45d2258969f668aab82d746d798bc5f738aa9227d853949a0e3b6eeb8d4b5bd8f68" },
                { "ff", "5236f9b3b8c84dce9382133da7447fe8eb2fa830d4070a79826001b617c2629ecd89a6af9a49339da2dc149a8a11ecd0d928e32d54f2abfae3c958d31d8d98c9" },
                { "fi", "b5d417c9f9e4a53d8ab303380cda23fe917734b7f654d7b3ad6b40333a1213a7fb8627b4d100076753f530ebe21c0792b1b28686107cd8b1e448ce8c74df15aa" },
                { "fr", "eac3829d9a25495358ddd801dfaf9cbbe239f7fefefd4b3a10e013b1b72b5ab897f03e35d2db11d562d7767d97c0915e8bedded37e7b05bede156936c2a11a1e" },
                { "fur", "49d982e9d524f0c8377d29344276af1b7ed189fead8c3603f75f95cbc8b9bc6cfebdf7099f0ea06bd374fb7ef46ac0e05366576a341c5dfb7eb01fd735479a9b" },
                { "fy-NL", "b0690c3176195f3dacffdcc77f54038bd6f7ea40c181ea87eb8539ea0793e24046c9227b0590f2f7ee45dae23c2636eab6b2b31e3c0ab21d5e0409c965f95b57" },
                { "ga-IE", "5e12b9dfddfdb3cfb9f86a52902db471d9cff5616dbf966204aa11e6e0f937bb9fa498277a1b59f9a062eedaa5176fa98f38001aaab0865c1ae97a5924652e59" },
                { "gd", "50e92e4145e61becca980ff5f3bc7fde2ed1c34edc050ff24413a1727f1ac2992dfa7106f0da1ca5589c83c98227dfb3532ed3d2f1ca5e03558e7064beb17023" },
                { "gl", "f5fbbc9384e9cb3d4c1c3fc288523a409702c31687e5c96531cbf900b84857a7063b4fffeb639fedc026017208a15f1227ec55f274cca6f4a699ddb2a965a75d" },
                { "gn", "df93707dc0ba8e964db380bbcfa3c2e7236a6a5d0183e1a5b1a59239483703e8081a256779b6267c9e985d8602456cf7cb723eacc7d3535d9a61356149ad503a" },
                { "gu-IN", "90f2ae937b4ea6f491743892fd8031da2884b85d27d8f9ee161d02ea492694c3ffa60b4c12b87d8ab7a193c88c60a2a84a71f122bcff28f88d4afe88482ef460" },
                { "he", "4d6570ac7defbdff7a7966e86157a939199fbf649641a61c91d4cb36dc19beccd611893326d990a25aa904e835d7560ac2ac07b1f7e91785431edce0b823cd57" },
                { "hi-IN", "4c870e3820157837012bfae9de695535cece575586ae50510dad7429952840737ca63d298449bc59ce44717bf53db760d1db4c1a54fd564c24256b168aa0114f" },
                { "hr", "73a46c5acd017e9bc5745036e385c52e6ab4fbfb4c52908f942b5402a918c1f6d48369e553c9205ac77c294804352e9f48755b607bab0994c1ca149d2903d99b" },
                { "hsb", "18788a3dd39bd9fe6978fad799247d6f656f7880e053899602e9bead3e4a4fb32cc88efbbea61163913ca15be52ae8e654b363c5f3a1783a8b7f8f5f35b6a4f7" },
                { "hu", "6300ff7189b6b400db03c129d88d092dbe42870d12b0cdeec4f52533a1badd26fed6eabc72c064ff3b5601b800755978bbd44928c346781e2b38b2c9b0db916a" },
                { "hy-AM", "5c97ef083976d69f3f1f3395f209c5621630a82b92cf602343c8da27bf782d0d2eee5adf736b500aafd77a6f19e7cdce8dddf2d97e33d71bc1bb3d0a1a3643dd" },
                { "ia", "fd9ccdca560b87e77c904f469b6191b8b617fdff046d739ed33fdbfe866d5c51801540b08311d223e8ea610551b175a9afd054b81df71b44cafe5bee870b7176" },
                { "id", "88b9f9f6daef279be47ba9cc258ee986adea0ea10a3ddb69e66ab5bb6aaea8fd67ee7e0a9c4ba9b4bb2d5a830d396708d2f2fac1a8e24508dbe6043621034947" },
                { "is", "872ac6f9e47755367ac9496bd7d549ed209b5b082f345196977811a9eb3aaf5d8d7d70be7f98191a24f95fc7e448aa9c11308fdc2cbc87092840904b495499ef" },
                { "it", "e367aa21e7e1c0beebaef4ede719c5d32f4814221427e816f0fe70e820f6963b1cf23c798551520f5dc933b9a0098bee7c7cfa1ba74870683a42516a8b5ccffc" },
                { "ja", "04bd7bceae236c1668c97b09a83cf57218cda1ffb6c1f73f42073708b164a3ca2f42e3c3aa0014ab219330e8d9107986953aa9d3a7c32a2cdc3b2c5206da6c8b" },
                { "ka", "ef568afe29c885b9c0a5dc0058bee3144d19560847bf200bb3fb744e709f9eb1b4ef11f96bb6fc1dbc4503c1ad0a1b7c1c5e870c90c49f91a90a7e1a9cdf5506" },
                { "kab", "7b4a069db6eb8e3a07b74f18e43ba889b23bdabceef36bea0b00baca311bdbbb1ef743417af0d0c494633d827025c0eb2896da828dc38bed7f06d5caba3d8412" },
                { "kk", "73740f790586aa710b263dd5350055f317db4adebcafffddb22e00b1b63faef8d016ba446db470f7393619917b9d694027fe8c38eafc81fdbf368e419da3a358" },
                { "km", "c38bcb082d8497668ce10fc73bad5b5c88f43ca34e72139abc4cfef3bc351947e4fc4a4773a67f257adbd9214cf41587bd400f68ddb68492122320ef49260151" },
                { "kn", "f0c0b5147a84d9175654c96b00942f5091c7b4fb7b3a6d3fb0367a5fb9183a80e380d31e70b4dfcc58931f9bde649053676902ba213ff9695c2b33cfcb5beed9" },
                { "ko", "c8ce72d69dd60091a874984ec6027f4dd31434b6bb22c97fcfcc8807e09ca73848e435b1e378f77b0b2f1898c069f3a8a02edc3f593498db7d5593df0b7f819f" },
                { "lij", "88708b4db56c0ee00037a54bc84332ebb886c879636fbe96d33247c524a780b886dc72ec5fbbf3f842efdde5af6dde054e49d4220bdeb5485ab386292c822d47" },
                { "lt", "e0eded79d822c7d781d7786447c785798ebecdf033ba44a7de93f7d4bdfebbf5a7e1cf70389a602776cf69bb5351296c17eff9eed47a935e8e267a94bd672643" },
                { "lv", "d69ac966baed774d11717062976adb83c7bcb9e0f4cfb593deb64738e6c53685896450348ff45bc97f2ddb0ba7219e81487bc80a8fe0216a5f239a33f81520a4" },
                { "mk", "fcab998351a071adc81bc3fd226dc8a24a5e11408b03f850d92d24afba4b8f93b67bdcf6d7c88e587edaf3f5ef1abc2c5452642378697d103e4e7e706afc2174" },
                { "mr", "c727e9e72083cef80dfcb160c68b4a54167e9d9965a2cfcf67e385a2ba294aad26e1f79610578659b2ea1166d206a50b94a7dcc9ac7334e183ffe7dce1d03044" },
                { "ms", "fabd3037c417641f5cda7a6c60f0710dae6e1b4ed129adf91205863495c71bb54b125db5de4ea9a888bc28d5288d0b13741adeffe0da4aa08fee6d33243a3112" },
                { "my", "8e9f3e5d61aea97d84918a535782a179e8cd06fc0f5816e821c818226f6b7866f930e4363300d1d709aa0b6678278338c436925bf1cbdba7302b67c2e23393a7" },
                { "nb-NO", "2bacd3eafcc063281a26fc6b1b9f886aa773813ba4dae4bf9c7e856225a8e65e1324b13ec270ea047e9bab7281f10984366b040a50343bb617d00fd27746387a" },
                { "ne-NP", "2962753d04e5e98b1d9f74b25a12eef933b7da9382dd9583771033be13a3b79cdf776dc39b3eb15d9bae544122bf6b6ae63603583e3229355e71a32bb921e6ed" },
                { "nl", "ccb552c459226c27f6ea82d47ec464e98126b6915cfa0e30c1aed85ae5a380bfbafffdf534d2518272d44137fa00e19c5b55359058030752bc03199d37f62e90" },
                { "nn-NO", "4d91184d9a92754b60190e8471349f47dda69b7dd671732f978dcca1de4abb5754b1669a8b07c5caab968e04bef38fe8a0d2b8ff81976444cc7cd5a215c59f93" },
                { "oc", "8f76b34fa30060aae1e15b528f94b3fa0bdff50734d5a5e93a305cb3710e5069cce480b1d6a16e90b1fccd738c0490d24a3c2fa735a062379dc131ee9e38908f" },
                { "pa-IN", "9f7ae014a95447bf3ded13768c11a868d6f8d005d8186c7be8978424b89807ce9f6e6b30cf414e5513ba459e121ae3bcc2c9bb13045ff7de034b847a4a5ada45" },
                { "pl", "14f3be18ce9508a4c134b8136f12e3dcd50d7596ac69b24735ce31062d970c410b5eb6a2884de72d5dd167b1ed7f44712d0d031217d1bc79e943eb975e2810e6" },
                { "pt-BR", "f1402853a6b6fe0d6d526063aac9d242da67c100fe398436c3f33e8eb62dea878ef429b578ff2ee6c3d6a99231d24fb2486698ccff68e733d54db18f340da14c" },
                { "pt-PT", "1d2643c47553d5927f9665b2093fa5c48b94f18966236d9f0def4ab9a3792c1ec391276c6627018ee0441da9403afaa6f54850f3c96d52959ed041f119bdbab6" },
                { "rm", "4ae7ac4d1f2bb8f83df65f63fa6880226449ad613fdef5f48c5984de51b6177d95785dbab85bfd42ad57e2e20fdc510d79f469c16a75d25ddcdc44cd49bc5e75" },
                { "ro", "d1a03b4f0b8b6dd5058d8560b8ad7c7d0e6b1977c8eca0fb956f8804add5c603817e9dfb89152b48ac4a8d87382ba73f0bfa65d031a6c56ed50811ca66fdd66e" },
                { "ru", "6afc4dbb63f2cd055beaae07f35289541c359eb02b07e5d5d1e3ade9960007f045558f7800cd3d32c46f4c0deb84b8cff888d135e53d8238a458f8717cd02f7f" },
                { "sc", "008298ab13bb0224d4fb77cfc4ea5aa7c50d2e5b4654912883c4a8d6543519403d15e7c87c40c800a87732dc6ef83d6b438107745898e337f3a8629950fd0684" },
                { "sco", "c4d54c9dc78b397f02da41da79aed30fe51484e6a0bcb873b0c7a648e76d4bb9880cc42d7d8ce3a0eadf236aad3f62407c7747ca6c8facc5909af24eb61ee9a4" },
                { "si", "46c050812d1d357c02233a77dc07a0f02e2dd2d0d2064a40fe9de499f2054579c6a1d6baccf88ebea8e88fa55267d1e95a9e074aaef54968ee48278735aca8d0" },
                { "sk", "d23f1ecb23c144ae0c0b29c0291a7a11b2465c76a09a946acc74c60cd40c507a668c4372a98c82a75fe91425c5fdfb9f1d7c995186d2d509c5f2afae4b6cdaf5" },
                { "sl", "234738cce35733552be7def0144ff1ce6afe7af465e67cf834ac27659f55c1687afa0ad874296720c1fa353fbbb1dc5473240a3b56b97d1c2d9fadf7ab022664" },
                { "son", "44f399748dfa089029946a8c8da545435aa31c01229fb49c37dc9f7c39c51c8cd5a36d97195771656502754ecd3abbdcc7219dfb5bb2e615c18341091fda91a7" },
                { "sq", "07d57c69b32e39f1a5c0553548e89ea62040752188353eca6d8ed9c565484006dafbd7f963fac787b158c2bdc01c9522976c2a482245e51ac668c77f1136b4a7" },
                { "sr", "5ffe0aaaf43e94f5bcea2ed7ce3d14012543320ab2a9b5bbf1ae9c161eb7232bdca94559704350965172b85b9a34085ef67e6c06ee383633c833fddd6d1e372f" },
                { "sv-SE", "23415327cf38ce4d92107c9fdf845400510c26190a5b390f1f3b45f10bae5e4db5d814c7010ed2d21bb7ab68ee24e59c146c833672801fa3f79e5364fa4678e9" },
                { "szl", "f1b8475d3aaaab34f357ea7cebdfde6740f0a36ef3cea6742e59f85b7162aba7bf808777f13c3f828bb45047114657b8315a9999a48900a9d72cd4cfc53e8703" },
                { "ta", "cf365c447e0790632e420c70a2ff59820dae4641481b090a4680673ac95f9764d19e637d27e47dbcd1c0b5dec413060a386e0154b2ce6db7d02aa4ccf5fbe3f0" },
                { "te", "e43001d913a7c9396ee6b8867120c1b56d8904e49ad89d16d5172a9454545c366c4e8eef341156e2df24b6d7cd2eba5b2be4fdd2f68df84106a17b6c6b39ab6b" },
                { "tg", "93554ec5d834f210e49af22d390637447e10d008e247dcfea54a613df5fb428a80d764d3501d1bf10745f8046a72e10884fa163fa17da113fc87f408704e2da7" },
                { "th", "a80e33e134685bac9495c49d473e936ca22d891bfdc3c37e8f1c3c79b5a3e0831ba205c855b06fe0c4f398b73e12b88d5f9eb2df0214f784297c555339898031" },
                { "tl", "47b63f02f08f32947bb62c18b484365a1c5b484508872096238733119510dfc4bdd7119a46777a2862fb351d0bee5c59e315ffd7bfe02c13fa86ff21afd326a5" },
                { "tr", "ff4b71f8c348fe72695ed864901c6d57d61155e8b65fc81b377212de647b12fbbff7888c496e787883d2f60ba3d0fc0893bb72691663276649bc6f6f735c482d" },
                { "trs", "5d5a1ccb67c139adc1726fef576f9a85383386a4c63063ac5105a1abdd7775fad806bb400b3c824f1a4f679b62dedbf94b51229982169063dbdca8b8fccb5803" },
                { "uk", "2511dc681c76c615a10615ccbdc4567a40e8d8bdd49ef3c7ebe8571b5893404bdc371188fbe090a90b9900607754dd1251ae10722418c6e563b65b8ceb3cb6ec" },
                { "ur", "8c9dd6a7d762964ae21e65469a24dc6f59759bb42faa1a50925ed287265bcc2ce9bce51a594b010be343f4456585160b6caf5ebad9632b60d487aeb4cdd2f078" },
                { "uz", "d6c6ac257a193385dfe1fbc3a8af25a0297b1e07c5dcad28c829c1604619dc501c06882f2c1d446470e5a24e927393cb2f2f647855ea9eb85a221e284ff3efd5" },
                { "vi", "0e54f837e39e8e16c189cc2a0cbb3ec15b6e8897b85f6076d78315de0e74e17f1a05bbfcbfc35f1d4953405819c1e383578f54278a08f181e508cda5a65f9c17" },
                { "xh", "2a3f143220bb4f697e6c35b8d2c078e521d6787d3db0fdebefbdd3ba9362b3a360adcc1decb9a1ef29bd861688840f06b20ceed2c1072484d981c705b6030992" },
                { "zh-CN", "7fdd28644f64a477061f6ae72eadcde08f60f7895719904c677b5ae630c8725564865aa7fb380b6deb016c23bfce2248b8ee861b5236376c13f3ccae28544920" },
                { "zh-TW", "36d0f00f60ef9ed98c6e29ff5d5578cc628c5780eb481f3c424bf20948dc24ae4709b332677213da688a7bbe7fcc206324d6f55750b21c66183c1ef48bfa69d2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/113.0.1/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f1fe7205bc5aee930f2c65dc25bb36433d7a746352c7125ca63f00f68324b9226fe35c1de6ee1c8ad1451c7e3676b70cd3a091c6ce5cc5cb42ba5244e49b6e34" },
                { "af", "a063df60be66a7a9c76724b827270a5ef5180cece7df9192ac6263ada48a36cfea751e1e5b6cce3167a740274596b34bedd12d3bb50274fd6c612f37062cc6b5" },
                { "an", "32859a03e94308f7a5aeaf84d2dba40f3ffecb2bdfbe8ab0b1fe06b16f3aae3436e9700def901f9eb670246c1ec1778dcd4a7ef3f30d630b0b82757615a95964" },
                { "ar", "090896a65f907e2c896533cb176bb297fc48aa4949841ac0e30499a28033cdb574ba725f9d66284bca92089bc2e473ab71df82e92d65b48a6c3010e7b6ba3b65" },
                { "ast", "d5e2d18b4669b0fb0b857751586f048fdefadef895a1f043e0271549e3c98b3eb4e41026d29a9c5d929f7bdad08f967da2d550020d7b3cd8a3707765b0b6e2dc" },
                { "az", "96851a769fd387fdd5a295560214925240079de664a86bdc5ec2b3fcec0d4772bf1e531e62c595286e5a75f16fbbcdd0240aa36a960645d8e01b8e35e29700d2" },
                { "be", "dae2898741742e044bcf824948d3c143376585801c778ca7451e83156ccf893ae72c936ae01eaea45e1f8cf684d11ee37a1b0757f4bf6bd5e1cc6f296d2f448c" },
                { "bg", "ad5819fd872a530fefff8b9d22bac0c0929b4d17f867bdf7e52cd10e0888534ec6dc901d7afcc479ab0c6120979005f44a2c6444174b623e3b5ffdd7690b95fb" },
                { "bn", "fb672fce0ea770db70f08f0612efdf98e0adf1fc9f860be4bbecc3a5fdc10f4f1b472d3be0f51802857db751705b397d1744ea63d83354a3120902adacff81cd" },
                { "br", "d33cd28831398b58148ede24bd5d11a2b3b5aa617474d5e1cfe37858a9c37eee00e68749c5c57cc1553957850a4ecf7456fbf3a2a4125750838c98fecf8411a2" },
                { "bs", "d361668b480e76518ca739dab186a9640e4a537aa9d070f33b5832d6010ea8d528f52e7dc14c11966e7f3979f2a8d40bd01f1e01ceb17fca2e6187932e5256e8" },
                { "ca", "179c4dec9c0e5046fed84a5cc43737abebf30e8e0f704a346145fd75340cbafda5f8fab7c45e56644093a7d8614ada39ca80295f85dd462f12ac6a9d05ef11dd" },
                { "cak", "c5c2c055b25be0a50899b09da8a8dcb644fca8b42bb4ef1e3790d66b2753ec9f998fe52a0039be3497a56b9a0582830d94c4cb8ab5bd1a23bddcfe78d251ac1e" },
                { "cs", "d334d731576b15a8d14bae2066b72d743ef45215698f0a39f41edc0046e752e2878a519d3d63b026722c17450caf3583cbfe6cf82ea959d31c2078a4ba5f8df9" },
                { "cy", "0aee30d6727120ea685a19446e4d55e8adae59466be2ba08805d3e3df50f5b86d15e72b55f288c0d30a16594394898606a71b9e1c9df1c02add66d260acd5357" },
                { "da", "39f4c029673b53e315a7326c07418d340c6414d91572bb1ff441cc4c848b702eb19207ef42244bf87c7c834e73ee89ec50e573b154f79f48c2966bc239ef72dc" },
                { "de", "f7437c035d6f01255180ca41672e9b69e89c2b87363657b6fab9c764ca34b3f5ca67bb50a798d8940c012eced8d2eaae1daf4bd5fb1cda20f3bc222b5ad4a527" },
                { "dsb", "2a33973e582877f848d8285d1bff2f854c67d143a8b511e616e1b5f55316cbaef6e4e76263223bea805278b7ed914817ffcb3c07cab678b9ce97604c3e22421c" },
                { "el", "34ebb9c42cd6ee4b91bc3204cb2e9ebaca874cc91692bb0938dbfb4b9a0a55b5dc04cd9920c9a4e3226cbb674d98797bcf6eadfe1ee318bcb17a7d6cba8eaf45" },
                { "en-CA", "1829fc15830a7a7c01367ad2e27f2c04e10ac3bf16a0289ce18bd25b96c5112db6cf8b0c191ce28ba871bd57907d3ad787fdee4165d868c23538669e5756bc2e" },
                { "en-GB", "0afcb4803d07681ea7690a22e2e0a9d82e8e6d54b8e9cf3cd2d43062018ac9cd00fb5b0c9d609a27119c2bfa015af1761fc3f9d34bbf7ccbcd4516d8e9c02aa7" },
                { "en-US", "0cf818c2cfe5a06568a2ceeff57ab38d6806200c6be71ed7e1a249648a5c376d34b9ea484a231a8ffe61eeed05f099207d0c940f5374c8bfb25dccd9a9a97ec4" },
                { "eo", "a90645778b414807f436a5babde1cc88dbbb4f5c829bc8d2873b2e20c33cde2b71c75c4082a458563a9627a3d63980bd857df26eaf54d8ea45954161a5bfe48e" },
                { "es-AR", "728d0a7879afa260c853e0be85ae2025f5ee3af960b4c2e838de83e5e143e6febf73a312a11bfeed54ec9d1ff68224c279bef1c6fb8e64a8092c6d7783193d3d" },
                { "es-CL", "ed4bbfa781b156c0284883cd4100572a53476432a0908a2be54a522db3e783c368f19b2c62a679a6c875931a6976625f67a49e8e59ac6ee71e6e2e014db9c9fa" },
                { "es-ES", "7d653977991a249d90573b7f151a528c6db434d04af5ef3d10a6568a1d4118b0b9ea3d5cf71da155ddf2b5888847a2d82f255b74ceebdcc529d75e4b03596f82" },
                { "es-MX", "bad0621512387371e9c1c08238428791a53954bdfc4b5fd4dc0c6d5b55421de78bf16f01ba7360aaa6265d99f6a07806919808bdb23929667bbfc09dbed24063" },
                { "et", "467fb9b5b084bc79b6c1789bc0d12ed56168aafda76d8d28a208421577dcda5d8969f80cb5773c4476cf604dbb087078e87577caeb43d2f8575d3d799031d0e9" },
                { "eu", "87e852d47851052dd0280f0fdd795d74b33d5759f6cba5d5436fd68e4be49d151e0bf9dec24adae0d3f4e93d51a3880f1187bd3e48e24892b34535251dfaa54a" },
                { "fa", "f599759e254908c184aad31ba5e8d4608ef063dc2a21bf2325f3aca7673b6dd3c3cdff3779d3f8485f8b49026f4de7129919aa6f5a731976d436e25bc4ec9cf5" },
                { "ff", "4870e118feb3f6c690b85678562481bb02e5c7b4fc2887a62ed436efe5d1f7e1ff3cbc7de0dfba12adc7cf183a51b15668e13113ad261571bc848164f52b6d39" },
                { "fi", "194f2ee69ac7860f334ce2ffa788f30bf62d208c3792eb49ab21891f30cc7f810004cfcf2e80b16c96a25859cd135c8c162eab0a373810660f8d09c291197db7" },
                { "fr", "be5233310606179aa2b6efb0ccb5370951146dcb571d930990b75acd976b4e6037a140ab413e1fba923e089d4fffd9e0d227b853198d2820dafa10687747b614" },
                { "fur", "46de4de250f840508b18070f9a5eeb2222a6dd198952cdd3ea4af9c5bb832e334039ea32e71be38af701979b85448c8de33631e1a29f734fc9e73ae448dd49b7" },
                { "fy-NL", "9f05240400e9386fffd9b268a6b66ce3b57c52f7bf7e26139fb9afc3bad2ece401d9bea16a9909473509ce1683ae32ffa5970081741cd66f73c7fa250a1f1f1d" },
                { "ga-IE", "4a868e2d06ef26e7842297d2cfcc56f33c53f246247ef4b850feef99396552319b092f7c857dc0f1c7a4866f70e55b674a8c9a257cfc3e6cc06c08fb54776342" },
                { "gd", "707979ab0b546cda44e0995e3539d79cf081521f03c1662e02259f0ea4ca523f2b8b825f8d887af07a7837f330821ea597a9f56c586195f999cb74f40d4e13bd" },
                { "gl", "eecc5b6d839521b7a5aeb6e8387e0c08bbd8e069f976dba6dd0d213578a04c3a1634af9955db91649ad3c96ae35028dabc93800c6a4431d8a612b526e93d06ab" },
                { "gn", "980aa22095a7478a15753033771d06aa7441a942a48f501ae527c89469cecd05b08b09103a612602711374f39fe6b50c9a85df50fcf903a50b55f8c562f3d18b" },
                { "gu-IN", "59cf35ed3dde1a5a99486c3028fb4c910086657dfcc42c83ec62cff22f9e8d171c9a5b03bc1e6cb9314e3f364bedd0b37cc28bb2f015b6031593b40105eb3498" },
                { "he", "8a4104b280d5040f7da726d337a1498c26757a9133e9bcf9ae874455aece58921f733b28e4931a22cfc05edf84d78d92065756bfdabf4ce54d244dec360f99fe" },
                { "hi-IN", "afc9a3dda4648ebfce50839394a7d36461967674b1d67d52d07c955498c0788a34cfb6e9263e52e0d479a81d6606577e2d80271ee3b00b888893f91c3028f31e" },
                { "hr", "f0338bbedb317dd3ff13e23e67cd050a9d6467b038bb17b0aa411040fde6042c1fc01a3eff32368e51bfd61d910ff67b21e6eaf98cc5f29e346a1624f9bdf32b" },
                { "hsb", "2d1ce5e930fd5547ec49521a1254da851f5e95858bca726140b6f0a0dbd7243c0a70c457cad08b57a7c0bff1905a31bb0ce4659b3597358a1c5903e55293dd5a" },
                { "hu", "ef24b0a69bd7ae5d77745ca11a7596a4c87d4f04613bea770771dc81e754d4440925c2bc3709a25c11d6422972a8de7a594e02935f2b7de0234c54968abd2e95" },
                { "hy-AM", "0841607bdf20540ab5107b691a31e2c566ab93536af314a8faefb9e1b7062d4001d1788c0937b8e0a7b3a9cbcff0c02117d3e221aa6a616f07d7398f49b8fdc6" },
                { "ia", "3dbc57fb04c7b1512dbdbea66a2f08554865ad68e96d5c85a9354ca2d39f47caaf1e5d8372d84fb1b221b2d9356786d810aab79dcb96c52eefbfe557a4788488" },
                { "id", "29ed5ea9c1a76e2c232977c5ecfe2f5ead5b4bafd4f1c056854ba88016d0e41bf8a1e28fb5ee0c76ed7a6892cae5d6da56afd5de65759091914eae9dbbb2cb46" },
                { "is", "30bff66900e9b668a57aa625b37363841450601d601530f7ca3d91ca7c963876359f561ca689c7402f293ac18a9a53ff66e815328850b891a619d0ba40fe309b" },
                { "it", "ee130e431cfc6f42050b5bad2647769f2d4cc0e2716ebebd0a439347cfb4a4505ca088b7bfec71b14a17d057e4ec6769a4626c33afffec054c8a8f61c9f5205c" },
                { "ja", "1f0f561feffce6829d5420502ab4abc6ba1ee98dacd86fd4a379bd59957c60cae781362ee5bff7951ae0d993b4b9574a3e36a19a4a00747ad72993767cc06895" },
                { "ka", "ad0b949429005529c26258a680e4b1c8918f8ca63d034de50cafa6041d343477bde1b1ced45642815530ed09df88feb78d07c197310bcf5705b1cc218a4301c3" },
                { "kab", "ddc669997562d8d683563a230b2956e5da4d381677cd41cca852efeb91253c5f8d3510cf30412bc621fa56a69f9331c860b31e137bacf0bbc8178c38f420b2f5" },
                { "kk", "c39a9f85b7e774f1c1ec2a3a92bf9738a1c0c6f75ec6ccf6ca2c1975589201ea0ce44c78aed6fa3daf69f18be6fe2ac57602a24692e86a4c5d3636b6cfba99ec" },
                { "km", "3d67be87f296363b6b94ba75e3c8136ff23568bee7592810e18c9bf52c950c0764bd4aa79643db0b3d24c3fabcc696152c54227428cb1594ebd27905c2c605d8" },
                { "kn", "a78b8d9a484a810fe944710cbdb1edf0bff966dedb489151130e896ab20f3b57b3645aeb86475fd76e1f8fa084182e5ec088b49aa3e128dc2f129d1ea98c2c45" },
                { "ko", "120d94dcda8e3b2119c0c5312485df1bfe23ea89d8e690db0028d1129cebc560be041c982dffcbeda2c99537ddece63ba54a5435f90c24c1378b8d5d5424bfb6" },
                { "lij", "0e52d73233f2b6d4c3a5f147fcbe6f394e9f670b01a1c598129bc84d6982ef08156829322ab016e89259a6ee27561c153202cd3e42193fdd808deb6ceb64838c" },
                { "lt", "9873dda1f22201e263a7470aaf9ae06f55122fb5a422f1d8c0d4c4bad20f27eb617c8a5b5c3141aa7c75e988d26cc293367010dea90b5ccbf952f2a14d3e5493" },
                { "lv", "c967605089030b8fbfd4a9c47e27d063183a0710888cd59001adab1a9120ed27c7fe77b3173cb408dc23a04a361931cb4eecb980a9ac30ddc3d09887d051d740" },
                { "mk", "5c3b38ebc5a263bac9c208d7870b9b64b2e4731258b732095a3b9cf9aa2f982fcc58b6ba25557eafd316d1b980c984ed244ea1fb748893e60caa5e220a85822f" },
                { "mr", "33128781f3d59116dc76cac84a5dca58721612a0056bb37f1e018bfc0909852e72f5ea45ce33e884ec7b99d0d6b27addbe1864a7aa9ae07993bca0e3050bc9f6" },
                { "ms", "d8f72e136474278b95c0867fac37e7b33ee47221fd53b1cc09df8a79914b49e4b59a9ccebea8d014232639e5091c354dd4789e11011cea71e8b731c0b447b20f" },
                { "my", "b49d820d4862983cf7efaa15661adbb792df4a038f5b4cdc38738d9df362a47dca263f678146a143e31202cf3d28381de3532272ea2d3b54f908b5e8c10f03b4" },
                { "nb-NO", "baca96852ed706efb4777f16cc424c5df93aa9eebe5be695c89c7561caf5d106cc8638e9fceef86149d532e447e5389408f7c09757f13cba05064df5c29208b5" },
                { "ne-NP", "4afb62d525c0615905755321a8493dc304ba13e3736a50bda195c208dee54f060d4ed2ae78617f7d3bad0a69c7f16adea83c2c460bb1e84c1534ba38737afea8" },
                { "nl", "8206c3cb2d92d6afb9db16d58feedfe61a289d4ccc09caf512da357a247cef8273cd8585cd0887ecc86e4d00b9527be68724e22a6fade600d47155987e2f13de" },
                { "nn-NO", "939f4d41f617ab66619a6a80cd9bdf5b6f9b723ac4680a9e5d2576477c2cdaffa53133f84c9e3f4362bea86ff7880003eae785349b98d1a55568da205b6aab87" },
                { "oc", "cb03403293e21a4ae52a624ad45b779420f90dcaca30b14577b68f5d284d5b2250a5805322cffb42a65094928d604fd9528d02ae93a3db6f6c6dc57121172838" },
                { "pa-IN", "bd14f42bde24b6130c74741b67c481d178d9a9921d35619d94e66d4eefb2b761fd8afa6dc82826c7f29f963c88fc95ec8b75f798744cd65e6aaa0fc741177c42" },
                { "pl", "dc067a0e7ebc122e40ca3e9e2887647fe8023f22c3e7bb69a15b5cf27e64a339a5416e818169ffbcff6e192e728c174aaf343beed6eaa0df6706e40e9476e914" },
                { "pt-BR", "7835ad54dfdde2754bc15773ed359fbdbfa5f730a7b6ccd08506978670895d64004aaeb96edefd3ba605e2b886eb12a53e54c23af7d87b5b2b84e1f1c595e75a" },
                { "pt-PT", "0b365c3a914f04e15a1e16f3da64539a102f62f8907e72d98d709db68f5d5329819fcee5496f354ceda0b3752ad3db47379158f19291530703a0f7d22ed71aa6" },
                { "rm", "7e0a4b381cc2440afd3320ebdd97f0f1a74c90f6c11421ef54599646593c0db1879e1a60623e964911580be50c303480dc69df6bd685a88aa10d948b9e31dc6b" },
                { "ro", "866e5060a489c8aea79aeb2d8ee84f5d70cc4481e52ccf6c04e16cb2d59698ed3b851a60fcc71350e7a9f4f6e1f6f9fe73c79f450764347a092cc14325bb2664" },
                { "ru", "8f91de9f8cc3e956e4d55f034a4f75ffb6ea39a20e8c259122a94b581fee8ac3ffbea9d6edef44d0ef52ad181e57deb09d3ae51ecbe812175d9fca41caaa1602" },
                { "sc", "24cfd4a8fb52e5625ef9b0dba6ce538edf823a2bd584f86b2ad11895fb2812209bf733e8e391eb9ef50d9165b8c163c28cf1e105f45fd21725fd88791af3569d" },
                { "sco", "738c945a0d47b96a9400dd73a3f712d0ffabf53384762843cc4bd16ba7a6aac0889bbcdff136e0b7ce0f3679a193a7d82842d9b09198fd2dbfc8c0ac59b1ba05" },
                { "si", "a27031d3bb86d65f06348987ef23d9359b3aa0bc1cbd1d9fd7eb8c1bc02dc02c8210acae355ec62335b0816a0372ec08f01ba737c2b8807533a1fe72f6b3b2db" },
                { "sk", "fd966706cd3486bd83f2ea2d4089c36ae4b35733ac2cfbe334e9cdbb8099e54f7abc5de8d3ebc3f6be7d8cb3ebe95925fb3f981ae704db4c96dcda019a7537d8" },
                { "sl", "28444fbfae2ed8115bb2e5babbf4e0e3958d2e88b4ffa82747b01b3223615940777bbe39ec72f5815d0bd634c63c11de98d71ba51c02a182daf8c1133f02314a" },
                { "son", "2b66614710edb8bc828c6cb48325c3269acdf635bb5e60055a47cc6d71835e0d3564fd5734d23e081437220b0a6873f7a8fb1b0fbf3884cab3d65be60c68e7e6" },
                { "sq", "306d2533e9bf8fb178697d00e6b12f1eede9230341b29e143bc000daf54c1859207452c9ef9c6ebd0b1194cb9a550f827f9d52b175663d9c38661eca9b334c14" },
                { "sr", "2e7055a84d5f055e11f80e79a872e562b17415fc1e5b4cdef33a805cbe1b308675fd9cd456b6e863fb9e03f920954ad3cb3779c3c043edc347bd43166e929004" },
                { "sv-SE", "a670225c8305cc0b30ee5ee7bf06ad332b8a40e7cc59cd1201dd933165365c9e5790db20eaa548a84185438653940eb7f0f67a7ef2372b4848e0d8b3bce3cece" },
                { "szl", "85c43fb72251676cf1036aa6f526f707fd042b89caf823145ca7d522334d892098a3dfe92d209e7ba318764371e734e2b7e973f36fb167c5cf3a0f3061b94d12" },
                { "ta", "c38f18283381d312141ffe19f69ef34a597887246caf87fe191d80adaee34c2d4339dd4381223c0bcab1c0e9c785d9c8585afaa787053a3e6e88074a1092a936" },
                { "te", "adccc94a2017562963f2666bc7eb1c495dd4fa1bbc7b0175821b98904fe02daa21ab387ecb1c5126b41a1e90d4beeaf7442901b6d6a74b3c4a85f3dc584cd097" },
                { "tg", "2827bf06846674b508730af7fcee0a8e54d11351110197dbe2e8680306313d49f11cc271a28d0f43647502a8d5d52ccacf84843654033d0ec7030627abf4ee9c" },
                { "th", "5c43950d9f7c5610cc5ffed6a617474518947cf9310c5552c3633f86e72d5127aecbf8c78ea178906328df92f9d6ffd28ad7b49db3186ca473124ee249331087" },
                { "tl", "55c27a3360da981d8b1085abd7ce285b26f4d49dfb44dfb7de0f33c3f021dfa2306b06a7d5cd65aa9d04fbd1f9d2df8f934af892132fe4a612d59b5bbf062560" },
                { "tr", "9c1e96a63b1d5e0b43cfffbc45b6bcf173d56175613fec301c958639c642d6e3fb0b6a83d3fe9e5ec06056e9a341f678107894456d3a4af8c17130be41fb7d1f" },
                { "trs", "3c5a0f9bc5ca0e21126fd34f35b260fabea1c4b3e9cfb72fc9a83c739071e2bae0db73fc5d6e3fa2eed775575d34d9162232f6bc4e4a17c1d40418fbf4b6cb55" },
                { "uk", "627863244fda1e7329d95505a4481d7c67fd62a50fbd0d7b027902887d3ee94f90bd3d08b24071fc2a3692037fb8f699f1ebbb2b788aaa9caa8860e8953bf840" },
                { "ur", "70948ff26715c9110abc228a87ad7a4b3886b263a5459e852ff05c6a30fff2a1e713e00f54d67fb32ee589e68139ac27eea6724a7f3ea7c77c70099a62b4d3fc" },
                { "uz", "1d5fe9734b2f1874a7c610dc15d9eceb37547eae54c7b44cf149baa1cc4f31b0a9cb9559e2094e50fb455b44b46786ff414831210de859af472b51c703c7feb4" },
                { "vi", "f558a9b50d09e675401a0165e64ee5e5e754382f2c9a213f62a9465eb056bd45920c1ce505bd8ea5b032d866c068d99bd67b20da8b8ae1f9315076e7bbc8e9d6" },
                { "xh", "9fdf159a49f061683cc06dd1a0b1d30a6d1a1217b8eeccd834062082de1ca0e5939067d0cec1ea3d36978b754667c25073d8d627ac43e76be2b07617288ddb6b" },
                { "zh-CN", "22cf3ff0dbd4e71d018f3193343a7e8ffc8a5de85da8d2fa0ef15bce948f418e3cf6166038b3fd5bc7ae37091cbf8b652dbbd0a002b2fe1e3e7a57546b675ff3" },
                { "zh-TW", "c5a2c8e0689d9c735ebc126ba08836e32ec8d3d304b5d377d4a82dfaf802b5bca39b4b2bd7516ffe9e8c5d4ecea0b54cac4db1c71386cfca128287f918348c70" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "113.0.1";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
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
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searcing for newer version of Firefox...");
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
                // failure occurred
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
        /// language code for the Firefox ESR version
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
    } // class
} // namespace
