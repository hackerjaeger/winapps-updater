﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "90.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/90.0b2/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "7ae5acaeed40bc564bb40cc8dd2de60284fe886ec3417209796abbe3c29d3f39f7ea10688e562d2f6e5802e02dc2302be9f33053ac616d68f1336b5758836238" },
                { "af", "c1a2d34e85266d6f990554a7ef8c16f632098c2c3b46bc6e9526fbe6ba23dbe077eb9da64105f73f31bda127ed0b13a649967e3bc52706185830a316e453af48" },
                { "an", "1b41b9681f8bdbfabc8c5a14e3142ee215279b89e7c0f9908309a38dbf4ed932a9feceda9d05137fa14dbc89e06d48a444e8b5ea83c49f2028e67d9724ddfc15" },
                { "ar", "490c804404e458a0def694b36596d400beb853397d226c8ff13c6006798cf50858ce8f0dd01c650e75e7980a8de62619d7d10551ed1849127cbebad90aa88b4f" },
                { "ast", "9ec6738fa3284233aca319c7cf234acf44609d69346ccc599690a1eb5317fc1b25e2618ecdb97e123277c39193f729fab5ed3dd805e31c1c4467f47e365767e6" },
                { "az", "6de9be4e3155319539f3a26db91aeb7649309b64f432eed20d52dbb4b5c51e7f98628c6bf963fc3adfd7e12282e2b4139948d3dd756dd5846cb8b48dd809b661" },
                { "be", "079b3fd55c6270759dc5567b1b82c5bd038c70c1c392a52d02fd5aeb41ece46466465e5830bc450c0763ce2dc309df92bd91b0f16febb68e38bcdbd12b9f70a1" },
                { "bg", "c2f568916a62705d09843b19014c3ae8ea63ea2f7311ad010f43749251f9ebca39ff60233ac5de2e4ec46c8141417f4f9db98f778e547f0fa4b2625ff23544eb" },
                { "bn", "9a6fee56ca2f4d813d29488a9df36930d6036359129075bb7039142f298784a7e4304f4f57b99c3f62b08fb86a8432c04021f6a1347d7e2005281a88aa4a9cb1" },
                { "br", "76599af8bf663f40e4f190321e0e4f91f6ebf308e0efa3187d30ee1d0ad82fdeb3b6f91e08ea542caf9c772cb198cf4929dfc4961a2bba3c8afb0b3705851f78" },
                { "bs", "25457539dc028530e209d2a9a7be1ce5df748e7e4eacbcaeefe357dd748f465cbdb93f64b6640e69d220afc974f39c04c8bac64c6ddd2a9c05035642937e24f6" },
                { "ca", "26650a7ddf80ce96b09a0325dc4472a1cde7f605b2388f200bebb094f4f4f2ace3fdf4f827ec474c5fc405ee42f31ca0b228312fd1e5228ce3bad6540aa74246" },
                { "cak", "fa285d533e92ac40c41eab8e400f44c5b0a16d53a417c9d27eab99eb282064fc07064e1b49629cc8a2dca18e7de00eed1c94ade7d51f33be9bde6aa54b5d762f" },
                { "cs", "6c0f187660da433ca1c842795a25b45afc75ae07234a1e095aebf0601c1165ea5fedab3cec61c474b6066d7efefbd935bc1d8332cffe828052da5687510cad71" },
                { "cy", "4995282e79158ac17d1d4a4169eaf90dd2dc90b878ac8abc8fdbfe577b18762564de7eb2e8eafd43d7a70131a741d44bec9148efc655f1c52ae5b08230bfe4b7" },
                { "da", "aeabef6dd5e5158b43fb593b5c3f5ae863faf39bd8a7e5c34a85c59b50ff884ee60ca41d712aae063eba3307d72e24ca70524ecefc0692407f694f7841d0c2bf" },
                { "de", "efff382fcb77e3ab114aa80d6f7f4b2db857a606033e8dc744c270ab06c50455cb4651b6384c6bafdaf7203722d25ba5ac355da3386e2c4758f35de8379a4090" },
                { "dsb", "036e7ca0f1bbc52bf7e2dc02aaa82c5b0bc0ea687e3f235659df957a2e6837666979b19ad8521ead8f2743ebc2efe4ab33f11d663fd6129b2f53b4c07200ae55" },
                { "el", "b96511586f74479e323750a5f5191755d75efc1208e8490d1f0154abc76965e08dde386b61f059bd6f9ca4f602344ef5125780b8d3f624f6cf5bb5d97ad13faa" },
                { "en-CA", "0f378fd754ca51c604020c6a00358a8c840286f8c952bd0ac69327fa48da2415dbdf72d4e4fbf3ea7331f51146af2334d34037abf982cfa991341b9539d3c903" },
                { "en-GB", "2dea2b1d04e9338edfab39b58f82c90d1fe8e4ae2256d4ff75eac69f5b8064a3f31e305869c8f80e1829bcd6c597a784391b7fe3b8f56fcd31035b17a21321a9" },
                { "en-US", "cb2aca0d8f37e4f27336abe509c4c5e889ec44827b846b606dd05aeb5012ed14700f14d519755198b0474cfd8f64305abf2973e3afd051a2b054b512caa4d2a9" },
                { "eo", "870b0d7c4731c3a5dfbf569a355a5938fbd91229422763895cf3833853eafd7f38f0efbf9648e0e0f862816a0c2f3db9b15d2a679faac3c807368a5d289508bb" },
                { "es-AR", "f1833df16c9d0942351c1b0250c8ad3d8421dc7391c58e1b0e95f327f6567a0051d2773b96098011a9e2f1c5296da611b1f6f413399d25c59a0831b1bcc73812" },
                { "es-CL", "b2ff111aadb81b00afd8c844907d5af0ea25756a38ac7f903ad03ffeb704244f58bb2e1eef6c346780cd2da7596e519a8a7d0ac9af0e026c8d7c795c0fbe0623" },
                { "es-ES", "ddde1b2f7e94a8155edd3f31fffe56b99762abf7229cc7e952553fb472db9d4fa4ff677654c2e8188e7545b11503754a9bc2c5bd2e902ee2a85037248d39f57a" },
                { "es-MX", "f60fc003c93eceb42b13486e84c9f7bd378cd02f5015d8c61c126a909d2bbc39237f6c178bb8c30094045444acee095f3814b5bbfb2eee74cd2382588029857a" },
                { "et", "f56de5d7eee89653981a954d33dbb03ae2a34f302a303820974fb2f4bf12e145c40f5b6f5654ceda0f11d0c4412136bc697428eede59af93bfbf93b34e1e5069" },
                { "eu", "23101214618b50dc3655cfe0f76fce4a40ec0715d4d2aad1dea3fd44175d1a7b7d0a6f4e727c31e4deafcd86be783da88af3b937ba8af8b888afb188349e5db1" },
                { "fa", "62860376a6c15b1f407966ed2d3ff25590d6276d23c830ba28c5ac5da2afdecea243974814aaaa65a512bb78bdc34efeac38ea15bf992e7e80362ed010ed1994" },
                { "ff", "efb05c7a7901a45957292937b2118823b08f7ff6071a28d111c74058d8804a9a8c37cdae3651095458de9731aa2d45352bcb5ae485a6a3bdaf673292c6730082" },
                { "fi", "e8661d4a5c90ea5c02d0a4838132bd9425a74b9869d6716537a0825d9d0a7c3045ef7f483fa9eebdead2c0af554861d3ab65f344501a6732a6850a0c770a301b" },
                { "fr", "d27191fa3cd5635d278df2dd8495ecd741524d5000c702a87ef06f3e0426dd6642445b609d0f927874c7383dbedafc1ba93b41f6eae219d5af8ecac631a5ba56" },
                { "fy-NL", "970e04f46ee97ca45c5016e60d976bd0d5a8067a99d9da5d7ac1efd6beae0e652ce791ba38b17b21bf8c7d2bb18ebe7bcfc36d49d09b7c73ece8c0e59e6d15c7" },
                { "ga-IE", "d3d19445766cf6e52126e217546d38197951753db559fba902807b0b527302882b01392d6ace9844888b4ddc99b11558f214446bba1e87866867a2292a01ae6c" },
                { "gd", "589edd58de866aa87003f932aedce41d2eba1a9bbd3ffc90213ed7da07297ebd5fc46ef09e416c3280caf9249844d851d64f0ab8b93a0a2119c29bc975757f9b" },
                { "gl", "678d7122d47b34decaf7a9bba58fd26528e075e65b6291ef5c7d61855c4e2136a6d1e9b6d58d4d3b911c4bc3424d1311cdc59920b4dcbbef581f8cd70895b589" },
                { "gn", "1c663749fa99dd2756ea0e69e34e8953c1564f22dbaee9c5c3001714e53ac545a73196371144d4a6566c02692f94823d7a3a6f12d04456ed027b6c1f8a01ceed" },
                { "gu-IN", "f6055b450600255bef6faff00e7a74a6f39f732bb758aa3a882be813dd25329df372e4016af063a6fb8d5c7def263ac2140cdc5dd38d34d282ab94e2c169a533" },
                { "he", "01acefeb86c3917a6abfbd0633809346d0639c6d1581ed2f67f77b100b7ae25296ad6ef8b356b9b123fba97d41ba0dabe45d614d59bb938424998884fbd90c02" },
                { "hi-IN", "df6b3df740298c04a66306b0df35c319508ba04068638a55b228e469eb79ea0bf935c8b3c2ab844b3eebd21cbb22e26d9860719097e60dbcc52101b872626547" },
                { "hr", "4545ab02c32e34c8443a37d304fe870af3cec0f57ba27f80ce2f63aad5dfe6a71a76df52aa1f5be96d59e347e10eee45cafc1983d0fe7388bdcb9b91583f1be2" },
                { "hsb", "4243f9e721e504cb1d681ecf3c40ec352fd15e88a59cc77c015374d52d1debb63c526feec063dd90da0468197880bd02e3ae17142e031fc796ff8a789f356cf4" },
                { "hu", "1799bc2d3c0ac994343a827f30240b43dd4381e729ad78225185073dd774efb63fa8514c16ec51a6e6cf714bd767ec51fd4c1a872f4ecf4787b1c39d3b2ac7a1" },
                { "hy-AM", "57d169e251a45a5e4bdc24cdf23d2e4bdd6d9035a04c6e99a15895f283e15d7ad955a9f9b3ea95801b451ea53410504e301c51377b083f996bd600b4ae99b225" },
                { "ia", "c651a3670da13d1d331e3ba6c576d1ed4d8ce636aabc62b3168aee08f979922b1d2eca4a3e543d2030f72040942bb18045bba7fcd0f5fae6f49468518d25e7f8" },
                { "id", "7e2deece36fb8ab0f1807f19d8f73db75de449a25e6324a4312e4cce49631fdc276164bf452a1bcebdf308af5abeb89d96c1dbf632755f01e78e5a1b1d48e094" },
                { "is", "d93fc63a1a8ebe3f211f3d42419993e173fc334cf68963e0b6b8b8494eef240f17c0b169ea6b13d3c994c5fcf46b00e8ecbde217bbac7641f57c7de4b32fefdd" },
                { "it", "9737e72502bc2eee3f192eddc8908865c70bfe52d48b09829e61b2b8d8ee483fce236c5c833b3493ce7ac2df1fac46eea592dc9ab8b66a26b09e8014d11c792e" },
                { "ja", "815acff704efabd1b9d10dd07bc64c4dee2a8a0c151bc8c0fd0298513cb4cf08e9ebc8431b91ee6db998a0f72f287a16b9e193c97ea14186f4b1dc28703a5123" },
                { "ka", "6e3d0e9048960aae6a714cb8bcd05f88ebde7f6917be299c651206c91bf6ba88f1139736a965378fb982d48cca7a50cd0884c7cef4f71cad1278005e171bd4de" },
                { "kab", "0995a31aed0db9c92c5c0ac18573544926b2a734b9b158ede2d1096809e31e889d603a47f49c3c4d2e2577c68b3f6c171aa2e0471b11ab280fecc4997f062903" },
                { "kk", "a4c45d74d91dcacc984efd8109ae628e2a620ee308757e5a4cb1eb22e6d15e7a42fe5294d9ff4c6c1e9b1fd35cf44303d99a5dcb7333c3be4879ab32cb7953d8" },
                { "km", "bd4c6594cd176fbbc1885bf73386043e302e6992aec0e318d0793bd86b989f86587cac9acf9f2ebd794e5049747c474f8071990efe09efa1b64aaccaa847e878" },
                { "kn", "c723fde1d134651563b9133d8e9cae45ab0274976c1c84804772434d4c0739ce18444217f58625ac6f70f6745376b3c962be1949f8bb23a7bd7d9907634c45d3" },
                { "ko", "73eeed2e01518730458783911e35c7e96dfc4d7f59ae4355e7a3774e51f2a853f8a9b5a47cbea7c6b01782f6e7a6d18df1cccc05002b5ba3f0ef025fbf75c88d" },
                { "lij", "4878e9b4a7e64506611cb9550f2954ee0535a7cfb64d4cf774ab7098a4bede2f323197257dbbe8924b28bbd073e411c708483cbe9fbad346b1666b4a318431d4" },
                { "lt", "9c409c29827bc8f56d7bb0fa30add526a388df08cf6cf16e3617d4d3655849a3ec4f6fdbc2e2040b6e469f376687446cf4f5e6cc14504c81368ea1844c1a3b62" },
                { "lv", "5e7edc0a514c68342e5efdab0fc6ef502d54325b3cf769d4549f14629e66a7367017be8fc5801c5c4674cdd52410f3705d8272a38d1e0482de0c2bfc9e8e3da3" },
                { "mk", "587ff29c5fcf8e57e8722e8cb865ef984aa4591a3b66fc666a199f8f75487f44ea1ab2c35dc40b9097fcdf2f792c38b5757d3bdcd9e43743536c66d1ab5f4a2f" },
                { "mr", "fc7dcb4869ea2184af7f6dd12cc0fd59b51a0477351d40bf1175eec5dfaa546ba5afcebaa79fe4b46c43d354b32af6a7110a9a11850e189da544c0103f451227" },
                { "ms", "4088bae517ddf98d7561f38014f6cb6bbd261393570ec83cc5fb2e9a162fba70a7cf46987b031718362b0b7b0750d7f333d248cb4ca4ba2f6eafe633aa629f68" },
                { "my", "1bf34e0ca19554043da410bc3b8dd5c663f32fc71ae22bd7795f4347ff3741725a487e30833a121173e29c5066b14ca6275089218b43cbcdf1dc793c4726b414" },
                { "nb-NO", "3d53d1a9cdd056652229ff788ba3221cb535faaf2e7be27385716f76b9d321d84ce859bffd3d0a5044b1114eff4fb85fc75d7994cf74ee16971aa0f1f123205d" },
                { "ne-NP", "7f70f6e3b049452804e3f74321d519f6031e22029a724b6dc5684907ab15947f041bed42125a5cbb0e2c91ee890a1ddf12e251c1c85fe3c0a66aa0c016212c24" },
                { "nl", "3e0470419cbe88ec504541dc56763ee220d4240f3572633bf15f359ad4d73bfd81a0ecfcaea9d872bdf053ce1b7825f1716585512804d55418187f820e449b0c" },
                { "nn-NO", "f8eea505091a1b444e5974fb65eef7c49e8feaaa006b780d7d033e6193474efd35cdfafaedaddb11f8cf1e8c6e3753c100ebaa86d8f83563839f4965abfb5a5a" },
                { "oc", "69e02f87ee5cff2c136c26c84e43a1726eb4c6823e8b8237889ec9a8ee06f80f102fcc9def96f85df23466e50ea34c87ed39e7dc8436aa75a476f0de6329f620" },
                { "pa-IN", "8acf2ef7586fcf53b10eaab41c321a34587cb3c803df3da0dac7f967f2492169286f4cffa6d30b95066dad7d9c8b0b720c642a90481f16e15fbe298605aafeeb" },
                { "pl", "2c176de38904ead2cc668f82a3af36e72b77366de6cd4713c13dc4b62dea6403297c524ac178d59c2df0f83dd2a4442e6746441a889dda4c482c17ef420cbcdc" },
                { "pt-BR", "15c819112e631bab640d94d95bb0415f24b35174447559f105ba3101b46642535559f42b91c01c9806933178f5062ced0459f7570a6c5325acf3e0abc052ad17" },
                { "pt-PT", "34f6b39a28655e61ab2f2775a3254a01548b3e27d7d98694d3ad6321c6e2bb30ea00c22ae1e7f2e6efb1c8cb8c4da15c5bd7d086b7ef0decca0b77b38dd9dad3" },
                { "rm", "9242070395786432b7a03906763261d1596f083965d1f1e5d1afaf24ae33166385f176ad84ded980c2c99df9c21b0783a450f4bd67ce84d9a5409febf6264e6c" },
                { "ro", "a3b4cc18bdeacab5f48435434578e1ca8cb306b6d70b61805a995fa511e0fb4d169bd9fad2885916f7efb40d331e7cb9f66c5497587eca3f5c4c73b51b1dbe07" },
                { "ru", "bdc55c96d56c677dcd5926525a0298c4a65e82971c5c40ffca11de285ad747679255098cd247e3e3fe643b8c24738ca28d51ad11fff58649e1869d2f1f44a898" },
                { "si", "01e629f34e8013684971625d29988477e58b1c649ac05731bf237ee97948d38ea319dc9c0eab99b201dfd22cda8054fd942294d9699b68b0314041973af652ba" },
                { "sk", "10dc6cbf72c08baf1f8fca8302a81ba6fa0424d012348b4c59d1f098eeaa4f090e9144f09b43e290cfa3f50b85eef8686e2410e7f8efd80daad36ec44d30ea49" },
                { "sl", "6dd80b6f050ebe38bfd91abae5bec30f9e55cfd7d7500e7dd6cebecd5410d19f95e2cacffec448b1e3b7074ea8cc472382d96ae6d133f8557479bf1a09b898d4" },
                { "son", "5d7cf85a48d3ace3fd36d40c64f4d8b8efada9f94980b1e2ed17084e51c10e7aee261f1998bc0a4135b260c013f818924c82f4e446384dd74edc95d43fc3b2dd" },
                { "sq", "6204edd160c7ab9b18f93ccf4f8e3daa9c8dbe0cf544eaf47ab198936e4018b5cdbd8735e4b4b272b32d49cbf9836d9eb543259cc4fa7d6c4746bd06ab7e0a6e" },
                { "sr", "8f8c4e06f63367d47a2db762b0590676e1dcf18d9309e5c3b387e70d7ec79ae92ed1de579f87d0d28a57c9f88807bf710c40b220ea0adafb4e2873fc56351f7d" },
                { "sv-SE", "4e8c117a7bcf2dc3285f62ed59a5a67458e1ab101441a9a73dabcc7f8ad16194fef85942fa1e6efe1ae04a061b16ccba7969edfb89ba2f2a49df1a57d3800d31" },
                { "szl", "f9a4b8dafdeb16871978eb91ba148d2fc28257aa5b9ffce09ee956bedb7eaaece5d15e594b97933c653f3a60f3f3f6d10ddd213ded21d619b92b3bc7131b9f55" },
                { "ta", "75f1b1507def41ffd8b357e4a3db2c5b99ebe46540c621d90f7168cfcfcfcf72d9b97a70ee5005dfd170b5ad5842935ee1818eb8a25070179da7a901f7066158" },
                { "te", "f752307b0db8c7215fd335e98143704d7b56930b34ade61d719b21ce619dc157823e3f38775f5df6cf826e7e0211ce8e00c711bf184a8bcf224913094e3028f1" },
                { "th", "3ece839e64e8f4cd0a87f57ee4a64cdeb30d8f72d7996b0eba2c0de0f9e646881ef0574acd329e3266cf6833786dbc2e873b854c0883ddba4f2de4b9b620a0e6" },
                { "tl", "db934beb1d6c1c37c20afef04c35a5d2a0c16edef2cf9a2730a10d853c59a896d2172c259713fdb847f23b2e408608112571aece75bf15911172333158efa880" },
                { "tr", "a973460f160cf2c8f45ab5e3ab84356a6426e84b263cc54c6120417df68515020a09694b4914dd05b06fc3e2ab8a77a12f84134bbfc82ea54090f25e3de95665" },
                { "trs", "a6ff7b0a311064649d64cb27435336d18e8792a582087f3383c40e7325cca094308b8b200dca5192b6c0cca5e40fe39798b6e3daaf68650e0d6a36ce200c7c81" },
                { "uk", "12f310c1847229a6f9b8e5ffbf16318f03a9affe7e47b04b3f11be368f7a041c067b0ea3f08cc356e94739b9da0f3e34bf9276ada2d21d1b82937ba3c10ccdc6" },
                { "ur", "672cb789d6303f9720ad6e38b4f1fa428dd9d2914a7a6e9bd12daea3b5e021eb3e1e76cc6e4ea595e2322dd4a2b6471756fa4979d17ce63399f8a9d77f95517d" },
                { "uz", "f169fc280906e96bd30dd61752fe3069911e2e38b37be750f54d9d2ced340cb17cb4a20fcf574e5555153dbcc04c38a2bfbc204b2743c3ff3a8efc21e0ba937d" },
                { "vi", "ed37f56adfff8564b2e03cea915320b4940d10792c65514ab5c178a4054fb70a3d92d5672eb9f0bb96b496a4a5b632f35ef1d943a29c7ec614bf6d19b4f88a58" },
                { "xh", "5550ef06ececd70d9ccef82537e0b1ef0a171de3e58a2f34e65f5e6b8f989d83ce100c24a259ae22eb5eb9ea827e038de8af71b0adf25d2a810edc6c01e6d75c" },
                { "zh-CN", "2cf1ed594e7280855a05204c27e7bda8c01e3abc0b82017b45e4039e53496b37217f4c95b81def081eb6590666b32924457fbbadf56c9e491a4e5df726d02edc" },
                { "zh-TW", "1308aff8c7594e518dd6ea492c643baac578a3b90bfc6a47bc36cc79793c5de690ce04439200be10c9600f10cca6533ffd83282eb29af718fa63e5b020545194" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/90.0b2/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "d352eb9648ca8e47c674b8c1893089ebbf0408aa1f9da77d45d9ab9b1832c565e4d95a6c2610e1d274b97bb13ea17f80f9a214d9ab5c62711792da4953265b35" },
                { "af", "370ed46aceb487dcc64b13ac0fe998778e17af435d46af2bf33fe7c9b8258dfe245432b69f2368bb949944260461db4a6a66c476c91055646f129a03b76c6b0b" },
                { "an", "fe6a36531ecf7153175e2d675f4d55fb593510a7a76b68970ba2ab9e3c1ee3df9a248d34759078324a23d85ffa5d824a5e8511ca912a7f6075e274b9aab97f5e" },
                { "ar", "cd36da625a15ea26670de5efbe93a53ca8b5b32c6194236857cae655385dfd10617e065efb7d91681d50cdfd0fc5af3c4d67eaf8f74655f544ee9b50e5445d63" },
                { "ast", "65543bd0f67f2f67ed2c9983d7975880b68c20200af827384745eeb717ba45999093c1e9ae588e2b95680c3955c6f06c377b204fc70f52eb7cebb34b3dff9491" },
                { "az", "2f2a73ed0acb644f99ccc4ab61aafc6020742f8950eb99d32f0ab7e0e986659dae62ccc6d9baf09848465ea973ac49e0b6ed87ca6276a15c438ebc947afb398c" },
                { "be", "d5ca67fa7ab1fb45e7f2e7010e57bdb9ff60e467085f39ff6454f10ba371eb457b5b340e2f2f85ce840cc1f0032a7ad46ea28d88e87aea144976db316edca3a5" },
                { "bg", "4a0b75d03b4d690d52e86b29d6b7e424f8b7feee660a82256d7ca12b592f706149e67a50fad8cf91ad93ebc9e15c95ad1a7eba60b1f47257ab9be7e7f876f711" },
                { "bn", "ae05bd5ff1fb67b4fc4892e66643475242a9282d7b34c03ebc42213cb946d99273cc372512f649122bbacc15a2489caaf7cf91b567fe9e3aeae1a80545f1b8db" },
                { "br", "300a7344e83d36cb1f026b382864fb9445443173fc4c43bf56bf70560c1c9412c408458e8b2981aaf414a68c22eb90a369498da569f138f222f456d46c498ca2" },
                { "bs", "6e0fab6f6ceda47fecbf4469c2e6796568188ce77748a6c96da2d1495fc38bd5c08b190474c252e694ea44aa53d1c422b72d755d5dfa866b3ecefb72e22b12d8" },
                { "ca", "3bec96f233806576192267063c854370235f849c8349beff617bd99402db30a63b32d62c79b7df717a2354aa2aaaaaea9885d01fd2225bc94af7ecd97936c4a8" },
                { "cak", "b457f99cb92a6c9a05fb4ee66e7dab83a9f293f80f356fc307571c8620fb39a8c883b364e79d8674791a737a129ccd78265bd7355f476c6926cf7eed08bddfb9" },
                { "cs", "45b67990df100df34e0958d7093173c6126e5807a3c86085545eb1bbbcd42feb58a3dfede1264bb65a34d1df807db25c00263361b3ad485d71ede19a1ab433d0" },
                { "cy", "748bb8e5d25165092fba1413f604aac247bb2bc82df76398d86a070c3be56200a00d98d71e0e2919202655f2cd1092fdf481e13c0d15c02488ef293fd6af01bb" },
                { "da", "ca9c0828257204705def5339899f2484dff479620b6e3256f6f0151a65ba8264868634d4a3a96bfc52adc53f7817191de0fc4f7aa66cee75055f3f4d72d4210b" },
                { "de", "e5371c91327cdc15c1690b663b3783e6e29ec2cd25c0536c7a32c6aa32bb595bc206eae7d5e6b14c5586ad8409a338416f0926610c36f69f5a314b2f1fe3ed69" },
                { "dsb", "1f02b47bd3e7f65805f86306dd6275735c52d56f16f67df2dfa16dea9ae08dc481d9e394c575745a431119cf4137a7c85ccedef2a10165f97b4126e24399faf8" },
                { "el", "cc8dd6cabb434d25fa59de744c37405e25ef7468ca1940a8d1431edc70ccc0332aa39c14f5307256a1382629aa22cf384b7eee705769246b04ece30b4baa480b" },
                { "en-CA", "5df97b01fc78952255e6dd8fa1af6d7ad4a30d0322d5cd2d75d8eeba7e37ec8bd054eb1ba59ab712cbac205f0e96ce5a3256c8013fe4f07e6f0e238786b1200d" },
                { "en-GB", "fb24ea6955cfe6b2ede957aad074f37c29a7d4fc92b0f81b86a35d71834b5eb222d0a3c6a2da2fb031fe3ceb7f49eee71aac079e5893d84cc760df3565ef4ff2" },
                { "en-US", "efecb0d3803646936c84d0d4940cb26e5c498582fdc3eb752f93a72753d7e1dd92c299aeda5ac5e43189a5c0fcad3cbc5dccc85a3096a8ea3b803cbe76f97472" },
                { "eo", "032f2bbea2cd901d227e2bc42e111333c002b74f2768901a5480c904e41762f42a129e4f139d39414e7fe1e0c1533306c7e471229b20c0b7fb6a14dbe615697c" },
                { "es-AR", "8b67c6ad2ed0d10f5c7a645f1d2491d92dbffef1ba18fe5b1e3e3c38d233286493db275d6256b6bf8f045d79b7e8c2d06ee495673a0a24938c2638ed20143ae4" },
                { "es-CL", "90762903d635a0b9e76f1986100c58f6530b516c5c4ab9871896d809ec760ad1f16fe85c606b1784fdf53829bd9ed02b344d63104cb1bbbef1c565a48c461ead" },
                { "es-ES", "2c33d0337e2a227377b1b30bc73b44f5ab74c74e071c35d118cef580e7383c36dd175ce2450f69c2f673fa9a9ed15cb55c51c8267a2fc3322181c368957b658d" },
                { "es-MX", "9ee44927f546ca8a39a2c56a7518e4ae6dc28887820b20766e2f0289c15829e7c9592b0ef791e7f257959caaad1740b3692d9d126f556f900697ea1a0c741399" },
                { "et", "a5143e909602f737c9096d78e01671475f2fa5c69efffd75f62901217de3ef2c3f8b28780c6008e7ad709a71d39d345232c001637dea0c604b87b288b1aa12bb" },
                { "eu", "2e2a04c79746dc3be4900bff9859536707831742ec565ff0e051732c17bdd69e7f3eaa1ed727b15d697ac7d5388f6304b0426015602b9ac9cd22b9a7032e75ea" },
                { "fa", "d99e0fc02fca502420585acaf0264a8ab873c783801b43c321ff0fd7ad85ee0b536371c1eee20090ba5021ef957f5b8a7983e36110d3187cbce0238a571b4c47" },
                { "ff", "26ea0432790f4b553c1f3df66e9124d016355d57ef2213f67b94ca029db02b4bc5f0a8371affa4cb3d4aef845d21ee64dfb6f110b72eb309e52fb7d6cb8f3d9c" },
                { "fi", "64b241c5fa26df6b92dc7ff787df26e9cf4021614c934a84ce5cdd746c74b38cfa50cb5495b5d39224b7aea1b42cdae7d05f1e42930ab1bf5b952dcb2635a88a" },
                { "fr", "4a533ae7f9a5d50e4878cab7607a251b9347f7d757bec8cc69f6f52977fa1b160a2d16b70588b073fe5ce7a1fcc070d27bd6a2f7de78133d6326d074dfa49809" },
                { "fy-NL", "94d3a59be9099ae4d1a9fb0e6d1d2d38f79422db6c2d196f16ab69090ea76d4588fe49bac3a7c3101797b1edbe811a579a190bf7a0e23eb7bee7f3ad13ed7868" },
                { "ga-IE", "cf2e155c63bf9933b81e93576680f4a6736f8a11c230ee6a61da1e2d7a1f7f6dde8ae9ee859e3de291506e68f5a4af90e953ec573cc02b7cdaf56554638c0724" },
                { "gd", "576f8f21df80251ff97b26c594e48404df5b7d101d1e757c606b140a2e568647c835791c043b5540f7ccb702835f6fa4ef889683708298c08cc97e20709d66cb" },
                { "gl", "417e2584d3e59111ee7b022d17c883d8b0ea3b28daddd7a70bdd337b9b38a10b383d48868c88814f7a4ace7a0a18c2d639379e6aa9ff930b0e00bb5118b26475" },
                { "gn", "522445ebf43479c026f96704c3ac966ee49795585873a4e65e01d8eece109c1b9e84127c49be61ed127b07215740600fd81d88a74a2e5f8e9f133edfeb571d38" },
                { "gu-IN", "79e37ce6b10526b2c404fcee9279eed753ab4b323f0c1cff860fe762d2c7a43a66b122fc1b346cd8b0c4e620d55f05d48577e8109df8abbc6a9b2588fc83341b" },
                { "he", "3195af8ce59ea88d664cc142e0fd6fdb1f91d7bc3eab4192465100c0def232723e9b11e0a10c857f0bbc5b8ca5d6bc4468e75da908249ee6b4ba6beee206da12" },
                { "hi-IN", "037d2d7549d8a1b6f1d4f404c66aa96ea458fccbf12f69ee20d92f12ea8f662d48d6cc8e6f61957c1f08070194813225f56decfc13e69e9d4ebd04942a5c611a" },
                { "hr", "2dbf7b77b767712ca32081c426058630531cf9b926409a8e3e721e4147f2092d5e4f138e108b6649d6139b47448761b1c2529ceb707b777e818c250967126c56" },
                { "hsb", "b98e327cf2fd65a8ddde0afe1698e897b5d004e6f79ca1ae2158ad0bdc909756387ef4b8271d1881781efaee904dc7f0857ae569e3b1cf1b3273c96eb32ba4b8" },
                { "hu", "b8d22be9c19aef464b27bfb5d46627d19fd3a9ddccdd287735f3c6686971449e362441672f7d15b9b0df0e62a3187bac4e7f5b8bab375db173b63fdcd1fbbef7" },
                { "hy-AM", "7189cae32aea73f9f4bc4069d9ee363a58ecaf82560895e4ce9973c759a93880083ef1041e3d4a62e426804a01d0de324923376552e3644b6b9cc0d5d7646d0b" },
                { "ia", "c1c6f8fca7ddc66c4644865d5e1c91082526e6add400cf8c04c6931a5d856577ae067dbb70b0805efb5dc8b0196c70ed0e6c7170ca875af9c76d75c4601dba3d" },
                { "id", "ecfd96371edd7e120c7c0e5dae53e411bc58d8db66dd64fea697b5c88495c858398da680076fe2878e864537e1b743a2fbcac042ad0b0493561a4fd6ee7e083d" },
                { "is", "5f8d103fa539de5aaa7706d64694a9f12f27ee2e5fa2e920a3ada31b57ba6654c1fb064fc258f3824abe562fa3ba68b0db850070b6010fc247f352f4fb32d853" },
                { "it", "739802a265a145ca8d5702d2d8798b2e8ddb1713d22f901a9970515ec10ad595ce9b1ea611b65fa94f2f1a61873aff5aa7e85dffedca1060bf8596c3abead6da" },
                { "ja", "cf8c63ed9d4040bb8940e3671388878e403e4f58d156cee7751d037fd46dd340910519d7c83ae5e173096de2ed554188ee3c685c9c48c8b5a1b9cd3d8d2f3f7b" },
                { "ka", "9218fe40af591f79b9392ed5eb8b49604dd65cf2e4463f2390fa26ee223d4fa2c2e49c1ca29036baa287fb3b71fdf056a0c9eb4da5707a6eeaa306da736cfb08" },
                { "kab", "22a41e86f6ac116856b14a0c5e92cd3a7952e1ad25dc07c7767fed20eeca01c805a11ef84dfdb5ff764f70bbcad76109a82f914769d3895ce20dc1d2c812908a" },
                { "kk", "aaaf08cc568081ea9151f266c894b004f0a0cf20bcbc1fb526b2da4a6504ae5b995c1b0e5037e6236d77bfffb232ae8c0e55a72c3bab232d6aec3547756c804b" },
                { "km", "5fe1f827159e6385a7f8bb2dbac93a00e972080f432c2a0071175b97aa847beca4828cabb7c7966bb098302b159f8eacf9883184e919307bd73935009ffe611d" },
                { "kn", "fb8ed3d8dff35739a368b153bb026905ab25bf8046b9771bb315926df82f9c04e0291163e63d0ad6a4f68ac1f828405157f75b73851b2590814b0a894c930651" },
                { "ko", "a9bcb031932ee90f4e2ceed49f6dc9baab745f3e7cf1fff706d71c654298a856545ce3d716c67a5f71417c0490d4803add5202105c150c3ed5bf47a7d654bc4b" },
                { "lij", "e961b4f7e17b5ad5189abba873d063b163b970b62d9a89563177255610dbd20f47d5bbde73d47be12c2367857f51e5c4ebfbe9ce10f51510a705f8f4706baaaf" },
                { "lt", "c1869b8438ef4b4ce233b6458b33911ecbfe25794009feae8791d1257032de9f5ffe98f635998351a89868fe4bbdccf99c56938eac663571104ce8ac8ac87719" },
                { "lv", "697cbe4e8b4f5f8b9bf727ebf829180f583c91c7a844586f9707a8da834711b0cbaa28be2809fa71aa12ac971ef9027a1d4d127d9b48fce86d121db518b61c6a" },
                { "mk", "1bbbe7716543ae1f667cdaa3d5fb9e78ba2850a880e2ea0a37d6ccb6ef76ed3b74bd2bb24fbd3d53913b293b3f69bd9d7edbd5c9eda42b41f43ee098ce254361" },
                { "mr", "f74a32b20ab089736d3d337a7ef9333b571c0e23489d574ba6c7f65d8db9ebac80f5f2b060d44c76697819af16d52064339b163ebf221ce1312178261f201559" },
                { "ms", "6ca7bb66daad87a1bae7cf967d75c7691557775337e2594594888afa347db11112d12234891a37f1da80e0cb2d8ab004413af7e5fba06826c2fae67c38d3a728" },
                { "my", "fc47b2d8426adee573e659221cfee778b72f77a4d680041a2e589725d90873c76fa7cb4eaa44261d05be550d9a8c391e27feff5c9b0dbe5b87b9f0c1e707d16a" },
                { "nb-NO", "ede881dc3c4e197f9533d2bb024f3cab82736530897a8ea9d38bdd5ccfc53275724bfc86492758bcc1c36fb78d6d16c60c856b4fc2ceb097b5e191adb04bbf63" },
                { "ne-NP", "15d05974cd4f8548e70db710ae734b779a59a8e9ef4e6bb231f9d72d3716d6ba9a00db1e0e2d2c4cff34ff19afe5bb778d464569a2d0e1e811d9f6ad9ccb5088" },
                { "nl", "fa25fae8c02e8a2963972c60a9f56bbc0aa786b551d03e25b1db8c0974735ee9a5bf3f4786935ad6512c20d497794908f6442de7d76ecb1ecddb097867ce57cc" },
                { "nn-NO", "f46f1b63c122cd288a94f56d92f7c650335e08ba7f34b6e2ce953bc1ebb81952d7e870118143285993eda61499f78f8fc7c835cb3c7a27620bb2d9e48befe59e" },
                { "oc", "2326ff517c3f71c92546a55b714be7826865270e70ef9718bc9a23843d8fa60d2021eec8af7652c1a5246ef9c7f5e4ed5d45aa9cc7cf4ef1ee8fffd02a76d4d0" },
                { "pa-IN", "17dfdf66974648facd852fe5bbfd1cc3ff9d458f5e7926d211844a08f91c596e360c070fee6a68f2bcee16471e72a8fb35e110452f4fe96dbb1fc3dcc6039d3a" },
                { "pl", "0950490edd8f7f9746b8e9ae508dd311e2eccd012ad93826624b9c8f81475a9fec6b8db03e2675579dcaeb71286f4bd326eb6b446416c2187e22ea6babc0e8b7" },
                { "pt-BR", "11e0ac4401ce5a0c0ff9a2d85f00e5df4f2e3ac98271a5f9f91158eec076efa114d9c4b95880297c1eb3f5298ed920bb2ca4840e1081aaba61b5b3cfc506db75" },
                { "pt-PT", "3406c4560d7e8b11d09ce16fceb7503572e52f968685b97c9fb63de2a1f56f1da8d6a11082c29c93a6f2333dfc08043ed146f5535cc6f12ba252de960aaae01f" },
                { "rm", "6444fcd0fd4d4148fa0bbd1052e39a7c0c3549095ba4c05e3a7303545679be6d239ba4488c9258f26ca2708c5a21b63644227b4ad1217308bb255a515ad13ab7" },
                { "ro", "315895670724a9c0d9db3fad4a85510512f699a5fc79c7386f3f05936cff8b04230db75d289b132d9a80479af9f9d8055ab352823dcd0c00e718a369a675ed2e" },
                { "ru", "608a8f52f295e746f8aa21e8322987b1384639026fa2b4924f80c277b678b4823f5953d3bedcc1ac22b190b4ffd97303395d3b2457d96819e9d3c9273ce91541" },
                { "si", "319bb31e62c02e34e41d86b06a9bfdefbef1056ccf82344b702a53ca58c0c9f636e24c53d29f923e39eb697774a37001fd9139e81f9f50d36c3b0811105edf7e" },
                { "sk", "bfedbcf1f6c17073cb9aee2c501f8768302d0310c142ae675024ffcc8c693abd691cecba826380e14898726a78c06d9990d0d3eb0bcee065a94333efb6581f07" },
                { "sl", "c2bbdb4ed3b759fe617f78c2aae4935191b3163793382ee09523bc70bde740611b768b4a2aa72bb3dc2854509d11249770774bda001eb7590866506e0a5fe7dc" },
                { "son", "1a803fcca32cb04dff78a12b37af44aaca289fc04d039293a180e5b4925af3193ab0ec8a4535ed3fee715de8717234ea0a1c1150b204504882d2c95a386726de" },
                { "sq", "310fc830a78b71110f6f6337173c0eabcb0dac00ee8233cc986aa593bd8384a65f137cac266bfad8b1b9dabd991e645f648992ac5941a196e9fb948cf69704dd" },
                { "sr", "bb48650bbd4f5ad84f811b4e4a971dc13bd4a7ed7487626c7e45db9ca5f334d44991382deaaf66280f6cc141f2bb497db66e5196961be1e23fc510d1076fe329" },
                { "sv-SE", "9f6fe3062d3f8dc6b39d9071feb5e0ed7f1c9c8271645dc9c38f74b25733aa81dc719df29b0cf050cd900a2fc6dc4b7b7686dc498096774c40eda7d20c6d5391" },
                { "szl", "d5f99064035e4a68fa30f258e0d292b66783adf70c2d4b5732d13e5a15d92f06966ec3668b3c141ad11e2ad16a637cc7e2f25a749e04aafb1f2ed121814f26d5" },
                { "ta", "2e817834a2a43c049f8ded38488b136344c2c3df4f4464450bd28e83183ce9aa4f1990583fd0478d656e78fdb64385158e51a041a322aef37704c5fbb27e2b57" },
                { "te", "65d052ba8c23f05c2ca6c023c6875e04077e63c06759bcad53b3ed3f40deb422ea27fc3b3e4600b5579d6724998887033c0b86c601219863c6b15ca90d570877" },
                { "th", "b9a8398907f41df950eb83019ff194343d52a28c2e5fb853db16214e008dbd92013cb4a83dfcf7486da1f5d1e8745dfeaf07001f4a605da117a1d73be92b9852" },
                { "tl", "fdc922e8f57e733dcaa826aaf40a96e17aac742c18fb6ca1002c7891649457143a4e30bee3e2689cf78ab531f37d2e6a987ec3b241d270e6bc58389f3658fd5d" },
                { "tr", "3212b18d0da00bb36556f31dd54d84bc28229d824ad81a841a4415d67df69508c66cf4ffca75e4ea8175d1db2fdcfcf1bc7971cde528aa4420fe5b81e993025f" },
                { "trs", "599511833b7c828149a0880ef75cdf92510c9deff44f9b4b8d148ff11333a72ee3c6019a52bc9863edca0400bf146169840366bd6f84a770c9a06a07aad348b9" },
                { "uk", "4161165cd5072e42cf8ecb722dd3fda3192278b2e2e2c20a46476ffe3af0d70ad4fb661b8abd4705794dbe12efe1bc076a6c2217d86ece8a5f8a28b10aa0261f" },
                { "ur", "b931599f44ba452ae039fe95f7447052ecf547f6bb25604691d1b34530ccdcca3005214ea54fea57e494cd6a6d17e601da1e6d90eab9d9de7d07b00778a4c337" },
                { "uz", "484c2d4cf0becb0403a33cc288d2df6dc92390a6a5c3272964136180d3c69ff83df797a004381a7dcba8fbead5fc24e4eced1da457fdea50bb54975e7469d736" },
                { "vi", "fbb7a5a603c37a29ba67f7f8b20499c61bd620d4b0fb4b68e60f05dcf0cd10d458f0b0e8e20b9fc021d1d72a70244b6d3fbe53af296df98e304c3969a873c37e" },
                { "xh", "edb79756e68ef88a00c0bf5943f16263fb18949d67457f0c236a5110a272486f13808470ff81aa9ec5c314d7284b75204f01015f3f3c092eb0486b0a2c21d9b2" },
                { "zh-CN", "e865efcce69d92bdb5766d58163ffbb21ba7310ca910d26f569b18a3f18e1b85f418dc8f42d800ff5cb4b50a92bf3db5afd354960bf90f345246f027d9bcde2b" },
                { "zh-TW", "48c85fb5de1d02fbb5879e400eda855a9f2acc6b2943fa38ade7fc2babbe74927298a666be0d65cdbbff744f2d9b0636e9db80371cd8d9765ea164793c957434" }
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
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    Signature.None,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    Signature.None,
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
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
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
