﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "114.0b4";

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
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "f484c9e2f9e6bdf6473f0bc56f8e494c234d18199baa339592b0fdc0f4f2ef9ac3c32b31e9479a7d647a92906e7fd65ce54cc370380e3faab3f479fa2d56c00d" },
                { "af", "c6d5bc3d24a1005451ff18f5c942b66e94a66034ab152b438f26dcf24868374c27d59a438cc453a42e3e06e1edd749149801b79320770cd695e3d7a25edef4a7" },
                { "an", "87d9d03b75e1dd7ca94d3a82906f28bf487c30bdd09789a76fb2227c7bc473073d0c81fb3091130ff069a748c5e2af822df480ad5d760e61eb02f45933f991f1" },
                { "ar", "b4ddfa0553e89888c5a52e246e6fe5dc707b0a7f3cf6872f6ed86e52fe150374844e8d88c7284686cb3894bd0f098c2d238e87da2409c7ddd0b8bc729af98baa" },
                { "ast", "3ec88c7199e5e82ac0d43ea31ff64355f0a98177f28a0f5a8281535b5d7c1a4f086d17e12da78beb3a1a77c63d47786b387886697234d17f0821d54fb714cc74" },
                { "az", "ebd287cc8dcc0079f6e1a570fe6e59a5083f4083f51ea4e37f8ecc59357de271d50c178d51661da1455fb08d72919cf61bc2432d1dcd85fc1ac3606386a22c54" },
                { "be", "873e37062226a183284296e178ebfb607bd5688bc7f72231df547b995674a0edeebcbe8dd56583abdba2a9bdbcbeb7b8b103c4027d9a384491809f736cbd25f6" },
                { "bg", "6eb1917944328d8347e3751cf2c5734526623c8647332c6f6b1dc295dc52afd5cf58775b1fc1a4d5dd3d4c55321165334122de38ab013f4dcf1a82fb4f163873" },
                { "bn", "bd7dec8ffacd326ba3d6cdae3e2227f3b2c3b5ecd3a253474d922cefe37d643f2ebcc29e9231f3bcfff6ac4f40f1417dc018fb2818a1d47fe1ecf3df54df9d27" },
                { "br", "e7d9a46ae576034098ecb9cf458a2e6105f2a0f7f76ed1a3b0ea48792f262685c85f25de9bd6091a243c0e767c02c183ae2222109e2c06aaeae8bc7127721161" },
                { "bs", "0227bb58a0d9d14f4b6445465a541f9d6b0a602606dd7b2e2947b1f219bd17c706ba7862f70a7d9f658cba2311e34b326d837a52716d67bb36df20062aceb962" },
                { "ca", "17b7050f3afb7f88cffe0ff7b3e41e2fd5884b3e7b9dc4c20861532d63df1729d7318bb0edba18d1a732314121958d0592079164fe3be8ffeb03a5958c9a6f29" },
                { "cak", "69d32fc6d03b8cf394a4e3e0604d2ba585fe40bd355f7f8a7bb601a4fc02c3ba48c3eed7e6f5558cafa19fb50555f2598548a9549ba7122e37680936a4be3b49" },
                { "cs", "879c81124c1cda82f528c392cfa5f79f50a38b04c316ca2188cdf9b156ef30fc4d1940bb50c582a7e65350c6aebd8af156ce82cbd8d9af91b234874b993c30af" },
                { "cy", "a18d093647e3b57a6ca39e56d5f4597137da2ed0a50d4081baf2965d499847370e32c92071d490e17151a4ca4e0f015e55e333f4df7b01b901aebb5bcb4c0253" },
                { "da", "aaa17ed788af3f8dd305aca88056f06166695d4d0df929a1fe8da1e206f5e871892462cf4ac18360bcfdd3d777ba53a80e50ab371ad1a6fa5b57978326e53537" },
                { "de", "a53cf10df09f913a3e061b3a12956df95c19773478c4521359ba3f3f2cde50dfcd2092ffe4b1b075a14e2d06f0bd863cb97ae572e3083b631aeccf57d50f3b96" },
                { "dsb", "4efa1e2fcf1ae49450b2a8f6c7849555332029a46aa303a6c4619c66c0a0f931a6e73d270bb38fe72320a038acabf4fb164c75240c1fe63b3dff0f0381d26db1" },
                { "el", "00deaadce2dfd49b09c06236086349abdd7a9a5e0040b6fb1410af736123863651d083c45df36847f9af90526b42077c60a69c1e3d764e61c961d165b90d0209" },
                { "en-CA", "13ec0a77cab10ac594503ef4aa6c2b919d70f78eb236f2ead6e0471b5adde27c5332aab3fab7d998e542cd3b1962523f55a9cf7d2f976dea55cc3e71cd1ad8a0" },
                { "en-GB", "0f44b72b240470c7c09a4afb9064166ca59e516d687ddd95ac795753e51dc24728461df98ff758a2d2a8126459d197f01495ca43b729e8da09f93e723da69960" },
                { "en-US", "6331c32846ad2ad43b2abb014b83600eb8b091449fa6c4a8599018563430bbd8a3199e91010e1e444c63a90cf2208c5b9bb724c9623b1705dc0d1ee9f78f4e3d" },
                { "eo", "97bfa7296df2a677d8af33042ba466c1189148214f03aa99186c80898d34ad9c820061a7b84adc1845b28989ce1ddaead5ef0ffab24a44d0cd93857e7a9877d7" },
                { "es-AR", "2cca64cfb0f8d300269241acb710a332a2d73ba4f22a593bb1f46b741a83fe15b8631aba93ab067b28392d5121c7cec849854db9e393d52a15b37822ca02acf4" },
                { "es-CL", "114df6dcd40de3fe86eb0b5129b64e3a8b7869cd15234bbf4ba474ae8fe6fb3ea1263f75d30d292753fdbe2ab74fafe3fcf66fbb3130ea588e4b0f9653601893" },
                { "es-ES", "9343920716b2b30f68424b0049a31038c5f7f53aa71394f0b2af86ab913ab88efb838ad7c6ec1016f53649b75a17d45d078cff519fdc1ea33c73acadf97c058f" },
                { "es-MX", "cd48c40b735ec19a665a773ecfb151cf82a0a8f7781506b8008872afa809e43ead110033e799f262ffe6a4265de14527c80eb77d91c62305b33276b8912abeee" },
                { "et", "9c9289a0a41f95ed612472978385a089b2001bd0af1378eb3705cb6e22f86beb2bb4b88a01fe2c59b0eb0f178b4a79140e7f65452e3bfec4f16d4fbdb8462f67" },
                { "eu", "925337f6e73c054407d47e794cfc3903ae38a0f8aee96bdd1cef0eddb7dd62881d88a83589487c3fb11930d1718de7d369ecf307148effc2082502824e78af9e" },
                { "fa", "515a9915dcf2f3c1929c892603c93b59d1c53bd2faf9018ab4dcb20cd06ce668e9404ea6f544a46b9619116f25d3da89d45553fa4b6d3fb0ee4429f58fc2c017" },
                { "ff", "cfe40f2543dc62fa1eeb5612565b9a9e777a4ddead2b31703a4d3e4e0df847f6f9f9c56949d31a189ced3b922b0d419d6580904852431fe4ce90a34424dcbe58" },
                { "fi", "a30f53c371373aed008ed3d8990f47408ab844e4a0d078ad6e24f5cb22f937e28fd592c1f378a0bb255b9a94b6eb519d99016615d8083a64a94b490e6322f7d4" },
                { "fr", "fa34824d0a169b9c6b66c696031e7773c29efb91c6b77dce9f5303e9a6f12a89c104afa2e4e950952b6c2d3ca90201fcca49c5534f5ef508bfc178eaf1230929" },
                { "fur", "21411f347d4232d805a2a1240299bb6350fc567b6a89aabcd68cd83e68db1059ce427e8bc906fd4e0669cefa60613d7aeb0598df24944404ac873dd818edb42a" },
                { "fy-NL", "edd805b9efc2853012b93e3142e08a26f0732603b12663a61e3a46348a80a06f60e9105119cb8e2f06debda72b999171cce6a8e2f070e24410808646f40b2356" },
                { "ga-IE", "6b1b61ffa520070b748530e13c42b9cd631033e091ad42e59f5507926c36d4a598eb16b7bba31e2f84690b0e7a4a51d7b778336b3b927cde38aace6eee5d3b6a" },
                { "gd", "5f463d2fccbf163d49cdd205d76e027e400784a6ade09c6039caf519aeab97166da4ad719689559ebc21f5e1137e3a9b6f1ea967b788344e70e82a7469e7f4c4" },
                { "gl", "4b12d00e9fd627efcfb3af982bafb4b0419121c0baa4efe9484d58bea7049ce4fbc6e34a94b532a7bceefb119d4785bf537301f67b57fab0a66ca05aeb6e2fbd" },
                { "gn", "6c21a54b88266c287eb54eabcd67a8f1ce52aaebd5cabc269d24dd7f7f10709b5d72b010cbbbc39d0c487ff921819e38937dcc796ab5ded452f458fcc6f11eca" },
                { "gu-IN", "c09a88ee1dec1dc09b659391a6e9fca96e5bad03311e7467860276d7f68a4566725eb775fdf10bf10ff9ae83618168af7194c8822c282d49ae76b1931feb9afb" },
                { "he", "ba91e99349fc25ff6c66ff4c5511139acb5831016a44c18340e3fe72819d6bdbc2d0ca4626566ce6c5cac88b7fb04bed88d7330b47f59a3007a921337280b828" },
                { "hi-IN", "56db308f54faec346a974622e939716c1322583d5838af8dd1aa548fff5e1129db9f221d2186caf029139676850931c285b7fae515f9cd6ba981623df657ef70" },
                { "hr", "3dbec4bda7bbb7fced1b2c39b03c84859f72ca4e3b23df2616c8287b001423d2e920719e736e23092d65d9d51e3eaadf743ef91937747aec1050dbaa94c44932" },
                { "hsb", "0b82eecdb49434ecc2609c87ef5ba045e0ebbf17264e9fb1ee35730834cc409c9672b49cfade5dfc761fe488d3b25531703652fad36cd8700ce126d02d6a1639" },
                { "hu", "8b3054d3b93dbb21c4e415fa7be7626d1b30f2eb036f3983508a92bc1e9231b7423b1becd7738c2f328f41c6b28aeb7004eb25645fd4bdf0b73a2abeec3091f8" },
                { "hy-AM", "aa4f0bce0439ef7dbdd48898c4998aab698ca01be67eeaab451ca5cd4f86529e36a1de1e2b615fd41d4a9a8f1cd347a566df63dc98f7e2e93ef4fb25d26630fd" },
                { "ia", "bfcca902b086d005d681f2f398fd327e74e62eb0e16c82b4f4a507815c69f325162658b996797d58dbcc2019aa80676f66bd1d431ddd5c2cbb86f48faba2bba9" },
                { "id", "391da5deabfed7a43af81be042ae60f68839d2cf8c53db1ed4634dc5dd6a9aea2a8a0eee3593b27f19e4d1f3939b24edf2d2540c176ba1dce18660db59c7ae2c" },
                { "is", "d7e81527d06a98f31ea2d0a13c76b20f0231e77df5b9cfd203f94b93c0fe2a8e014ee462d40fa2fdd345eecf3df4d091449f7b03bfa0ab73c4773223bc6ef71d" },
                { "it", "4c7a99f42b2a4a68bf52e616bc75cf7fb890715db8f767a59dd310879b566575368787164a114b4716a6d40037397161e9452ed0e3729b0ee1f7a6d2ce7f2537" },
                { "ja", "f0fc040b525f4913851f464549993d1fbbf550035f7ee40e3668405f82aaa7185c57fb15d64249c4009239a639c6d8a3faf96d1b309722b1dc5b94d01709cd74" },
                { "ka", "1e7e25944a0cabb20b2db8fd1c7dd66e6b22a32200f0168c4ec4a532138599231b1323259614027036c31290c957b41fc0adf0793e11e9a3db8c29663043bcb5" },
                { "kab", "262b8a49f73b3626c0cef88742b0957c40e4687a4b404ef14a1ecfb2709aa64165748fd0b207e2a1eb8864f2f3007c8d7396e2bfe5fbfd833520c6c1e43ac2ff" },
                { "kk", "1163f3c3ef70be6fbe5490b91c0db16966271b4ecad235e13c9ce1e1edc344a4e61d86254a7cf19b9c654758c1b7091cbbd971a24940f388eca440c85c81b0c4" },
                { "km", "d83d0f9223ca52e8c4d14726e0c795f79f8058bd51aca82db71d5672fa6fdf19174cc0407bf62da8ec03c2c4def2629b9bae48e69410bd02f93c198dae65cb2f" },
                { "kn", "7fb305b35ee9b96492f1a569933c527441585372ff32528a4825c42b580e60bbc60fa3b3d5b9006a3350d2f940a1461ea541338a9125a92b64ed9b8966effec9" },
                { "ko", "99633c9a0edf00761eea0c26cacfbf35abb0cbcd0d41b9bf7f1510122bf1e0cafe26fa248d9107c4bf2a16b3d59e8e4b4bb55ff3452770d6395c387fd781969e" },
                { "lij", "e9a19715289ac526ad5be0b8473f3e6ff6fc244bf9b4dc1067d61d2baa47609210f0102f56919865bc502488962bac0665fc3fd770ee05dbef8883b15e21963d" },
                { "lt", "caf285a4a0868b241af890e04f1e6437b0cc53fcf0ded9e98f1c55167e601164441ff8c531c52528f7bd2f1dbdf20be1c38edc21eaf979f886e29be5535d2ac1" },
                { "lv", "d3d0e593f4d6ef85cbc143f9d42bf410744157682bf6a3de456c4f25c418a6aa7e4aa272eb0238556c647e2c8fdfdad8a2d9b246e0874d7b99e13a2c1755bea7" },
                { "mk", "e3fc2dc5ee02a155d5a219d7bd4263503ee073891647688f7a52424694b0759ea99c3d3d7a6d1a20b9cbc13239327093aa2acd78adc4c97d1154205aa95cf9e5" },
                { "mr", "929cfe71d888f886405c94966f9a403991b90122a3b268138281541b61b441b2192f7005ff5ecc0b4851ec49cb2fa8c3dfaeeba8f62f5c183702e08d6c4857d5" },
                { "ms", "56090cdbe71436851b906080ef09f3ae1e896ea9d48b8ae30fea5e7c6da1e3ee5a087eb9cc411a787cb8a28c0742288af6e7f32f0fdda74ee1cc64e9e62d1460" },
                { "my", "b0defeaf34fa33219e07526d9864ac837ed1d1d83a9f6fc66428ae5435c87ec3590d2fefdbcca9b74bd67d0799096ac661f8069cc709829b1e58d15f1d67e1b3" },
                { "nb-NO", "2334dcf5db54a9755cf3a0ee23972c35ad96b1e79618ef6376bd9e5db3066e0345f9e3f0ddc5f34d85ceea4355ceff94a9e452b8cfed81d5ca2106ccb820c958" },
                { "ne-NP", "f6992a57080a49c1576441d5138be63645065e16958c7746e6a0b30a2e53ce78cc129705974c8bb066741bf772f9f0b495b02d61108cddcbc412ccbff10945c2" },
                { "nl", "93aef957ceeaaa24b8bf96757fc91b718d488f73284f6478c9fd0f05ba013fab773f55c1911318889e871f21a63c918a367abf65eca7f52b227c61076a0af788" },
                { "nn-NO", "9bb34f8363c2ad85c96f21defc4822761e210bdb6abefe7419261757218465d7f14c26c6bc848f9b53f4bb8c06747ef658a7d7ca815a94e814f0c3dd0669f886" },
                { "oc", "9e5faa3315ca04ae902c0f21b56f58406edb9a7f5b274bd2025ff15e2ef038ea1c22f9198da3f1e55ebdec9a1ccce3329c5e9a8132b958a9d91dbb6fd52f639b" },
                { "pa-IN", "434d6c103422b2a0214e1407c552597af4cd2d680d5edce9cd4daa44c668286a2e80c12f02629e22abc79374ff32376b52f996d152a354c5e99987be29096980" },
                { "pl", "b84995ae380803da0a9a7555a83b4ffe10cbcc2038b062c74f7195ddffa18c8b2d2712ebc447d3b29a62a01466e8f560e271370ce00cc40993ae9a04842eeecb" },
                { "pt-BR", "c99ed61c520f5574fcf244896ec3737da247d5f069a96a280bb259214144e35e9897f6fcfc5b5d9668a1c4359346f22d049685eb9fb5d1dbc66e604d4b8fe8e3" },
                { "pt-PT", "392641a6455307f3cb8fdac672be9e1d233dc04243d840c23e24ff7c17ade726e0f158a1b4225fe6f7df7366bed3f644c23cd65724a7d5a6579dd085c381f2ac" },
                { "rm", "5644613c93cd685e5faac07282c73e57d15a5bd354185fd5589e1e6af747cf927d0a43e713b50c37394dfdea675ff06a9fed3dcaff7ff75c3069ddcede25e0ac" },
                { "ro", "b931f58f67a6047373e409ee543170047bd23f560a5b0dfae9af881ee250e0938bbaa00c84eb323ffa880b2fd6c356044ac07b9b45c16eecbbf2305a32d11e62" },
                { "ru", "0d78a5d9f53a25d044f4f25716b36faea0be440da555e46442e4867f680cdef4d5b094496a8d2ce4bc13a6bc71ac672000ac6076b005d9723d0d77b61d89103f" },
                { "sc", "e2b64afca9d15038ac6a150be0d414495dad96b0f9b292f80ef8b0e61ce7b8e70eb2da340d61a51022524620298017052230d7af8413b13c072621ca02c33305" },
                { "sco", "7f548a3ada414e410fd4df0de41e7075301bdc6b183144dd6011e9e2ffe48f30ce46b76bad0571e069a1e0e658de026ecdfcc05dae5765d35c7290c2e7829e05" },
                { "si", "809e72e2578b37ba5166845e3b8df00f3077f54f71003e5eaf1581288327d687e7df4464ba314ab20fa1ac0204e90a3c0ff27d140e2f8b75163c32f4af89be83" },
                { "sk", "b7a00c6289ffda150607a4e6b6da110bfa76ba25f955eec434adcc4a77d51c921be9c846c6617abde2580146251261b338d0d783314f0498bdd8020c4b3f8d14" },
                { "sl", "dd444f1f6c28661cd5dd843ff82a5fe259e96cc20891340128dd5c86b780c37a26bfe41013e910b1a4e45cb6aad98de5d0e85b7a4d7e78ae1b640b1382ba963d" },
                { "son", "aae60078dd33b49634c1be28199c56e0d3e09b56b83ea1bb5953f3d12dc6c05d0280572ed3e156f0509983f5ba0f0a82e275de277ae391b5a589fb0bdd3f8702" },
                { "sq", "9daa7fe715990daae02435c616cc74c174cd3b19a760196989596dd9c533a7e040c3eefaf91e2815c6631ec66a049f94e6b9a9452ea634904e797762de098cee" },
                { "sr", "2176a2a2ec9f7bfe771c756fd99c03e0bab1af9b5bcc63477ed402424c349e0f8383e7db751313d6d4821ddb4b189f7cfca8ee03ee10f31d31c975891b08baa6" },
                { "sv-SE", "3e5111c71b86221d8146940153c4b4522190e9b10a0e4f8c77cf33c874337ed6449a913d8f8adf1c9892517d7bddce5600723ea4a7b67bcabe4facf2ee0df994" },
                { "szl", "73212986e7a9241b2541e8e44cb6594a8f09c3ecf1052b2616d98d4d3c48f019889ce036c7fa84a9f37a57857cf72f1186a1598aaa47ff5db69394923426a2f7" },
                { "ta", "76b9ab788bd232a96692427195ca184def1cf9d3225a5cebf3ec96d8ffc1d65336341fa9be106ff8a64897ac5b41c43de2316faa3ed3185d240a9a2f2d15c451" },
                { "te", "994b4b93db0b27dd9fd7abcbf0b555ebf4bf338515ee1e4719b70f3d91a75a9cbea332e29f5eeae6051531b48f2fc109a9516d6caaa43d622f3ef77f5d37f894" },
                { "tg", "053077f777357023d110ca369da257b05635a127c0fd318d8c0ef578d73730be2439785d406ce5ece2a8a8abd12c8d193ac69acf7068aaf3409184c6fd8ea79a" },
                { "th", "75085310e0cafca023365a36ad3c05fc7c0a3fcf3c7d910f59c9aefe466941446c52dfe5871f3b50258eba16c15f68016b30532fd3dc8f8e2d800321503acaf9" },
                { "tl", "ed372a9f4270101f0820e0030c285da9a36a965f462935e57fb3c929a8ac8ca5a44a72fe77200f02d26a06dab0b33fb30fa823224fb65e905e495ad6d2f16a36" },
                { "tr", "0d52daed8998a29d03983ef59a1d032f50e9d6f9d830baeac2ff0fd15795a0a6a827c969ea22541cc0f6e6ad4ec90c10f044d1603924654534d54c9af032300b" },
                { "trs", "ea9fdfaff9d9f0d60f0484b634f1c45f7ef8fc814bd58d5021c88d8b32379d0fc1a2b009096f932a390706a8b06e627819ad4d6684ed776ebdce75456e7e93f0" },
                { "uk", "ef46bb7ce0f374c1e7e12be59ca2e5cecb9a6477a4eeb309c3e0fe6487e5aad34da3eb515727bccf5e43c05896d8f8790595c76abb8f93eb393b8387dcb5df47" },
                { "ur", "2eb964468eaaf5c78e62e300b6a599da15e70698bd9d6fddd47098936eb8a95e22e2a58ad48a2bc6adbc60dcabb73ce594ed43e177451cce984cad64c42c271e" },
                { "uz", "34f3feac08c3a70bbc9e9595386522dc565489019f8f0436fdd8a49159467a7987c6a5f9454d9ab8394991b3bc160d248a1ac6292483f9e7c55589cc4dbfca1c" },
                { "vi", "d71732266d30a3a25d1e8834725e3f780844894d9ae2369289b38e3daf503d6b67a6e9c58d1e9678ef63e7928d83d6748e08b91090b4f7ec41afe0658033a4f1" },
                { "xh", "15ccbeb414a6f3cb9672ab1f1eb6b85ceedeee75a4a9e7cfabbad627472dfabd26697e9a391db0f288e8a695226b5fd1fa58cb5fd149c6151aa5b5aed2745ea4" },
                { "zh-CN", "4d5dad8633ac82b6cd747b4de8191fbbf8c68cec6c635098b567a2295e9c85707e1078789ca370017441e3bd8f6940cf639a374cb9432a674ab1a829ca137e06" },
                { "zh-TW", "b7f9b25bf2a35215e2cb18c9bd4d5838f26375facd7081c761b5e4a035d90d89bd9f52a430887456b15949f718c0954f558fde85a24f3d298ea3363ed5ef00e2" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/114.0b4/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "7b67d80e30466578027387523ebcdd2201e3bd40780f2885cb777b241eaf2ffc74da36da863453b4628b9484f2a333019292f6992db48839777658c9cee61ff3" },
                { "af", "35c93e6a288332eb2117c8a6288b4eb93bdff2225be0764a6014f0c6a3a2426a8a32dfe6479785f3a90396d4c8a3c87dac757ef202f6c0c248924876bb39c7fb" },
                { "an", "30ea71b6b39c177d2ffb4d12e74bbbca052fab816a1f50fef606b11d7e05516460580f0e766d2a9a4ff65db6da6d6a6e8c764b813c7e58950cb2d58a2d190a3c" },
                { "ar", "fd39efd0c4f11c7226da4058f298cefd743a3bf0625488a6f147381faf3dc6ef650b9cbde9e6ebc342045f128cd64fa20494745df9ac089845c49d6c28c35214" },
                { "ast", "e8efeb37041785f4973a667a0dac0f5a0aa4babfd863ec7747df623e835d0170bbfbf5f2c4734e25eae79ba7ef0e42b174ea169328dd513f6c7784b4528b942b" },
                { "az", "ba88ef32063035a7f262d4f34365746076691f5e88ab400f77a02bf246517ba859cc5480c69a2ad7d3c266e83c52737c19e5a9b86be20e22beed9d519b753111" },
                { "be", "02f1d2b1e192c4fb4e135140d2f6d94a7d80fd733cd63729d06ea08645f0369e97d3d57ec5c5cd4882beae1536d8d53bd3a5204913c198351ba7ff652feaebc1" },
                { "bg", "b701c06ace928c419d2a9be33c265a326132297f9abcfc3fda8b7ea45ca8930c65e1e186638dc92af3c2ef098dac988a7ad81437e4c24a923a0405aee74e9d67" },
                { "bn", "9f33b2783117f3e415a96a02f56c713cfb857f5e3e8a436e5ff74e440a1b51760efc24feb49cd119f12c3ee53a78110990f67f667e35476a768bdc4a6293b0ea" },
                { "br", "78e0978a20c10334817214228f1ac919634841f9d1eee31ab87ebd3a4491dad775ac2efcd5a25476676c2c3e6246711ea6725278abf31a28f03b0f2c2405e3de" },
                { "bs", "9cee1ab9c8b13fbc47c4e5077444c64cb910bdcf45fd51434ee06909f77097671842fbf45ecbc4cc15ac0f6551fa423890aadf6168e0472fb5fb47f0403db0fb" },
                { "ca", "869e18b83e934bbd2e3a443672cf335e46c711f9532d185e355184bfef73ed9436b165eaf4dc2752f6d964c20ed0db047c9b60ac17e8751c49208e9e71b00254" },
                { "cak", "f2d405c57f0113254ee50b0db144bb7b1286b68daa46cb87dd3f619d710a6ba1a9c0362876f93db28de88501f8964812fc1385baa78e934878e28edca39fdaba" },
                { "cs", "3b9c4820d692997246ea3acb39bf96ea4bac41496951c299f2958044b0862dc11b105434a649a57509b3503e716b0a66a99a1e27bba04705cd7036c091408d23" },
                { "cy", "abe880bcc54dda80bb661d30017d140456d553aa3cf11fa7f2fdc13c2be7acda2184bdc7c3652f50e803654e3fb416ec143927e98aba36e3b59339adcddac5fc" },
                { "da", "104f6aee84c7dd52f12f7810c592712ed2d95cba3589ded3b5e45119e53b222608953b0b72c55935c586e0539620467cc04dda982d6b445faa479cd3b29eaa3b" },
                { "de", "693dfc922f80437451500d608fd6a68244ea10b07168df97968daf2eb8dc842b681d324e81183e191bfa334b880f9ba28687a61cafa643374447df908d555093" },
                { "dsb", "678866e7f493d5509ef1dcc5ceb775cfa6853c76aa78b863446cedd2544da031692c2e6357f7cae10a8a64330fc2bde70f5df1fc72089a423482ea93ee8b067f" },
                { "el", "19c82cd33acd0835cf0654cf63b35398a10aa12041af90e2cf204279234882f6b78159d36ccd6f281dfae40d98705e4c9c9a51a8c3349e1631e719a44e1a65d3" },
                { "en-CA", "fb7398d5a62be556da056478ed709e883a38e906c8d7bd2dd10f71b634965141ab8eb1d0847b04b39961cc6272506b7444356b501df11bbd17d5ad2351d9a1ec" },
                { "en-GB", "341970aad4c08181566ab569af05fb88341800d769ba551ef1e2dd845b6b5dcbf3c4e8c4ee9e0b0e1192652387a678a9acd1a4c233fdbc829e8d084a303320b1" },
                { "en-US", "f43332be85035e4263d2c4a3375744e7e4c28afecccf3e9c4b5d786674f0157b764e58e44b1094468c02f596e7318bcfeea3b22eb8179cfe29570b04d3eb33b1" },
                { "eo", "fe17effeeaba60423f8d3c62a3e6375fc1ac7d8f92f7ef27866de07184dbda331b92ddd6a93dfe4c00c3755c27415d2d18a173ecd959168b725f614b9f256c68" },
                { "es-AR", "e085c733d5d8021c4a41914f171cc2ce676624c94bb4a1b44cae022ae17783e3c18f587ab2637b172feacf87d1bf7139ea7040e9456e31f78c6993bb3ae1a87d" },
                { "es-CL", "121bbb28118584ccaa698d8dfb094f326dba7f725885a26f5b48a8e930b3d52cf853873cbef000ac55b9a8a79d214addbf88141ebcb8a2b73a1c18ba70586e07" },
                { "es-ES", "eaeed6e3661072076e93beb99e8c4ffbea1d4807bc7ed2bc5416351482c92f78a7bcc710e42ca4c5afca6c5103651cc5ba3e97b91221ae9078ca5193e5733a39" },
                { "es-MX", "343339d7352c3ccd32d462b4566f89c56f51faf9fa37fe96f3a0e66e7a3a3da38c5c1ef022bd2d88922f467ecfaf3b82394dc6fee0e947be92a5552a65109ef7" },
                { "et", "e0c928c78d10c42a29e8ecfdaada01b9f5af6bfd9de737acab567ae44380aa8804ec186a129fca5d7ffe3bcf32c5b0b32d288c0a3fa6a2b93d29dd677ed05874" },
                { "eu", "4a547566bb4215e36f46cbe96566e08a702406ab0fab3fa2a7cda022c364a3e4081543b4cc0960b40f46534059d48b48c84d3cb428dd660e48ff71f27934f148" },
                { "fa", "8cfbe27d80edffc19aefdb5d3117e5a0877532fe26417263ab27ca4028696021c766e734c1834328fcb72e2222befc0ee46152ae5a0876fdfed31622912206f8" },
                { "ff", "745d6ab82c9ee700fbf44e454d8f7f78ed447bc77866f96120164f841036d941c611af25e56456d339f7d0402f48437313b916f9042bdbd5ec2595880a0c5d40" },
                { "fi", "6374ed8d91c86b0576d82cc9c75ea6787d4ae24ccc679f8669479d7de1d27006e17f47bfda6940f32e32fba1fa517ce88534c8925a88ca20f0478d609f5ae7ff" },
                { "fr", "54de93544f7ebca3ef3942e6faa35c0d144d05ab8079335fe52ccf2db18cd77ac5062654925b29b027a4a957c97c374f059116b18faf15636f8f4ba7c30ec15c" },
                { "fur", "25394319cf13358c924378ded76224da4dc16e899a479efedfdcbf7585235e59e3aafd5ceecbb254abe4e33d1870eef904096bafeb330649d4cff5291be2f68f" },
                { "fy-NL", "c163bddd14f2f46989ff0c8202626380f9546c9c0d0d1d19ac20413efd06be8c16d228316e4f532fbd9b4f37d24f0ff6b25fea19628703c972700a3a64c498ca" },
                { "ga-IE", "1235f808b10b8bcd6276951faa0b146dc7ee90c6e85b904e20ab15b3cf813184a759b64ddca8a5daf3477423ec38960420e0024e284377c6a944b6a11055bc6d" },
                { "gd", "5af664ee6b251fab39fabf27d8c43e069f36509901273014c224a2067c5e81a0dbb1d3b3fb6cd290277a24592d3c6178d6cc763715442d138661bc46a7670fb9" },
                { "gl", "5b1b76dba784282e045913fbe286c4f2cd19bd1383bde70b4d3bf9c7b062bf1f17394538331f9d9503f906ba30cfb7fb7fe8b107d0df988bd56ae374feb66a5f" },
                { "gn", "9be6745baed73f01f89f59ace1d55bb18ccd36baf6bea327d187031083430141f1462fe1ac413c2a452f8b498209d7a08042b5fca6a815c4b23dd7aacfbd35f6" },
                { "gu-IN", "a77ea8e1e9af12e23c1e4501c6af06125ef8bbfa80512741408f35e7416233678b654086d63eaf80f0991066db9f346a2267d092ffccff97c59786547a3dd5f7" },
                { "he", "7dd62dd30e37d78b427e03e7dbb7cc41073b070203d31ccba91e10a5d37bb9b80de6a94006cb466d22491664f7d379c9eb377454c3e38549e00c15b6cb6bd83d" },
                { "hi-IN", "203d7ce104a38e6ca79241bde0bb2c610b7820a06d46ca54b537389aa89c302c2c2bfbd2089905932ecb435a1062a96b54f3ca9668d252775ac6daae11f6fb14" },
                { "hr", "f79ffae3f1de5db64b3c7a33e3c0a6432499a5ac6e3073ff0ef5ff21986fd032d12d14747efb276c3b140da72c50c49f34359c86fe659410a87dc903d4e9c7cf" },
                { "hsb", "78b22393e25911090b37b4583ffacc6277d73c442273d8ec8b979c73639e8a7c8321e1d9a16ee769f1e7eee12fa42ae997b7ac5de264f1baf9e8a889273ad6a1" },
                { "hu", "800fa572d203c790d10b71c46596221b0cb4623e56473f98be46fbe428295f3bf78a9b2e40948eebd9540a7a7bbb808407d2737e3dbfa308cbb0e1f59831eeb1" },
                { "hy-AM", "b73b5fc1dcea2c2ab3443c1869e8bb9b0a01c050c8cd0ece4fb971b4b0f49978e8d3d7a27447b3339e08288fb342001a6f07e9c0726606ef5d91ac286681cad7" },
                { "ia", "78d9b928cff979a95d7150c7c5cfc608c2589a13d4731c15a40cca7b2da523e38d135682227c7f5113f592972727c18db8f05f0358370ebd1bc95eb96c9906b5" },
                { "id", "652e510680d48d8553d281dd5f147146afe68e9100507146283c7d789c13d5d331413d0c001c0053fe9a1b332d4405b722e23d59ef1e9689e23264fe37479ee5" },
                { "is", "9deec24e11a2fbd4bf9b374edcb1334183f69ef371468848a40a081dcb14b577ecea0ff32bdf43f91df7c36e919e254ea1e8a05b1b61069487d3b243a9478c48" },
                { "it", "37d62441479e6e152b0b21406807a2bc62136498e68fd5c837451e89f1bf5804dc6750ba00fb3e44e86ea4d6cb3d4792ae63254076a0306bfaa3523716d8b65f" },
                { "ja", "ada81aebe774115c553509dcddfa14ec0a4ba41c38bfa742b7c1f04e68bc959a424e7112673c28cf5b40209abc5d424d4170b9d64ec0be516bef2d2bd9cc1929" },
                { "ka", "5449a18d0a18129f0f2936c870d1bab61e47121a0a197dabcd690ddf0326380355dc3d91e0483a3a13aca887bf6a1db53d94d262114649a035ae6d24e8884aed" },
                { "kab", "2117697cbc42996c74dcfd75190ebc40c1ea07f57dd2ef053ad23f4b2fd7083513b8a2a1b1454ddb3a461a38b545a8fb10965895af73cf599ee55721c23969b3" },
                { "kk", "285100b1e7848d40b37decddf6e60959ace451b11503fc8c100f104aa2e5e2097c31016b65f03f29f73db7550bedcdb59a8fffc58f984061f7ed1ba722efb6f6" },
                { "km", "bfc05b76721716cde601517f4f6d536add4e1d58102d4ad363b832d4e2074f16e59164618db07703feb914d3a54456e49d3f20c6cab29b11f9eda877b4d6028d" },
                { "kn", "259af52088250a199eac5784863fe98985671c5a50d2c2c634b2ea2ca7f131865de729cc0094a02bc34aad42f53f56f207b4b2952d352c54bcba546afaf3bd34" },
                { "ko", "14a10aabe288723dc670a775c6a45ad32e615c4c174a2133024bb8059e30656ee7896e2a1e35690a6e9a2ae433890cc07d734711597b9cf748475bc1755ebeae" },
                { "lij", "5d66e9771104d1f887592abe0a0a2b47a4881aa8d9496f9107d9f884157b00abe4a44fab659b14ba53cd2cbed9dfbc15c2ddf8dd63e24fab49d35abddbe68e5c" },
                { "lt", "f3d5809a8d4dcae0a98fbd6403a3f555bcccd62576fdc6cca2bcb163df9e714611b9e646b7516814bf82d124044febca4b75fef299aa17d6eb9be2252fcb53a4" },
                { "lv", "06f23910457cd246f4ec3642a56431f52a7b2068be3d2f663c850625862ea34b6c40b8ad8530644e8b11e364ac24734bceea95064b9ca678a72dbe5134626011" },
                { "mk", "6dc103b4ba2b46df23610acd8543615b74909ef7753ca303df90ac92f8c947ade388686c1f89e37e0eea0df7912d082469f8e0ad5cd5370f3df9224263a6d749" },
                { "mr", "6e99764354286460d483b62c7607304b6a8a99c33fe7c64fb02da86aecb0fb23e9ed61747d0c0f21d6b14caacae22913c1da767c3a29432291bb85a19cb0112e" },
                { "ms", "2c8015e94c6a3bf13ba8a3d9cc46bc730c1c27dcb33da642924a39f6208f24d84a34d26d62ff3fa8847ab198b0d82956f5d7cf0d427f532fe367a83bab6e5ab5" },
                { "my", "8c2067d53ee64cf5fef178eb1b7b4f3bba468d40cb4c50bfc5eb5aceee0cbdf901a3873784d6fb000e80831370752dd53c667116100295e46f0f07503c6b3810" },
                { "nb-NO", "d84080976df86d83666d62719e647e6595a2ae9af7dcaa4729016818b5f03b943099cef538077b14fdb96acf51e41c857047a86d1eb123b1c91cdf93c497ba2d" },
                { "ne-NP", "b3536de291102a3e7b61ac998f710703d96327baa29f38db87dfcfe7f6efa5fcbf0de446f2491e3b5e6217e47867cf4a48444c79f8c693f365e77d9610ab4099" },
                { "nl", "e068d944bd9d31f17deaecab846cacf3a621050b30d0d87f5dd20c49adebfa701c8f709b4b9cdfc0d57978be00962e821fc4fde32ad67676926c6c1976588714" },
                { "nn-NO", "c51e0b666479363324ab54cbf45ae515249fdf35e2e462ddda946105dcfbc02e914c6fe853e2b7493c6db9866936b0b605cb1a83be5e24988eec195ed5b82f00" },
                { "oc", "52aacb231f31f27216d532ef0a6cfc8e21462e256b153a006a3a454b9d9c4f7408d22ff6875d257335118c6a35129a509f0be0f608f380d7375e6ae3a643842f" },
                { "pa-IN", "76f77c14ea6ce77230d74c2fd6607427b7599673cf53cd09aea5656dd105f32d4154b4bc839bb55efe1abf8e113080a92d9d0abf79b329a98457c39efdc2413f" },
                { "pl", "00feb27da2e810f89c6721258d0364784c91ff719599db7aca02159ecb29a1550591a1264b7e4e6ca1f465db04aa155afb410b10734a37aa8a084d55d1ddaad0" },
                { "pt-BR", "423f7662343802514fd22a1f2ce9de7b334b4910a9f36daf08180a374af4abd91b92884240bcec17cc52637f2a776203c86732cf0af70a411fb6a87fa57d55ad" },
                { "pt-PT", "c34fe0894387fd9330fcf3b68e06fa58aff71277b3b08f7c81c81aa25492e9b8af6820c91efbfbea6b40cb24420a91de39e23730761e6727eb8e8cdc24be30bc" },
                { "rm", "b91ecbceac25dc43c7e21c1d599d1a28666ed90f2783c3d68b59a5e167b48c726af6dac00dc6a686b89085e712ad3cc77470c4bc3a53e7bf7df5d7aa613a7f78" },
                { "ro", "8228879c8371b78b94b36f509031f1549a86dea95ae35c642e5ad1138a32683105f6abc49f8296dfc7ee15723370a512b535be3d1bf60d264644dfc3413ae8e9" },
                { "ru", "e2ebe88989ad2d182d591796c27ac33519b9215028d97298d412f3ba7fd2af78002b88ddd1b71eb4507b473a8de45da251af0fd020100d8dd4f5f13c36ae6d99" },
                { "sc", "d75e2e00d63a2878fc28c8650deecf8533a6de870847fd3bd7ba5df2dd18e5246d0c5f1a0a4d8cc2adb6d1ecef09ccf9b5db24a54fd3b5a603ff52664fb731e8" },
                { "sco", "34806997fa42475fe7145725b874c35340d5e792ea19d8da866c41873c3db3ee550790c1b81148b6dbbad4dc8607d66d351f9db69e23dfc53b148a1edb5eb09d" },
                { "si", "b922b511249bab419e668bdf36954201f344cde1dfb2153f16c5cef4d2d12288e679916bd153c2d001a800221faaadc6b53e44f55afb3af1e77eb486b560de57" },
                { "sk", "fb4275c4e055712afe91c5a07a09bfde714be36bbf58e971600c98576d5f7086da87f52e01c38fd86d61b84f728e54648fd60138e6093489f443b9fe0e073256" },
                { "sl", "f801dfca7a3e4cc00711977c5a9726dcfdd362f438433d4370f03bb75282e5dce6985ec99e9ab8d282a916104c6f0c6bb685d57d129022bac260542889b68053" },
                { "son", "5799b002ac67bf7942741de79560d1a08a3109c93e4cb8b7ca92e84f1fe1ca6b2398ef43697278c94f8dd40c1e8c69b032e8b74c4b6e68e3edf5018b1d862a6d" },
                { "sq", "36e919f3c068e656ec395527492e3316b4d67fa3aec62acda475b596ba538ac4f23a27f4c670cfcc6cdea648e30fa51684737247b0e4a261e66364525c655d3d" },
                { "sr", "08d48794e6b20f8b44d7d0d6f4e88f0534ee0e3f2c731039c18beb9c83b923eefc37c408544b4969a6d275480457621d3ea23eb3abdd77310bf3ac9d89efa97d" },
                { "sv-SE", "5ea914b01bfa8087dbac8c9f0e38288130592a1046a1c00cbcd73b985acfcf11822d94dba8ede8011f5f73e71f86905d985fcef6963706754e8588b063b74b81" },
                { "szl", "99bb76b7a3af98d2fc5604fefbb976914199c3e9e7aef48835c28a000bb9d5ad3333b561eec0d7a6c98c3489c5591b8fa6dcefdc726c4482581931e14fe49a0d" },
                { "ta", "1a54ce5f2b7e0115fed61b88669d1ba837563af6b72a21be003e983824e65ab87b61d8ba625815a6249e2782c3df2746a2065cb611c650025de8c508fb5040eb" },
                { "te", "11c9430aeac0b8988689d62db574abaff1d1c5d8c61c6be5b3d6ce1f6cf87c405716994dbbeafe6823f5e8a3652eff9ebb33aba62afe95dddfb267d2276ff93c" },
                { "tg", "2e3367a1e557be7ce5ed0b8f829923d8483df015967ac00e307723bf5c368bc309582ba9132283c7d8bd6e90907c5639640b5a45ccf3b1df92553d6803ac6f05" },
                { "th", "f89ef1a5c729eacec56a95da9c07421d35fa5d1652ad0ef95d615ac56b94068160d92d93233d6ca0fb300ddbfec54a92322d557c71204b9bba8f428a3b8b9baa" },
                { "tl", "de1100d5efdad111e8e4aa355b0508ba9c7911e9a322e858003c2149eddd31526a931aa7e8eff9bd2527f7c59f5c6ac73eebbfe9acd08f22def429499388eb99" },
                { "tr", "ddc597349ab52483294f112bd52435b1956d75ecbeb688f1ed159a49c83e308b9c40d1a5abe37ae57261849d22e29130eef2f5180c8639b0088a3a1c17ba872a" },
                { "trs", "cd1622e4b4c546828e27421f6496b3f24fc92635edf6aa46034c61c90baf4f94d83ae38925ce37c57ea8ca6675399b8f49fc78c2f18aee9ca73ebced57f658b1" },
                { "uk", "042bdcc95dad6b81efdbd33cae06d848de52695cc3fd5e0c9e118f999d056bf55f24e37d949c1154b4d50e66f2b6380c85cc04d92c9f3c838e5ac53fbad88969" },
                { "ur", "3e7c625d63b31dc02a80a6d74c4e6aaff648a70a0cbaa2c98c9dc1b88cf25940897c3280e9ffc2d687b61be58630cfcde1a518bd0af4d4deb2759250083e9fbd" },
                { "uz", "4d9ff9f4d681d2ee54aa261ad354d2a87cf612cb197abd29ea173793ee7a9bf85dabf990705ae420c1fea35706e1c6db28a790bf677a8b5730b1d27eef831022" },
                { "vi", "f3f0702f36ba591a63d4e2707e5dbb7d191c2632a9993152ffa0a337b57bca89d1761796354f8f0f819589187df51450f511c6c559fa7e646c3f6c47936a2b0c" },
                { "xh", "4e133635d1edffcf34279e0ccfe2ed0121c99498d15d6c0b6660321d942cc2fac77bb6b53429072d3608d9e12bf9630d090a0663e3fab047b66335a484d4d7f7" },
                { "zh-CN", "30de9beaba59caae681d82c8e1f69b60b9a74d18ce14ece45c466c513b334f19ef2ff273b42bc59d51c386f29718ec605d0659db736ab2435d3fbcb49d739be7" },
                { "zh-TW", "aeb688ad06ae532adfc3c8ee3953bf93b910889166bcb31f6b3d7967bf3046bfe1282649a4bf8273c313ee93e3553206bc89b7c61f9bf90f88b7d98a935d9239" }
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
                sums.Add(matchChecksum.Value[..128]);
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
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
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
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
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
