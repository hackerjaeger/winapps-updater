﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/firefox/releases/89.0.2/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "ec9873dfe7b24181b6cf69ba2a3b270371067c1a7953c11e885df20849811d81bff0787418a550c3671d77ef650c2345b5a869b6e9ab2803e2ce5ba0da82ea15" },
                { "af", "22d9533a31326d442d39f9b746b742b1a59e8c1b13723cb60b75f078770d5f051c5bdc628f8c513d96eaf11b988e091b1c7fa37f36d60d3a999f65c8286c7a52" },
                { "an", "30bcf78ac5ef5e75b721b28105ee88d1a95a0afbf53bb8c3ca999c5391027d76589d43db3aa07cc4adfdc7b10df993b2e46146fdb05895148e899a5c6200c260" },
                { "ar", "1de981d2e8b2dcd2b53c91aac47d1a14b15e148d09548efd5a3d25bef001e0041002a3a00ab8982cb2343c43eea52c86945af6509d2ece3ad9ebaeb904e425df" },
                { "ast", "83ce69f27907b48cd87dd53cfe48030b6dc52fd0ad73ba6b437f45c1fd5ad4df8f126051b33235f5a426a8c303e135e887d5d1d425e4ca4b696712403561806d" },
                { "az", "4237e69efaaabd1bde26c0e203e9deb02a89eb2b3db925ea66caf8009dc59a158b65623f35b9adf0b577e57740aa046cd68aada3e1364bd24a74697c8d4486f9" },
                { "be", "12713bd45291de8c0c4f805fa387c1f5c12f782ad4c90b0a1a8dd852f93f9de1bdf3c034cac49e8b14f2f5ad9359da2849e44fa9c90673bc9fef17bfb57f45b9" },
                { "bg", "c88d4857727f771daaaaf89760bfe3bb97a14221e9feabe3338bd6480af735c0ae531299393186eca5190301f40854bd8e4acdcfdd9f99fbdedac6d400d13822" },
                { "bn", "451f1d14242dfab19d681b335c1ab19cd3bd15ff171524bc4700a8b3fdb02bff1924357fbba44c7fc5c23cb5872595271281e60927baf3cc5c003a89d93f0070" },
                { "br", "10f543430daffbc5c4e98b2d69f3129ba7ab6b11986f0feea75a2c37596223f4d9eac34e48a577b6b3235a7d5b2b4a083975ff094d5c3f30826b95a0b998d04e" },
                { "bs", "60ba13ea9ea68bf81f75e2e75d83979e4c05a31bce28e39ca30a28f5af886e962c99b4920c54533c711e31ba334f2b7ed36efd73616fa374cb4c1f9af282a70a" },
                { "ca", "987966f1634e02994b3cb58e4e1e27b5e5c79da003f6d51640f035af714b5e47cbb696acbd25bf838e4c172bdc6c593b6b115c5dc3c46a2ded2c9933a7740df6" },
                { "cak", "5b90402c70c00875d46e7db18d95ec852485634152036ae050350222b4a4a76f9f2870cb32a9d8488317b4ae3675fe93ef90f7809e14c3811cb44367f3412693" },
                { "cs", "bfb2953c9b24ad370ee89ad65255b5cd061a5bcce7701eee26e84d6760a169f538f1afab41094e737e1cc75995ec07bb8b88d4af940aa5612a37ed74e3c5e773" },
                { "cy", "cdfce3e31ac208a2b92204e90af0c2e36852944a9406447140618cc5f20f376342a528d8935d89d62415ff9edcb535457bc2f1ff293f3a634455549043bd1643" },
                { "da", "665ceb0d89f38e1ad54411dbb76821fddd770e0bcf5d7e188238fa597579839ddc16d17b7a36bef162047c983adb90103b9ab471d55b157927c21a9051c7f09d" },
                { "de", "fa7639f967b9b83d88728ddc2c644e3b95997e6460703444171037c00fd5ec0d321ce5fd6d999732c5173cb4896316b5c88046d58437ba99106a901a0a081034" },
                { "dsb", "7c634799cf52c3caf9a0c7caffdcc755e3655e5b33aa612c739c74ebdb91691b46220c74c6235f7f6be0fca16f59721bb8d37042b3e7321f51bd931f313f50f1" },
                { "el", "917e1a9ca2b0464b9603cc71e80c743075d15bd06bd4ce9337455929b35d5effd97da451fe58892a2745f5ba7386acad6431b28f060a27f98eb9d9fd90e42e3f" },
                { "en-CA", "b85b6b402a734954b7394e39ea65f88ba15847a77fe4f23cc4d5f8b6b98eb6df5929beac0a28f9821743beccf5e3e9488d90627de222c541f77bc7246374c6e6" },
                { "en-GB", "70c83026d26b56c090642d187f048fb91441cf5b1e86e0d86635bbf50424aaa9b0c79a0c465a58b7d26d52aced31ad231f5226c18726dead70c61d58758ee4c8" },
                { "en-US", "8cb571fb4d1f700ed5d12d7960f7a0008bf82cacc7421428d78d5189cfb9010deeb36249ac17d6ee0cc25e4cc4756cf7b72f2d4aff5e48a8a46a1ea54bf458ab" },
                { "eo", "991b5dddadb6ca06ce9dfdb332421dbb3dc9fa90bb7c18ce2fe20df1262068e69c6d6979b47aaa46cc65e187a4d653cebbaf0b6548b10c3e38696fd3a5f3789b" },
                { "es-AR", "6fc4133e0c129fba551454b2a7e067e4810b0552eb9c1ab366263379ef7b17d9e71f1ecec126925abd4b33dd140b9fe05570e7d3f030bea0918c1999971789f9" },
                { "es-CL", "5734643fda6b2608d779da201fe56610bfe61f1cd586112360ce9b022a6822f528cb216fa7192895c89c31d50601329ca341127d4a98b26a489e2f17748890a3" },
                { "es-ES", "b7118bbf44b2898621e6a037aa10e119589388e912bac6eeb96dccb5549308a7d37c392a9616404fe401a6c8affcc60567ec4941d1cfbb308beacaad03362495" },
                { "es-MX", "0bd60f6caf126fbbe237ed86ec6fd3b0f94db545dd7cb42cd7de20b8cc26ee04e9f73856ad3fc9780cb3c3a2f333bdcb3c58caafeecd7d4c7a381619a4b9ae67" },
                { "et", "f05a495d70876265f758b5a6f3eed7a32b7d0fdfdbeeb4d3e8e35b8146987350351c17754cb1ecbaf5186dc414576fd8472ca321e19f60285130befb9dffbde6" },
                { "eu", "1d08d90dd2b69d09f35f0e56ca61aeff75b661438256c691b1d2f20ce470e3a4b6a35645a1695ccdae6356fe0032645496667714b7a0f0970a78fcba8747e4eb" },
                { "fa", "7dd0f5d1a5f4823626246312454f61df1e9ce92bb14978a0f180276ce0975c6221dcc9a95c4219b20414ed005b009076279f24e30e903ea1dbe92cf4dc3bf4e2" },
                { "ff", "c7634b4cdc5b1e049a962725eea5c50c3340244f46160d6d43cc327cdf9c98f6b2fcbbce31682d84aef2791f8c55cabdb76eff2a14b776ed7f179c1f303f9614" },
                { "fi", "4a5d6eaf6849b95d9df8391c0d6bf41a1dbb29b869abce3ead61eb6cbcb23d7dd8016e17f03884f17bde21b394dbf818df601cfee85f9c308a64bc7f796ced8a" },
                { "fr", "c0f9a6d7e7ddd8403fa2becc72ef2b23978afb273c4f881bba50526cae005fefb4afa124a476f2d98a519e538ad87ff6464db44c49fb7cd82b5a6e150e49fa22" },
                { "fy-NL", "0199ad5d7546b3b7bcdff80e43fab7c8b5475d449078eb01cab848dd0a96bd1f65905b472e8ca7b5bab2ba817501b15259dad2bf3be32972059f2c14c46ee92e" },
                { "ga-IE", "9cbae5a1759b2dce67a05990d1b863860ae50f4020e557a37e4350f119d20e26dd42c8eca51b261ef86d166c0b759698120bb48b38578c542253e265217f5ade" },
                { "gd", "6aa7c9b447a17a6728cde4bc67b091981feae23b92d3fb7af0353652d274bc8aeefe475a9eb608acb39a4625d9e3c7c4a9d198b141157f9c50c646840670ba27" },
                { "gl", "010501d19a886ec25c113b8a66ff3a7003148c85534a5cc7df46edc28eb0a1533954ce92181f4cb8b6f771335380633f51c81ae62c51516712b3b55ccee41dfd" },
                { "gn", "c8fc7154d22ac319560fa1aff60a209ebd52d9c6f84b045a9eec09cfd45f504cac8a89fffeba6e5f07106d90ff231f216ec9a8c96a813052c9149c7cf0150872" },
                { "gu-IN", "c687c04696a7fbdbab7c099dde0ea3caf906fd8d1a1ce754648d9480a23b8b72d63749362e65a922c20704b8267d0f8a266148604dc7d3c4d08eb42418bbd22c" },
                { "he", "b10cacc7ac2c042dafd6372cfe7d7b10a6f034e1d5246bcf6ab95fa6b42eeff1752a0a4c99e255b896ec2fd485d4172b662a0b4ce9882b28ac676dfb3e77acb7" },
                { "hi-IN", "e0b04fba8ad9d4d31f391cb51e07aeacc733f55c561f4fe7ba448e0fe2543dc19d0180e3cbef1a2d3020ca8016d9cebe4a870bfbe4905ac11ca59f7931645430" },
                { "hr", "013b89d1c1501d252c1070ed844f6a914a8092c504a8e6e22abfa9fb9981cbd32d0fd730777a5b9cc47ca57059b469e1f058b428adb23ce310eea947a19222b7" },
                { "hsb", "d68374e85ba7267c467bd44765d38124d4b11553df75ba384648563b7759dce78ff55f1358b73017e27938212b2656307fa00e6ae40e8df9d39bd35538f8dff5" },
                { "hu", "88be2b2a655e88cfbfa8e7e3b26f72e0361a1a71fbff7f1e899a62b625d2fd54731201d434ea99f2c94dd9fb8c33309196b668d06eb23a5b3268b9ff4f30e81a" },
                { "hy-AM", "707ae3a905241a106a456d4b4f1d3c1090e89737c3b444eb111f61069cf44ad6525adf549a39ca639d0894ae68d31ababec08eb127f90c7b4033a2a07cf741a5" },
                { "ia", "086c429dc4a39a2cc2be8bffe0491f9183c6076009e5244307bfa1cf9d3df790e7280333cbe8599de8c65d4bda46b4017bb084b9f162de881cc50635353db9fb" },
                { "id", "daf853dbc318c5fe34d9470d5904c2b61a00edc6b5a493ea4462c24c3d4a795f6cc1c9587fb4b310059ba00ae939f52694310560d12aa14a43ce2fff92d20ab7" },
                { "is", "fc96bbc201d79f2029c19ac47f683fe5ca488183c7ffd4a541397ddf3e6f881a31836bfbba5d18cb27755b4ef25647d4cc4818c272243c50b5d5b18617dea4b2" },
                { "it", "2a0bee4ca0e5be2503015d4de5508c5019341f2bfb8d51ed80aef5811534a9812e5f350929f0bd522d822f49d457e1ce502e86791139281122ce868940436509" },
                { "ja", "ac0cabae592d99f8c627ea112370ef9998d6c6de9619d9063da37755658614069351a5707305791e53d7ca6dc4812dc45cc587090c79989be373ff2bcf8c52b8" },
                { "ka", "7855546596b8fc1537f6230a40043d262d679fa506c7901749b5fd6bd756984a83de025987a0d341490669a37e64452d08aebabe268dccc57bcf4ad958344628" },
                { "kab", "1dca3e6674ba97de588fb52e1af4138be3726c835ae200923a581077461d73c73103b8f71fdba827c78e86f56dcffb8f01d0e610e0620becbd00e0617a90d971" },
                { "kk", "4b920a1b02e8b359ef821794eb2ec51c15ea361651a40d7d88437143e8794b14162ed6f82959ec99e24dbed74a9ed24000803e040ad35ea5d2eb753694c07f42" },
                { "km", "a7c69c7fadcc47cbfd934e3154e71d1b15fb0fe4ece423b838a5e19d1307013d66d78abf34c27fcdaf15dcb95dcee47b704882a73feada7e8fb14b267cbd31e7" },
                { "kn", "fc90839a84bf4f50928bc266cd1e57a1eb6fad5ab4013f40df0e82fd63c7fc7e853eba433088dbe04cf76c10488967d8e99aee54254005c6c610a313797080b8" },
                { "ko", "167ebfb4840f4883eaa64f70027d838087c02d4efa7ce46ba201fac4a9f6008b810c6724f718571a1aa68883e7268fa632fd04f5b1d7249ac51a9ced796006df" },
                { "lij", "71dae2ccb241986d355f1812b466f986f2f592f206fed2e583b625d183e9ac32dc14d68540d0cfd335b5aebc10bbbc586b379ec85cae923311546cb7e24183ad" },
                { "lt", "156877241aa42a6abb770fff9311cf688d725e768858feff08a234db3cf6dd35ee30b82e14f05b9fbdfc92a1aa741d634e7bdefaf13dd834470203b4467ea2e9" },
                { "lv", "2253f34f5c78899f2e8e02dcf8c15e7a7a291178cabb100c8e664dd117964535a4108cac6d811d70aa3a03806072eced9b7fc05cc6a346305b64c6fc7c7f1251" },
                { "mk", "adb7ba8908ae283ee7e8986eafa3bc84a6aa9905c3e52ac194335bb38c396f07ebf7faf2d469f1f43d74fc8b1e2a594ef03ce90501e239480f6c9f07d8a6db4a" },
                { "mr", "6a8c37afb0446acd1b6e28352655fc549aa4f6cf9d3be2521b79e0a08d662654dd0aa60845dcd0ba86aedcae279157cd042cfa9daf2e1fbea070eea7622afd5e" },
                { "ms", "2a45c00b55b357c8e57268b87360017fc6cf9f4d147e599032c40e0e9faf6a62b22a5635b38f03ec02a5b5eaf2930e57d4fe3a0b2fd4ef7e7cc67a3e141775b6" },
                { "my", "a48351acc306967f6dfde646cd2f169049b05df9c5fa68bafaa79cd62aafa052d6af2d15960290eb16ca5f441f7d64063f5512e7afaaa785d46b48b218ff9b22" },
                { "nb-NO", "6fcd3ac51c27f4c22ec8210e7e23b423531bc87b82ac7229ba00b228e826461a89c7e0997f2d2e4f8a36d0e707000edfd71ab407e989d55ace27ca940862e3b4" },
                { "ne-NP", "03bd88464caf2ed41e7b302808184e689248c5dbb6d217875384afb045ac267e3c0f7dd41bbab298dee4c8c54299157b34c41281e128cbeddc766da4aaa1f26e" },
                { "nl", "0813bfcdbf2718dcb898cd3a3f82eca14f22762dd56dc496f5ef8a8c9f62fb1714c74f1056b8fc6882c3953f402a4fe14046c774065569d19856112245a36f4c" },
                { "nn-NO", "405d8ec57cc52578bbfe9ec4d444d911c109ae31dd34dec9c733d5a224175cc578324219cfcd9de5e83793a614971dcefd06d2d7f9386681038ad7a20fdb5f94" },
                { "oc", "37cf34e98db1340ceba1f2af6bf47b10270afd8cd322ed9039cde4a6e3fa1076cf30490f6b99be76484410e51f50851fb5c9c26a3542c828d7c739457eeb2264" },
                { "pa-IN", "06373b6e45c626e532f741c2561da6c52eb08ed9fdd8c07cc866e935d30de8d3252a747ca3dc3a37169e3390389afd1d9445e1f48996f187f0e1e8f5975b9fe2" },
                { "pl", "84fd9fee8f46175fdc8755a717fb01edd299482cf5242edf93ad83db9978e1d52c5ea184cd52998f57f4268226362030411e28f36c8e2398276d579fd5ef6fd1" },
                { "pt-BR", "4ace22687bc03defcbdcae2f6dc02387a7e647e69c33b2d90975b6f845902737845809fe3cedd4cd399dd840fa1088763312be1e0f0cf87de865a33366e4a54e" },
                { "pt-PT", "2ea0243030a0996038bac190fb129b3394332aaf7c972009cbc5b4e2774900e0cab3c386c05ccd4c3d590a08aaba4dd0cbc3c78864ff740f2ab190a96ceab9c8" },
                { "rm", "cd1fe9449154e23e7d1c84dd78b709e1ddb52ac9793410013ccf456033e64b049ccf90eff48067f5ae4634083df4ce4007b0d7767692b990a49d9ca6a3aab2fb" },
                { "ro", "b7995dc041248dffb0b6400428d5738fae4935e20bd89eef967ce2afb56504fb52c7c84dda51de9e3a4c34be1ab061922420bce6f3dbe2ed1c4ed9b9d9ebbfda" },
                { "ru", "2842f1b160db49627314e9cf660577060b3c264e179da34b592d0e9a0d79233675f2265ae5b877e8766221ed07ad48bf1e5a9e1c6d44613f3acf6c7a348c63ad" },
                { "si", "c6c1334dbd5063c2460600236891ae0141af048809f2d9391b7523bc29489a6fc9bf8cf3a75723b5342773ef1cab41dce6d3be3fe0250e60e9e4ec531f87042d" },
                { "sk", "7f97ba13079ed2e6c5016108d0ada6a2ded72588d8987cd26bf3702aef366f012f0d067549bb2778bf63e33b0e663eed2484fd91060236c335cd035be75c6ff9" },
                { "sl", "41586ee611b3165c83a77dd1a510703de2d8ae9f73768279599c02a4224afbfe6d65b29f53f68a5d92909d49742c6789f0ee21ca40b506cf5ff1e476e853f5e1" },
                { "son", "5d7b9a1dd1cfc9a68ef0dd93a6cb95f1fffaa2a1dd3d4ad4fe01b42d8d8b49941fea8201b94e50d52b7193f3292ebdba1b1e22e6ca93c9fcabf85f51ef038723" },
                { "sq", "29a4143a331512f08f930cba8a4525fec6d1294c9a923539d8a369eca89a58bb461d4da2293797d02b05cadb2d7b8514668c28b9b72c5e9fb7659729a6336b11" },
                { "sr", "5a0069a3c1b75f63e072dc1c8afef69ba84410c04c1d3bca4aa5e12c15d66307cc9613811060fd44a85a0477baa94ffbf44089dc30d92263c1afdcdc07a1ed6d" },
                { "sv-SE", "18b019181a33fb2288a03a6bb8f5060a98d81c1e4e269e232f1c0c4107a623fca4bcb6703bdfe1ca285aced7d77e9ff3066b05446a056f1dc054626f0883a999" },
                { "szl", "01e22c34c01d322d54f8aa669ca963d1faa498cad9b64c72028ce6328e5afae03cc5d4e6641781905cc541600e9e23a6853118e6d8e37749f2eea36798fa6866" },
                { "ta", "936a9ad6fcd13dcba50b845dca077a682d3236326da6dff33455cf83046b0cc43ee3d04e75406f0a5d8a87d041354906ebb1bc20fbcd2e9e1f12af0eee1e5e62" },
                { "te", "339779dcf88befd12761ea8bc732b596528a017539a689bfffe6f53ea7a3062036d5c90b238fd8d9a4f04740cf4b1857bbf0f0162501b8c94e13b34f007d4790" },
                { "th", "dc4baeb10e68efb4facb0fcbf12806e6c5ab3c3a19566b3bbb3867789fce52e71f1dfe65af7567ad8f67413334e549816b1784c4fffea115efa3b2c54eb79258" },
                { "tl", "950468648410ab9e8af28a134b1fd74f34a1f46f19ed9d627673f7468755cdb5d621f31f8ac6fbd83cdaeccaa85b271a21b2aa666a5a8f7003032a3e6c4c54fc" },
                { "tr", "cd4dc625342670207591a4792b512d93ef1ae1b03cae69ec9e34fd39557d7fc4eeacb2cc147007e3389dad6d0ced847777bea7af606738d11e7f6cca7306781f" },
                { "trs", "9ea5eeee249d894318e25a388dd87c5a4931c3ea989ec6c83dfe5582959526b9704b6c0666117f34180666055a8e42c40fad662416c28d0277d91a837f81a06a" },
                { "uk", "fe3d2cf7b1664962020c7e2470563d485f4ebd142e1e37433f2735ab6ed08c9b44f22c41b9def16c6088d47218d7148fc73d73d71d59ca1ff2b5e60c0fd4ce80" },
                { "ur", "0081ef5d90308ad43e5c07db91566a2a9dd697f375c8bf97313f909ae97a6cbd0a0b28e7d3458f9eedfa889a1f8df6c0e2d467779f438d646ce86c2b282ea0e4" },
                { "uz", "ee1c9eb2457383c9314c41007a7cee5a83676f50704754c48bcffa6bcb97b001b70993693c6cddda3eee3f23d6bb49ad2a90cbf9d946bfcb028cbd446bcf8dc9" },
                { "vi", "5b5220a0b635ea6708b5bf81fd568772843fc0423d8d766fb8a376ddab5dc64df4d64bc745bb0a4dc418a864c5ed3328e84992ecf11f3f035695cf151468c98d" },
                { "xh", "280872639f3edced80ad85df1d36e018952f2cf50a3523594e5fb3dd297be71b5a2fae1a529790e962019a7a8989fb39929d2ba773472abd2f1b2d780b8ba9f8" },
                { "zh-CN", "0a6be496c19f3718a30663c98172970b3739b49cb8c4055c60bb4635631cc10d8b640432473343718568832f22f871339132adfe1d3b9f57324514613a702c76" },
                { "zh-TW", "9693b2c893932aa93b3450a467a99e2e65f28bcc90132e6cf6d182c58bf0a647ef4907a258efbe468f0507a66306626eaf3a654ff8190eeb5e80f8844763ad00" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/89.0.2/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "75f08ebf36be415c03ef8c8443512ee4e0c9b360a65bf5e88b873ebcdad110db5d1f686ebd250cd8e048b3089aec9becc14b45145831ef7d01a0bc7109c79c65" },
                { "af", "3abf4fd4ab58edb03bfbbf2a2fdd5d971c1dae300168cc18702fd75dc1e09696a75145cbe66744f1d858b3f463699a32b4b231a6987d41a8852d66ef9fe7285c" },
                { "an", "63b76fb6395dadfab5ac34a23a631d9600216dc339f6cb0538319e23cbf9701b904bdd386024d7cf2bc9edcedd647ed50073dea17db3741aa98fd179bdcc1b7c" },
                { "ar", "1d2a36033b0772ff8c03f3a61d05e767a63e6996a143bbfaeebabc2014cfc791abe9b3382e93c5ec9b661ab1f6a7d93df3a3819c4aa2c82c98c95c53bc3074fd" },
                { "ast", "35d0968bcaacb5c98d1eaffd331e492a61c4d955f98618f9b77457e095fab3c3251ec2b2d9a543a42fbaf515c804cb7e850f524ea855e2e79607f86356005edf" },
                { "az", "8e32899eed020ba0087db07014dd43d8cad73d7c78d40656a0d1231de2a07f989f8c8d6ebcbc41113891f846f6c9d58b89304c61f56165096a4bd59a2c9573a8" },
                { "be", "218754f9993f79aee28f9c6d582ebdb186101eb7e42bbadffaa8548047f498aa45bc8377ecb8f4a0f67a9dab01c963fc9514bbabd4608751beb5e3a5fee46404" },
                { "bg", "be3a956ba1edbdc6cb8af4f857fd1193a031f7389869af046db12a382efe4a6595b3527ef96bbac40bc8348e5a0ce12fc7d374c49e4b55c0b47912f6c5caeedb" },
                { "bn", "79b427499ded788ef734db0748bdef0888ed8961bc5c693df07a3d392124c1322a1eb23412bb70820ae9714a090610818ff148cc9d04bc9f26ae365dd462bdc8" },
                { "br", "74f95020909a002e76d5a81571e67861614fe92aa4081fd0faae7bfa7dd7f343fface0cd00f507e7059b5be867d043aafab44a391596d9f214656599e0a5dbe4" },
                { "bs", "455c1a8d201ab21beec5c18528ccf13a5100205cb2a111e850ce0a1176822ed64468d129bc36a5ffd4d4323a12772c37e3faff06fbd1ac330010250d4b3fbcad" },
                { "ca", "4af5ca45111f6ad95fe88b12d783a454a38d34fcf7762da4ae1a33e4dfd5fa9dfd7ee460bd8af4bafdcd0c3c8dea370bbcdb81050ddb300cc35c973d7befefa9" },
                { "cak", "3fd75dd6231376feba26d782526f79f18987056ecb61677b4e140c0c7582150591490b52dc18ad497e87904145607ef2ab7def727aa3cee707bfa542df25b7e6" },
                { "cs", "aa89cfec2465047ddc4cbeb9803c403a0666f6abf169bb407b98545b8996466b433c426c7e0a61b630a9bcd70e5919ed66d265c6b59b8424d7bb8e4ed7b7c915" },
                { "cy", "a9ddc4470d419849da02464b60d9bf4c63aea0ff2ec4590673cc90c06ac38855a7f43b54fa7d9476fb099d60de3bbb6a8b687ecda93430eac0942607cc9b3b1b" },
                { "da", "3779684add78e46ead72d2d7e2e398a7629468d4a9e663fca0376d9b6cc6bf0071ad2e6a7c825c8e78ca3faab50a708e2a426152e1b193fefa440d88dfd5cfce" },
                { "de", "5f057141d0f1afdbd95212d6716b639cf89fe96cbe11fdfeb0c22a7a4a436de8a16a88f1cb30483a3015e5d6edb6e5cb6b9ac4964a038b6731b29801cabd2393" },
                { "dsb", "8d6530262cb9f11e52e5d599ebb7b7c641bcc1ee2c255646942be901daf2b057b3c0d7dc894d14c7b84d793b785e5634ef24745575ca8a658eb85d13ea79e18e" },
                { "el", "de3dbe5e2d193af0d9249ef84f7dd31d25271a809ef56ff4fdde65bdc65f84f35ea99f2c5e26595a9cec064a0d8df8055684a97f657658348305eeed9cd93f90" },
                { "en-CA", "6c15ff43b7c25dc63c15b0bbcbb4370ef4c5457cf4a33aab19e98517598956eb404902a138b4de655fa07df7884d93558caa6ba6404a90dce9e00831f7ca3611" },
                { "en-GB", "d68c08cacbd0ea0c73911a374d7f0df18a437515a6a2a5e71fcdd6dd14bc07883d28472efc502e648b681e2ab4d7544f9d20d5f0d2a27dd2cfcda41d41805593" },
                { "en-US", "ea47a285025f2d2d55a57cb97276acbd7ef3750607488878bffa34bf47211ddcb939fa227a2af6ef33ba3414a6103c28f33dab9536e33b5104a953083af665ff" },
                { "eo", "8cf70ebe7236e96cadda6c3355359781f2cfa1b9575cd266577170f171d69b3e6c4f0e70e90bb471b2a341513c67f3c972f3929cd3c5b775b861b0ad4e7853e1" },
                { "es-AR", "92331d76a49773fbf2fac738c733c1ef54082c7717f05ffc847da1a88a26f36d43872ca4ad9bde883a52338a302da31236b1605eddbbed105f07f277de4ef30c" },
                { "es-CL", "a4791661e56967acdb2b8cfd936f11defc264d805572d259e20b5150c7fd7b4d4df57bd85f538a82306f89a76bb267d8b3bdc3c3ab1597216b019dc0a304c7f5" },
                { "es-ES", "881d8291f73f009a71efaf166a5fdd4e5b8e76833f59c82e4e3ae293c5a9f068a47e66e04f4cc7884a8acd64db5f1583e83a4350746afec1dc05a105043af024" },
                { "es-MX", "3b488eb3179fca331a10090df9c1a0c4fb496a8e735774fdc9d3aa5d93af1456d585aa81b44aa4d8f9e20d5bf48c742bea5f8daa58764d0dfc6a4631d6965137" },
                { "et", "8ff4626bff5a1f8b4e10e265e6d04e33eff63398d7a0487809a198fe64d4b802b9fd5b95e027f4ebed68f400e75ee4a3720712a998424de91feeabb0b6e4c9d1" },
                { "eu", "11ef8163575e191ec79ab43fd9477467315dd0471964aa8b349807e18aa7ab562d25991860f92266dbf08c472f9963d3d306e39764faa4f9bd3353bf61aeef84" },
                { "fa", "d04d141de6e967a4fb25897a230717be582c29669447d3c262bec087a5e13e767dd6a040475650bd861f7f8727c9d2033fdbf8635ad31269c6553bc42853801f" },
                { "ff", "fd9fe71ef88f43c3ca795084cddc2fca448f46dcba038de1dde2cb148b90092f22a34634acf1a37ce37a45ae6783c774d23be8be299880e82e4f3865336d571c" },
                { "fi", "b4a1351e179691b97ccaa5051119d4537c4220946a5da3f82fff6d8e8e04b13b31bd818bb0cb53d53978027bc53ff408a983d60b660a258b6e48349b2cda6bd9" },
                { "fr", "79f18b2fa196f66a2558de52377f694f265e9e48d67bbab235d5bd03faaef9f537162707169b19b9130800ec0f5aaa5f300ec69f088bd2c2282de23f5ba5f3b3" },
                { "fy-NL", "c75990426be99bf36dd7c74edb8c4a6f92e1fd3c30d1acb300e75eeb564510026641d5089f05e6d9591ed77ee4debc3b67634557ed019bfc678e29c40649e06c" },
                { "ga-IE", "581b4f4a6905b50a44d694a7266dc3f9dd8adcb006770af9bd318dcfd48f31f08ea216e2323e2a7bc1cac3a07ef5d5a25b0ff095fbe5f1ace0413be09750d656" },
                { "gd", "91817f592b068da7ac5870af37528cde11775fae93ca26593fa177bc575b2cdca936f9a1e06ecd6939292966e6b1c80f2c1dd87292b4c6b644fdb4559e264b09" },
                { "gl", "9d345308f2b0ce61f3cce4a79ff660734162417cca1329ae5b94cb8fba774a9b082694f0abc446458597a623162efec5e81649ad7268780f058c7755540441e9" },
                { "gn", "1134542baddf3ce1e46ad69c5da1833dc5bad9c114dbda3f736382824763ac98c55ced084d94a865f963d9ed0b3d283c24eedb9770e6a3a2c55a9d7fa343d352" },
                { "gu-IN", "5477a4aa8d6f5238a5f1d2478df55498b9092df1eb0206c866bc8386254bf39a3b80a543542ad66512b42167918cb40852b324ddd8870f7cebbccfa718c80877" },
                { "he", "406cccfc6d3080dd19c69dd24308ca82e5045f106e5f7b006c0da265d7704d1eb983213f6f799e3c779ab6f63b31325ce37ca716a139005835b710fd48a5cd58" },
                { "hi-IN", "15ec650634494abfa6c853033d6f55504f2686a00a681630e495cba10bb718697b5034f7b9d7bcabdb8e72e487b02b8a9200698644303109ffd34af2f1942fca" },
                { "hr", "943d6f465ae69e2574bddad1ed924c149270ce01ebd479ab6a3aef0b787bf6d9a952b47053e0ce500b91733561d48d3d6891a1313b4d4647fbb2d272c49887ad" },
                { "hsb", "0de182f075d2eb2e4127fa75f26c9ea2379d6c97e3b9b777c1e1376d6f9a0789a25f4341f58efe23e8cc62274df0d40b03dfa764858284105b3d78b1c15cddae" },
                { "hu", "b8e94180daf31db898a06b3890c194f186a67ab3e84b597bbb7fb50f24f7ba23ec543529e6e6b9b16283d92374e385681cd14e20e0152f85889cda4fffccb11f" },
                { "hy-AM", "fb940e6a6d2ac84e458a6b077b8e5463112ed71f680e005327696c63e6e523d81a6cd06c5f3429167af6390dbd93a094effc0a75c6a739080f44f1cb9672e322" },
                { "ia", "9075238b8335e9557e415847bd84c0c9268055cf62e15bcb9654b872c97633dd69c41c191c5c39ee0299b964dca637a885e62886ca9dc4084e4c010f694bbfd1" },
                { "id", "b0bd8d268c53904265ad015f0c035383a7fd52bcad338bb30415e9a784d367cd99dd5bd155d1b86ac20aa36a867a613051f533de2b5b396e65c1b470a46ccdb5" },
                { "is", "6483894bd013df2c8203b9460220c7dbec33c1ed7cbf5cfac1f03d3dcb16da6fc18639f70459ed07174fc4bf5cd59a99cba7723e9c009763aebaab5c011a3f8e" },
                { "it", "895f8ca02484beabff29fc345f40ced4215741d5d9ede6ae74fc4e5635c4a940edac947757a5c1d3eab88c98268c2a12b143c5214f5e335ea19406d72c42d554" },
                { "ja", "4e756971dbc5b390c06ba466faa10f8eb9902909f646ceec98c92a669d039b4e00d26ad4992396224e420ea679f9ff797651678285cde2d7aaff98c05b68465c" },
                { "ka", "445fa0ddebac5df78768d1f1393a7c8ef528be326c1611ac0febcd02a6f3c63f3c931a8eced43d0fc0ca5461d1ce2efbcab949f9af933778bee8ea8ec65877a2" },
                { "kab", "09e5dd5b9ce25d209e7981364ef83bcf7172defbdbb5b3f0572e8db696742b6c0a001b1c4f0ad4636a9ea3412e22500f68e636773417332270af1fb7824465ce" },
                { "kk", "b2fdb54f0fd6e6394003ad2e1e9f9c7d1a77aac4c26490a996385e13364c14b1d4d3f95d5b06e3f664c26d2d9f9f11c0afd48e4f3b5f671679d2ff8b514c0f41" },
                { "km", "4c8158eadb92cf39f1285a5b1a6649e16049c6880f1ca0d418c36927ab9589619056c8e52f66d9a7f99d00943e8407800d9a9fe037077234bd96959e3c33fe52" },
                { "kn", "ee1a5a47ee91d14a9ee6619137a5a55baa946471c07083c65264f6b1daca2bfaf38449020f81e185f25472c9b78805c4ffa71b50cd182770052c0716982c62bf" },
                { "ko", "5e0bb3f8385662c15989e49fdd69e2e51b440149d407015a065abf040e9eb3c5f34c96f606896e63de3c5aec6eea73667e46b29afb4304c056834ab7c91eb4c8" },
                { "lij", "5be768d24c6c884087dd4be15b4eeaa3bcc9fccd6270d733b497228f9e3da9ce32eec82aba49fab762ccf8b5349ba05ea1a95de831041c407eed5b3577e5f3c3" },
                { "lt", "c2d088d2b64d571221d17cce2a350491368f884896f111ea8afca55ba40ee119a7a6146747bb768a2c2fee80f9f50091b62675a61ccd11fc75ac37be69e61f35" },
                { "lv", "cf2077694fa34989df13c79577f1f424d1fd9a7f8060c7c7e432de83993bb9a6bf603dcc0fde68d3715c39e1e256cb49f68442144ebd3d7ed0c082b107103722" },
                { "mk", "1d49bf409c9706b88d6213e89aad583abb51485eff438e7bfafcd59e98261d2c607e58c5581ec8c26909ca0550db70f71fccc3863d01ea1debd11dfb8d53e7be" },
                { "mr", "e68b338f616e868816508bb1755b87718ff684ec12f2c80e858e7fdb56e214e4026288875b88e9a6607a7ba489fa24124362de0e257761a7a398bf9f90e26779" },
                { "ms", "b8b73e01e990f06c3d47f4416fbfcf480f86fbeb0949750407c5ab2828af7d7bfd453d3f17ef5bbaa9b1f37a77498fb1df674deb01c5c26bb91e3e6e0fc6dbde" },
                { "my", "cc28c2d510016f02a94902b3a5610ac7e6aeb3b2ced5cda543ee242a0e364ca50296dddd70203bb99cf3b8e99d733def40a9c8b68921cc1b78ac37bdda5576d2" },
                { "nb-NO", "e0b7d272dea485f7a088460d67915a97aabaddbe0eb6b1ec20f390aa58e84e13924b1efef23152092bf1492eb1e5d2d470a72bd9e35d06ae40b8cb2e8a75ce08" },
                { "ne-NP", "62027ce9ff070c23658b651c7cf2dd0fe715f1f43e20643aa84e8c6aaeae27976e94bbf845cc5a250ffe24d9ec8c71110a680b3ea76adac41de4eb990ece20fd" },
                { "nl", "a4ee47fad42cdd9ed9a6a81583c5b8d7cad850ea9ae6dffac942904a27e6f221de142e9c6d16e297926c64e14d6dcef7e3590a1709495a89f027e7f7761b7399" },
                { "nn-NO", "33d69f6bd3a12e96b3be742bfd581ce5d0b6f2315b3c4b395b6cf8a9c573288af555f76b3e4a428e1a1c2b5f878c3bc3b9e6c7e403293028a9f85bb0db16bc2c" },
                { "oc", "645763920238e690b3a45fc3f3dbfbb2b672abb12132738108cf539d92c39a2c645f82fbb5dc18d6f35956adf02702d0a4dc01c3e6bf61dc52e2f3d8158115e6" },
                { "pa-IN", "6d03e40cc70e243b82d78405feb0584334be3a60d7cf839ebc8417dfea4a5ce94e8d350bc32e633b684fdc2293607dc9bc400c0ed7f9cf2203b9eb1d25ba6707" },
                { "pl", "685892cd76026463ec82018f9b8e8c22633c96d8adb68aac5729eca82368ab5046b7bfdf3fca5105bda338f2eb003636bd120eaadfd5c6978a9a6621edf3b0ae" },
                { "pt-BR", "ffb5ebe8c7f3e05feef90e36879ba1213ce9de9644b5ac58a0020f9dd734ee4aba4417366ebb3fec535cfcb453bce152b6de0495bc7370909d307dff8366d0a0" },
                { "pt-PT", "8efb4d4235a12366c8d5ae8803aa05b8ff70669daa8f086576adb4871b131bdd163fb1bef89157847e761101edd7e0c2a82866a9cab8f434b7c883334ad4fdd0" },
                { "rm", "fdf5ddc14c739b8a89ee8c9f7bb123327a5f159d02b306a691ed030bfb05b79ab8f9cb585f6906b66bf949cc232675356074738c2ea9d508dd9a5577a53fd28d" },
                { "ro", "43249b24022d670a0a391ea3f0ae794b1da217345c6ab3a845f56c2acbdb219435afef90fb74da870bee18364495bb01fea4f1641aa9aef37777a55ea3446b25" },
                { "ru", "ca29868e5ce135f62d2edd92664aa20b8e0e9143aec6931f49a61da7716bebc43ea48970b5c8a94f575e9f1bad5ebc8ef4e6c25786f0cf887ddf7ea520d790ae" },
                { "si", "2b13d69a3cc35d79977512f7037e80fbb186dca4d01cfdbdda89cdecdab9726a7ad670086d099bc2d7c2e911a425660e89ae7b5c36d3e6268342019ab645a5f9" },
                { "sk", "9bc704796979ebe66ee59ba76f13e8a1cc71d6bad572d9760974848e64f90e6e69deb48e589553c52139ca86341262e7e15a8c146aaef9c4a1887597aa50d92e" },
                { "sl", "e2a10b4969f5f383ac3999987e66f95e1ebf8a9ba8f53f3c6265f5ed3569884ad32e993e9878eb0b0f6cd2c3136cbb8aeeb1632674a97d0e9a4b980ccb4ead96" },
                { "son", "f1a6338e66055e5ba7168794b68a7b51c6c618699c8a9e2e085d5e01758531934bc7df0976d95431f8b6c65127d50eee685196103c754a2734c14bb4e91be6a2" },
                { "sq", "39f43e971355b20961558863f64723398348bad714cdfe09382325dcda4b9c4486f923f648e61c4843696cbe6ba6b1b08e65f0dbeaabcf4ce67b16f80826cda1" },
                { "sr", "8eadb3801d0105e31a3c090b89637550b51d959ad8ff399fd9132e9abe40de111e9e386c98781edd60cd844b247b66fd82219a2778ca2fe6855779fc5adb9b2c" },
                { "sv-SE", "294647d1849d29d1f22d9be2cfc571a32402b9c9a0704dc92fda78b2a0fbef00824db3ea6bef5713983bb4677ce7922ad657890f9be583218e1e1ddef9b9f458" },
                { "szl", "c579891b16ddfe4ebd465b2f33753092c1ba93e5d302401186b2fdec25c4dd4ac53556e72d55e930a525d59226f94b848fadb3a0660460758a3b92842a48a190" },
                { "ta", "7683f7c83caaca9e3f94475f78c97fed15435dc7afdc38fd78fffddc37c804c23eb0510561b48027a94f8b366927f10bf5b6b5ec389627af297e32b5d2ddb603" },
                { "te", "2188401992d7f60db65e8f87069b666363e94a2459905de68b3cbae4cfaa4222975c756cc8ca077ebd115cdeb019e6923cea71be39a137a0e26c43c359a8a1a3" },
                { "th", "34a0158915115f51e9ebf4c74d56a7c582ae0acde8bffc0d16e2bae5a8421ffdc4978427fe3e9f2ad750a951a90d22236e31285ca185decca86acb8a641f711d" },
                { "tl", "fdb04973e3e9e202bfb77502e847fc4fbd4af4b6dd644afc0e40766664699d9bea13b98f4868c51e528e903f0c0f5509596215c54f5e47bc1147c115491345f5" },
                { "tr", "7ac246635344a6a1ce2a755eb8b74803eec8bcfde507dbdec8ebb18898e119476f604878f383952090d7aae56915d932a5fc69fb316d53b4bde4cefe30a926f8" },
                { "trs", "74ca88cbfb403d6012bedd461dc86ec18551bcdddfe1e95a62a4261ff27e9abc144edc66af5639fdc4af1551c29c0a5e08981b7c7e6f8eef1cfb0d3cc031b0ec" },
                { "uk", "3412064b0687dd0bc809702563598ef312876561ef0c44571f7e64eb8c5c71fe22746323922ceae59e8b6d64ed476851c6f93fb65a20b381747bea5338c08c91" },
                { "ur", "d6de84ea062159c42fdb03971a415a1f695fa5ec98e80799b140769dd4cc07a1f3d369d67a89dc66bc5ab05d5c158f4f952e19b6465fb57d951256fe99e92186" },
                { "uz", "e10a3364c581ac4a46ac3151a1c1ac95cc2322fb726b6379aa3cea4f453022d1e203030275dff1f924e8f2f1affb499dcba86b0b6ee4ebd3ee9e29f1b6a68c5c" },
                { "vi", "ce1879fa0f50f2c316c8495da9a313b66489ecce7425bae4b6fb855cc0cf8d436f0a6eb983cc821b2d276cb40a09b853944e46130cf13f4671eb935a60c0a634" },
                { "xh", "18510014aee32c6aff01ae0f2813c041c8d4c792537506578204e2e0d703f2be517ed2e51052731625d345451ac3428980386960b51af3520492ba7ca26fba9a" },
                { "zh-CN", "4c4f15c49590c364508845bd985ed23489a1cc0702e2f2e0d61941038705585f3dca2f27ffd38b8f7005f32e790992e8be973ca71b2d6270a8335826835ff049" },
                { "zh-TW", "3de902c31ab083fc69c99dde4bba0460ff007f1399c9dae2cd79fd74920d91925b102a16e00a1240db93d50ebc374598d7fe0fcd31d0f047202c4ef84b1125de" }
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
            const string knownVersion = "89.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]+\\.[0-9](\\.[0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]+\\.[0-9](\\.[0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]{2}\\.[0-9](\\.[0-9])?");
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
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
