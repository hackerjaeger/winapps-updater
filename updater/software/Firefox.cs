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
            // https://ftp.mozilla.org/pub/firefox/releases/120.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6e1d8fa4bb011230999f4481e6dca6666bf67e2947d135f3d2a87727a97f7baab7e3525f594ba3a5d5ebf1ebd8190e457a0709cdd1d2bc28771f6eac64af5143" },
                { "af", "fe6c9e412f82a5752dbe44b31d8602512ba24d4c083dd2d661ef2b6126e7f1e3e54cabfbe9efcb6ec12da921836cdc4caf3b981f3d8cc1983bf7a1e5493a77af" },
                { "an", "f300389b4904923bbf0020d98b3a888feb7d78a733d226f2662e5b03552b48656f16021f16f038963de32430d53ec0817f566cf9b4ec54140d6d96b5c8f6c50a" },
                { "ar", "35d567857ef1f33eab38a0fc3f989cdecf3ec652d48ac6fa3def79d2b689ea0b4100bd1c9578a419a3a7e288ffb4580359f4cf4b954c7f05c3fb99b78ca1e5a7" },
                { "ast", "eaac203b4f027282d0dbb3ecb10244f3bbb4e52ae0f204ce802aa9d291b785373185c70621ba4e9e5247335e41cf6eb9a151261645d291cde63dfad5158b557f" },
                { "az", "b9590720f51cc82423cc6603492d4c617926db9df9ede33c711c5db511cebd3980a13424a620cbf1ba27f31369fe69bfee3a790590817f868b74dc27ac482e0b" },
                { "be", "2bca6f1bd6842308f8c5063d6913c92d0a43baa7d276bd75fbe27ec898d2f8f093d31acdf2ecc81bd2a5fdb07da9dbd59063402f253d90f3166d9aaa902ca926" },
                { "bg", "e7fd71a69e4956b900af5831cf166596edd9c1087ea0cd2ce5c9ba6fb0b155a2e31e5376bfb84c87e6cf20846a5813d87aa402740eb03d5371a63214787eaf43" },
                { "bn", "9fe6a94da2b65d9d7b67a7b73b1791331e882189b6bbc4a629b7f559f14c9ccfd7ec8f7e68b3f468e24dc10ca8a49ea7472574c812422504e302134e08f47aff" },
                { "br", "e967cfed08c8114407662a3ad61dbf9b36ad179dde1b4faaa024ba99b7b2137ccfbd96b7adb869a6ae85dbc23a48160495f072461dd82d7f7e9ed0669a0e087b" },
                { "bs", "ab1788703af76c79e272145a0065a2cf6d7c0f3cbf6d1bd5a316e1c0de8551a69cdfc9e6d289c60b6111b84d7afa7f9b96c28a69eb1d1c350697b6493d0a2ff8" },
                { "ca", "a12deb62361ccc4b8b85dad59abf23f972e3ffe03b477ca306443245a3e2c16f3789a0e91bca78060dc41dc81e7e500395b0007dc48ca85568dc2bcdec8c7da5" },
                { "cak", "a7c92cbb9dad312c18d8739a0004f73caefab9476c1f9105dc02b6c084db8311c5696ecbeff5864c6a85da81547ede0a9d2fab663142cea0480c1ca91b59029b" },
                { "cs", "9e679e2be2ebe944c7ce3970140740db475449bc81f35ef5fcc5c171fe9537160c1823c8b336aebd2c4360dd45568d305fff33a161dfea4a1cbc34cd8bd7ddcc" },
                { "cy", "4138537581577c0f9b3703bd1aa5d75f31ea412b8e4de837035b08e671764d6b2729da43d3b8cdb177cf4eaee6de664df39155e02b5755f108964a7d1c1393be" },
                { "da", "4ea966b9dcc03dae3ed11185c0f3b62013d5458846674515e73197266a62491c19f2e9000665d2d12e3f139acf27173e6a99d77203dabe45360f0fd33d05cd66" },
                { "de", "321d3414bc5c2fdd93c917d643bd138a1d8c4b6ebbb6cc30fcd2a9fb9985cff925880970283488399dd7ddc4393d05088b68614da738ee813c123d41252a57c7" },
                { "dsb", "7b7ac915c4baed9fc5dbd15ffe12b9820ba17dfddb52884497d60c0a76868ad2b32b5ea026483af43f61e901d2e6b0193e69d9c4e02674db85125037670a1d90" },
                { "el", "4f6b4b584eec888de254bb104f2f8792d766c00287b7162f1f70ccdcbf0d0931257fa03f4af56ed73528f3e887c9550c3bf4a5460cc61abef97c909ba321ab50" },
                { "en-CA", "9318b5e0774ad89293f9d89bd3e2a195f80ef02f0be6988b0c0c28c464a24917627d268810fb4e9a9b3d5086ad6133f30ecdd9facc9f91cf0284c6deda75cd64" },
                { "en-GB", "bab68e8fda08b5fd20e28f2efc4027ae1267554e05afdce6901ff7487fb3228cbc4e308c4f6986922e4fcafd7cf52b2ed4720dd89cebd697674a8c59dccaab26" },
                { "en-US", "0550d04f220fbb72263d1afad033feb14a32d5380b497f3d6e09af5a7e4a110372d69a4ec08c0669310031a29b2eee985eba59d55beb68f96bb3c8d85f93d24e" },
                { "eo", "52c73a2b93c4bfb945db31ea8d92a6e1f5dcf8c9c46facc59bcbfe42f1eeafd18326b0fe08424008339871a3165eaf13daec22f1f4273dc3c8dd75fdb2cf67b7" },
                { "es-AR", "e37a6b49ae729c76669e42164bb0fc0c3989801dbdd3c412ff4e53be220633d8ac1d3aedecab053cc34650f0e4ee7ea14aad04be0f323bf755bd1132c0f58c3e" },
                { "es-CL", "2bfddaab1af585223a744495c9793cc8882aa02c4390ebb5565c55746f95ef387a94dfc65a2598f3e5d5edbdb46fba5e56186e1616cebe2374bc2936839e8c9e" },
                { "es-ES", "87ce7bcae078c9c3bc1ca4ee4db004e5e5adb22a03dfde4fb57b8e5f3957fc664e696a8a42830468d99e44952b525fc8650ad5a0a4ee9173f25bc2ac2d7a86f8" },
                { "es-MX", "e389579d54997005594206d8221d61c9a3e77d2fa82e75371df251d3d42185945e0fcf5207ffbaea63cd9f7f444e110dedef83452e2b192fad9db4b6c918d79b" },
                { "et", "d61dd06414939982a32ab9156c0fc18b6a95baa79e1509273a32976ebb886466de6e0c5c6317b45f2d2f261b643e148db48602057051aba1591deb2960a56fe6" },
                { "eu", "a9e8cc41d1125ff9b3bd3298dc8f5d91f7d0e28fe02224405fd06475b5de7d80ec36a8bb8ba6a9a5be642327d3471f4c01497dccfa5555caa69f6e77f295b591" },
                { "fa", "08f6f4d87141dbe7e944a7688230cb10afc44e4dc19f477552604b67f2af5034185a64701930b49759e6a6b9760260dae03110b7aad70c6275fd07d934e6df91" },
                { "ff", "6a7ca48bbad1991db4145547b5bf1aeaa949ed49ee072bd12a27d8aa3466757d4b9a36682ebdfd71da0adeed2da6c6f82e52d2a1861ccb5772a308eb52cdccf0" },
                { "fi", "36ad351601b5b8f71fc78e7b43a523aadbf2a9b9129fa77f074379cccf09aaabc5c0e5b93f6caf9de420c2ae15cae76fcee07570d0c59bc0eac3e000679025d8" },
                { "fr", "b6ca370e27d608d9dc9b0e130431afc442d784b36daf4c3b74fa2e034d770451eb7061398ebbd99cc3c76e92a141c56626d664fdf2ee0ab96c9e2cf0c9b8ee1d" },
                { "fur", "bf4226183cef5ff1f56e8137ee391e875d1f93f92528707b3537738d0a8b6ef3873d8e867a9808d228a5ec0c41c12bec683405cee0fb3ea8c210b6c98e5a670a" },
                { "fy-NL", "0f323168bb132314dd3de326e88bcefdb58b2cc9e2079f314c0a7380ccb4309f2e8940b925f9f35d7034d0af166e596069a9c8979bb72814219c546fdd3d8d71" },
                { "ga-IE", "db5b4889a77657d6da8b8528567ca3cfec9581842afc352f9fbfd6a1174236f562bf892f3d5f689b1567df9b740144f09da1e459cfad5dc1bf00ce47cb59be7a" },
                { "gd", "0fce628bac4f300cdbd513b05ce7a25bf740e0b48fab5fc5c552fcf0eda43c7098d3109a2ab60c08e6ded03294b41726e861b41357a58ef584f9366e375bf1eb" },
                { "gl", "fb5da1453fe3fbc001152c8f06681dafddb1e42df94741625ed4449ce7871b6f8c5a2edfe7194a201132387eedf7ec1718c4ac1f0dcd0c6b35f706c157ff45cc" },
                { "gn", "3757ecc199b329477cf61eb1169e4e260ba2574a32cdf9a6f99b21edb3f327cfc32344cb091c0b047a8dd9bc81088d7d5e7b36a929805759fb5aacece391bb18" },
                { "gu-IN", "8d29d0143dee540dc6c45ea1e090e6ec5dc5d16cfa4d1ac16842b7695a503a4a2a0ff926c260082c4f0aced57ab76bb5d471515482c99107cac2f30247409fda" },
                { "he", "8a51edcfec6c707be123ba0d8165e272ed704d51abb927c171ffd6ed76a77e172b97b03b89b3045a598498e61a0be0c795a3bb4c652c073142c2589bfecd0b13" },
                { "hi-IN", "84863cd2e7458e5cfec11aee2fcba68aefbb1e17bcfb90ba438de46f11379d0491d9d9caeb1462b8cd6e3a37954f1cad3d733d8f7d020440eccae39b16b3247a" },
                { "hr", "d71e3cebb0fc78da65a95b921e50f442cc9fe4494152483cb7536fa634f99dc785a572609aca7882b30a3966f233e32357816e349b7da749bb9e801edc9bcd5d" },
                { "hsb", "f9d3717d6aba33a5d4ebc8fc521c23124e48a44ddbf7091280b37bf52a1f01dbad4612ce78eff232f5bd2a16c2b888f6a44cc91df26fe20273c233ef9731a7d9" },
                { "hu", "6a8662d43c37c6e20be4dce96788179015204bc19aa333db52cafc8ced7828ef3880afe203a638270255a5368a26586150b0f41853825e84e4861567e47631ac" },
                { "hy-AM", "bb0123b4bed72e9893511d8b163baf51e2bbbde36f04831af0be63890c49887a96ab8e32f579412752cf6b99c1c371dbe0f92d5a6d2d382962fbf2f30dd596a3" },
                { "ia", "21fe0569d8016726ba6968fed40f66fd5c5f0d69c8f47395d0d078cd75adb3f91b8ffbb1936f35b03db726823d0dcdce40d61aa1ec15b3140a292d55b32a5990" },
                { "id", "15ff6290bbd65adf151df196526448ad40a818acc1db52048fbb4f48931d89cca97ddd65b5a31fb6dc5a15c5d324c46a87590e6ea894cfb795d5209e8760010e" },
                { "is", "d59bd5104eb701dadd206071679ff1e60df83d2e363e2ce21ddc7961997f1434de69e1ffff71a0f900c495ed8acfd31036cf9d2ef6e7d96dcbcdabb20031f57c" },
                { "it", "cfd05b0d9ca91467987378a1a3016272bd206fe80d1ccffdac7821ebb50e7e80623436d872158b85cf2dd8a4ff51ed6d7f9b3dbbfa1e11035e7bb9b3063b44cc" },
                { "ja", "52fbe69a143309e2a3232970ec4b5a3db03f8fffde6a102259ee4d141130b691f141dfa013bca9368fa75993910ca5e53557d231bd974e05f0383c5bc151822b" },
                { "ka", "40a937bb9637847d6e76b7a8f3dcdf78b962ecd1183631e1987a43de126e0f7aaca4022c456b614c1e9b2fedf536daf06eaf7a173c896aefd3b875f138b92228" },
                { "kab", "ca5fdfc94e213bb3bde4bff319b303c78c7987d0c5c5df8fb8cc84a10b5ebb5a678b078ce2c87eabea5c8a5879b3c21f8c2c01a7bc6d13625210105d65e2dde5" },
                { "kk", "081523f263c3f98ac5607c1fa043133ddf8d4da5f0d635151425089f1aabaf3996d8db39e3fda369ee327845ae62c2ec805aa1cff9eeb1d20684c2c5fda22588" },
                { "km", "87023d8ef1fa4059e90780d9c146b32c5e019d4237355c211a85558c724c19ade08bfd11f9c75a1df973ed139b8821cb00c9744f85b6b3db4e5f4b15a954961e" },
                { "kn", "1ec3ca25a42616bafa63090983fe923c5383f6e6f3b0a456168f055de7255a144cbaef27ed10702667f5f6fc795a0abf346f339aef391b0a97be9daed415e62c" },
                { "ko", "67819a993f56f92217cf91550ecba9200564562c9c4fd13046ad814378d4499ff7426f173a224dac06d2204b39ce2000851e80f08b5aa5deb020dfafc86027f0" },
                { "lij", "1be0fc745f8097c21c0e1a1a89c259239dd070422a71c0db8950ea9a57567901e119f2746841090bae10459c61f9b46dc1fad66dfb464b7c6381a9837fc25b0d" },
                { "lt", "1d4c5b5c362021780a5bda255ed421465133e5363e65a7047448410c50ca11f97bc885d4f6d0068695e0b5ff914b4ee64c27f1ce1779db7428ff02be8b38ede3" },
                { "lv", "7fc8c3a5c30cf1a6fdbf04e6094a24001f0633da34806ba81a08b502f5ca0c4a3337a21e33b9b2a7d2d8dea66bb38c3a43e23f4fc016ee29bb347a8af9e0abaf" },
                { "mk", "3e886dcec684e24b36a071a3fd708dad905446fcaf6401c845be8359b4b832579d0d8a71672c748a479bc1edb21d8fa2dd8cf9bb1f5f48829d91a76fa9c6a26d" },
                { "mr", "82ecc1f6052cd7ccdbe83b2a94aa1b2f8cc7ccb18ee45fe386c8c696488ed05f36c1668b6b63a2d4887d842a2d307314fcfa95c748b8b9e287e62a734102801c" },
                { "ms", "0cde0c61ae557dd91f300035130d70b94c044dc69bcbb5911abd04c2e4d518d67431cbeadbb0451d914ba16e619add4c587633fc73c9fb0c11e95cdf54e58983" },
                { "my", "0f6e8bde7885a24c63b18d24836340ab5fe0ba627829edb628b2926b14f5fdaf01eb465e3bd5a7863f945ec689959ad45089fd2bf12b1a98e9b60940d8dc18ec" },
                { "nb-NO", "e815328c7b70cc35cc87ce3ccacf81e1f829b1d52988ad9feaba4c3ead42f3f0095decd566d90ffbc4557bb1d96e8e98b3ada4777f55b9b23522bf95afed88c0" },
                { "ne-NP", "b023efffc13cf5e765b3b0c5348cee427f1d8e5b4b9706d4bb9bd42443f1cc55e865b5e5470f9c72956babd9b4b9d216a9f29bf24cd04b89a1076592fca54f4a" },
                { "nl", "3335b9f77b680f3eb53f076e1f7977b242cdbf630279bc8d6e4d8ef48b6d1a9a7db91737d5660e8ba2e86ea5b53c90f80df8b4d5202fe077bf98e31f5f20dfe4" },
                { "nn-NO", "128ff9b5a8709f87d20051f51a093d4d772d760cbc7a9fd453f18c654a45b5dca116dd64b38759baca697d9a70e02f6c43b94e8712ede995aaf2fea3743a81cf" },
                { "oc", "172bfee5e84c69b3cac743286f3688666fda6159834fa470fd2e62e15dbe3db06a36ef5732aaa63612e2e433f9f87932c1747cb15deb3f3e445432cb97b4f9b4" },
                { "pa-IN", "58e1ef3368255ce13d0b9178de4ea7e75bcd47112fb92118d1959c4c82af1e2197c736f657c51bf92f710c47b967d6c59c41837e4c21425f1eb6ddc18db23bee" },
                { "pl", "361c01eb6ef3838a716d659b3a3de0c86c2d8544b3ad4f6a6fd7b388b0ddb6bf435284e93c3aaccd0e5256202cc8429fd62e9861d64623bbf9da06058da913e4" },
                { "pt-BR", "98bc053d040999360a511eac6163d1440d20e784723be4a5e0eb9e8441c974d732621bc4fb4f9cdcb62656fde1fbea94e4dc0f2f2065235d0cb4a2df9bba66ad" },
                { "pt-PT", "6ede3dc5e9ed47e070c16be60799f67fd8ba8a655aa76fe2ef8478cc1280314d6eec2ff14627d964188c5ea900eee55cc8f8aaebfa1f43b71a41519ea60265fb" },
                { "rm", "c744caf557110d56d05525ee4d6345db0899cbdcecb03fec5d3bfd9e7568b4bc45f6aec2fdc3d053cc2aae71cdec950e096d537dba4e766723bf930d20200935" },
                { "ro", "d5ef5245e4744fa43dd387fac79a5c73cbf71909b711521c927785784488cf03f72b32aeb8f3ddc425d26437ee0eea6b2ea1a43abb381bc347c314da7393c720" },
                { "ru", "0153400f78d5c4b6b260adede29908a9b2296ca1876148fc9644abf40d80a46ecae719c19a5e5507d5719c04ec86c0dc97fca3dd84a23a244d652a6f0888181e" },
                { "sat", "4cf53098e305803b016eb4faed0cb30e224c8c93389570f5f1a007a83fb6887a8766cb630c3a735f7b6abfb137c4c3ac5fd7bc2cbb0b21c0101cfe4188ac0181" },
                { "sc", "fc4f217c4ab5e6be8bd607746f9165e75209cdc0524b53333446d907698136d1c187388f135fc8755bc75aff27a5511dd87dbf60fd016cf6e5c83688a7e33ca6" },
                { "sco", "32fb6cae7b4b811b7d008d50a851fa10e95683460b0c8c72f293a08966d96c2e7c606d05cea0c2b28e827b71df21d07ca931cbf4b485e70d46a0420fe0e2a3c1" },
                { "si", "59c7eba7b44007a8e3bae9868ce1321fbebd21630feaf8c2ae47c14182f7700efe289e9238a32c0f091ec9d874356ae1f2577057ee1a688b788cd818fb6747f8" },
                { "sk", "02fe8840d7765e1082f7a4a01838e7f2be44d505c917b57835f08c62ecbdeb27080a3ac8ed5463211ba05df61d016c7907c1ac1d3adc436f460a42b6bea81b3e" },
                { "sl", "1d0678964186524f127bfe2463d6b730702d43a2ba5e6fb490f030b60e1545815664839b8735a7aeacd2724a013e70ab9e01f13ba5668782e24e98b54ca525d4" },
                { "son", "06f9e84500131588376f80b3ace9646003196c9c0fd946aba2fb71be8a2e7a65257a818f2f3b0fc62d8fe5c36e41f7681973918879e512d8235f66c07f4b594e" },
                { "sq", "2490f44c65548ce57c51bc265c6703ab97e067d53afb9279f602d5eee8d63a53de8dcfd0e3bbe8eb699bf9431263633216c5071f7af2212fc2f755f248df584b" },
                { "sr", "4fcd1310f330a2a1207701dff5d9b411fd6c964f612be92069a90738bba4140be5679ff6823279e0e3616fba0e81317d436491d8164fa606f4446737be431ea7" },
                { "sv-SE", "9e6d9c36130bff13457740db623f311a854409417199ebf4f3f82fadd0679adcc7fd572f9b02961a8fea45c76158a2128b54185f594cd48d264f164bb5f9a569" },
                { "szl", "eeeb726d8fac5a4f5337962de6fc0c8522acaf343d17de75ba34e5a909ecbf4d21665714ad93cb60fcdf411a9dcb715dd04352009b2f8c3cd23ab5c40a344234" },
                { "ta", "f42e98142f91b54507239ec1d79f24144ca8041511978d07c326aeb8e86908c56aa17b49e66f03ead345f6df65814f82b0e4b075a1f4deccef7ba87582e716c8" },
                { "te", "85ba17ddbf06997c39ef3739edaaf12b337b827b23234b010d878a4e0728dc30769e476ed4afa3a9eb39d5a470f0469289e63f60f617e937686be37aace30129" },
                { "tg", "641a6afaaf76eec0583789581c52c5bd1f8bbea91ea1bda1834c7f31f8e437b95a3872067417d45ca61c5637f575c4fccc0ad67a9009401d41591d5ed8e0cfc4" },
                { "th", "3f6b26ea180ee702ec6b5695ed0daa497f3473b72c0216521439d7f867bb3b26596193c5e607a618ad3c8f7d9e28ba2220825ece44c3bfc812b92ce3cc6ebc09" },
                { "tl", "e87f3222e1a520b7eec34f313a23a1ce59a3e52c43925020b395b44054856f15ea96db53e5fa72f611b3f26e8a122ff2d0c1293237cabd5a107a5897b3609e12" },
                { "tr", "d0c88e75b42d2287bb2dd65104ede4c51831cb403b9c6751f9c42417bed865e4fd1925887804458ad86dc069eb7eaf89ef073b7acb316b26a0406c13f07902cf" },
                { "trs", "3c5f9b5aabfa0fdaa221a2e0f0eb4e218af3a5bdf46a3a16fa2035177ae0c0c37d51a780aaf96838f146b29be62626b792da4f7df90340c1c0b59e81a881f19e" },
                { "uk", "b104f565a5aa9252222dcb868813b9e277a17c7dfc031293172ba14ba71ffc996c9220bc64a14a3524bc7eaaeaa2805a1dda4d519742af462fe7c0dab1edd220" },
                { "ur", "2c5376c87efd89675397dc76b1a721929114e7800891d8794155cdc968a23d3887b9be03fbc571c75cd3cd9edd8a8549e9b26401c2ee24e66dbf694a311eaa5b" },
                { "uz", "9cc0577d6b6e55b1a82729466c728934823491dcaf6f1f15d874dc613cd21e58c4faa39c840be2c803b1db7f4c19da2dd027f7f66608599f3b4a27f1a2e44091" },
                { "vi", "84eb3d1aab6a1e75514480895548e403b33514d6875591523d2a0253a2363c71342fd56490cbe0eb0a9c71999e776cb65de8d4dcd7f3c40a8d6c97ce2557b60e" },
                { "xh", "8a962d6fdff8fc3875813f7101ef77c60887d5c9e694bdcd57ed1a6da77cb5ee8f61101320d547b2c078ce843e712b6a24a856772aad929d021368774cea10b0" },
                { "zh-CN", "d3c85762100c63bd184e6ef446c5e3d8fc1edfe961ce81f8bc40d90f2c22ed7b9911927d873bfa49b136619e0cac0d06fc87ce3e019c67d47b18130559ff0203" },
                { "zh-TW", "8cc1de69f9089d8101d41691851752dc1bd56ad52e0589100aa9a0a93793e04f1de67b40e7394d788903cbdf0070db3cae51a2619222150d462fd33f133685b5" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/120.0/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "20b780ce62214666506667f92ef971c23286a437105134bde72c89f381ee9d3c27917bb47214a4b4566a37279ca64ce9a3ceefc1741a31a4805205d6b0de5080" },
                { "af", "981f2f23c163dbffd7ea1e9497826c7b133f16f0e410a524cfd46fbe3888bc8465ed2f857e6ae32f8a3f696318eee01c9629706cd48b73ce6d905b0834eca1b2" },
                { "an", "2b482305ffb7c1112eb92893cb373f20c0b9aec5eeb081256f0634ce19fa10542bc10d682392946641edd1b9c38fd5c21d4a016d53a1180a7d40da576560376f" },
                { "ar", "f552171a29c67645c92b517f2db0753d392b3698a101f13cb26eb6b0e674c0c3ba6640ad5e2eaf6d4ed7c33426548f8646b819f3669293e7404138ad123faa6a" },
                { "ast", "29980cd19ac1676b5cc90b596b950d0453b5f22467336e0dc416771ce65b03cb644b13e49ef5b2482acae6266a701a53bd2110097e2122baf500b2942ed22684" },
                { "az", "ec6e9122c8a151b647f71c8bdf5ddea6a58c08de988605668dca8d0499b82e046dcc915d95d8e9406c33d68a0439ff8415ee1ff3cd1b36269af0d03219a1cba7" },
                { "be", "5505063c39813736983cb92f34adb51915cc71b6f9f74ccb6e9642b2a26a52ac16bf87bcae087187a9bf7da4916815e343e5e6cc5e6d93bf88631cd2f06d5cf2" },
                { "bg", "4060e6174b248dec279beef42cce55edd912779fa30a818c2c79565d324abdd28ba006494dcedc3c2c1b9a929bb14a009c0b19d5f634c8eea91321b452b67965" },
                { "bn", "305c7aa9c45eedbee652b764df3e1f90fc7e81bfde956c585a26677bffff24979b4106ba5b9be261bbc92129de2c1a792d041294dc75176091b581c738dddfb4" },
                { "br", "9d7f13b729a9e1f22819f0a20d8bbff22e63743e853efcbc58ee3622feb5e717ee6703f7bf3de3daf44e157559ac9c350a952fa14ab8b80c249dd90fed38e8f5" },
                { "bs", "3961a3b4af55aeb85eb0fc299e01072e3fe83232f76221552ea1257d96532eed34cb3d5069440307b5efd4b24e3b08ff7606c7e4b7cc456b705c1c7d5a8aafa8" },
                { "ca", "ec971f48c329401b315fa92d5e2b678ef17530fd88f5dee75a90cd1eaaea117b4d06d1a092981f638db35d79d9f9a178e1292cc8eb7ea66673588e10b9afba7a" },
                { "cak", "52125872b59d134e5d18d7b470b84c0aa9e78f177af3f320119f68f28fb6285fcb74f20c1d17acc8558da3446dc546c2980bbb86d700a9ea38b264b016514ced" },
                { "cs", "42ebc7bc547de33d82c7ab662a133422d3f67243dde0d41146abdb295ceabce55a83eb9ab2b3c8a1d2c50dfb32feb5302dad88b749054fe908f15f3342242922" },
                { "cy", "7acf2ebd9f3f4a3aaf1ac1bf1dec8108a1e6559056b297bb973e1f4e9b29c7dd3c0e97d15247cc9a530c89aa35693bfc9c63887a0264e3daeaec42547776e58f" },
                { "da", "5785538ad224276d628942f31498d96ed39ac45a02707201a9eeb236bbe99581c4c5ade45517d2bd446f9209ba1597d76612cdf1373171fccc94fdbd5ab96dbe" },
                { "de", "fd0dbc2525a3a5deb79011eb0789b7c5c67c16c8f0b5f7555c23eac5b5048fd259d30de7a2ca70ded34ba5e70fd16a07ca4467ad586b173390fd9ec238a33201" },
                { "dsb", "844a9b36bb891cc719117d0a8ca06a8c52c85b70c4807b0353d0da3ef5ef5bca32d760e73990bef623dc38e290a0e11948482d016b0677285721d1f997908f04" },
                { "el", "342be10f562b162a08653779b47319306c5df0eb86da954a081ad617d8e9b1327ab859769cd63ed79ab87e09c039ebd3e5f521f0b40accbfc41f17be9e0a1377" },
                { "en-CA", "e125b913de37c469476f7ddced85716eb05d84d9cbff7b6a3c88b138be6d5f5b287d487316334133e10bf135f80b1a3e44e859def9e39047566587d131a71efd" },
                { "en-GB", "8927a7084a66a7b3abc88c0e2e432ad995e54743962fddf68a27db85a7727436d4c64366c4cfbe703bac45a13b5ee5877a80885e7c0683574450ac6d65c5ccb0" },
                { "en-US", "8bb4a93de64068bcfb610b73a84c689d715fab5977cf7a20235bf35fb656503a93a913d3c339390fb1bab6ba994a6ae7616d67e558c98dbe7bca503567fbcead" },
                { "eo", "de8dae3493fb29bbda24bf2c766dbf73b6e50f5b0dfd6dd05a4707c91222e4c1b076d7bf24f08e5712092c996e144e77ca7196bdd16ae86e38b7c6b64f9af825" },
                { "es-AR", "1341b54b57b781271e5ead99db1d6bba245fb1196b8b46e92d317d4c7c62c8bdcb9d3dc3b45cad9e44d6c105430eea3d450f28929f65fd8c828a190a822146fc" },
                { "es-CL", "6ee0d36e1f6373ad1c1f0c02ce1a735723dba2d81266f7ba31b354908d830ffbe9aa3a05c039aa065369fe1c60f5017d6ed8b0fc5d52759f7a2b2f447f5a2c11" },
                { "es-ES", "acf23ce37bfb31f98ae2948109d29af4d38844d3d98f8685cc7a10179b3b5f3b5b5e4ed226aebb450c91d30b8aaeac79960a89ab2ef343103ca0204c41a7caee" },
                { "es-MX", "b90806c9af8d3e667c7cec0e3bfe2a7ca4f1f58ccfe1d67ea03050be3f383a1c4ae33b7c6fe72764d4e642ea093a9d0dca9428d8555c6b6c8786035d8070ea47" },
                { "et", "5fbd254ca48178afe4622e3ad23a24974bf5283725479abf2a768d5f60fb50504747f31b9d2ff87a30bb4346876591f29a63d689324ea59f7d7b47155eeda851" },
                { "eu", "1af373581d71cb43dd08c1fe7814903ebaa89b3c38db18aa2acda5e3ded79d3ac4d96edd8660b2136638380012346eeb9b06e9007175014f4887d6d62b4c7900" },
                { "fa", "b9e38de1efaf47bd2987717f5ece283150eea419b03b18780c70d73de4b2ba7c34d2d25e0a9969e9f1c1594af2a89ccc668008e41f2ef3d0475646a317957e87" },
                { "ff", "4b234f429c8d2e68d3736536d5a4d7207ac94fef6f6b4bb7a07c5bee1fe7cde3c139326d3041f214ac23d93e825f8840b106d6534b7c55102034b52ec1ae7e5c" },
                { "fi", "8c3a614b1d0a42cc0af49c1563d75c726b46fe2cd53b5f2359a9dd10a37c838c8f7bb16c937aaf1110e5b6518d3b10468f9529de0dd7b9dd2ec6d94237c06b57" },
                { "fr", "c310ceceea6d5d9eb540ddd499dcbc91bcb41e147114fe071a11dca0649fa393bdb702bcbe7f77d84771607877ec43977be099a20c3510b6378b7f6af0c04671" },
                { "fur", "3b51469363a0892a320b013c34d2a57960c043dc7d2f8c394708587f6c5cb3a513796eaa447d3c93bccbc9b9f4ea52899be79d40ab55d5f31cfc29708e90a932" },
                { "fy-NL", "f364184bff820f7d8666be352a17e14bdb3c5416ee0d578ced0e09f98176458c34df8a85516406d8ffbae205d5b09adaa0507bb99eb1630775064cad042939b8" },
                { "ga-IE", "747128d4c79fc2e7e922385e220c83c1e71368837d2c4a8b745480b63615237832e5fc26fdb60aeec66bbdc1a69567b6b677df3eab1279e0344a4b0e7e89293c" },
                { "gd", "45c25e33c54b5c3210ac39908588d93e6501f5f9a94988e25128c2380e55ec6e7980de20d4790d3917f75c729b64495172e148bfe0598e9d187826dbed87237d" },
                { "gl", "ddf6055b023cd3b918679440681916c8dd846ec2d1c967149836b65f6271b730e34860bc9867d277d4cc51e51dd139579bf1ff3d098d07017de070aa98653481" },
                { "gn", "ab2014abe88a2c944d749a5230d649aa1d6ff7a7a026da1e04c699d0802aaca1bc736a624a4c86dc51d68094de5c2c5949cfed0a5a51d5a33271518697dcfe94" },
                { "gu-IN", "5b6a34448e4554a0d5cda56b44a0d6bc8be4e9b732653ceab18d90ad4e9f689dcb10afb149dc5e473ff6e98c49223e920ef9ddb95b1a75a2e2e71a310327f1c3" },
                { "he", "ab6fefc46a7261510230d0052c281be2d857f7ef38ec448847b8d29b4443296b02ffd9fdabfafd5d1dffa301e90ffde458ee3b14a72c04d5627fbd093ed871d9" },
                { "hi-IN", "5a86f7180d60fd56ad1fe9afbac2895bc86dfba338f86349d1d25ab80977d04e2b33ffb09c6eb43fa7a9474fb71af4f8f407a93a15fcb739d08b51e8ebf97629" },
                { "hr", "5545cece51ef78e4e3aed04648c4805c15746d1bc6bb086879a572d9e95826c98e2f11c6ebdbe9e69021d6e02d7c0f58e7957c9643a91612c2ab2c321870aa23" },
                { "hsb", "ec4889379765dd8300e8d8cb4d12c94d67f14e94afb8ee7dadfd81f2ebef99bf927cafb70e815491baf1795f08ed7e1d67c0781bb058aa4b7f2176c3909b7f06" },
                { "hu", "18a606fd520dec008ace5a296cb97d3035d3d58773352ce33f9098c1b62e6420490e1623654cc649a9f4a40b182c48beeecfeb4a7d279d402a4f904dde6277cb" },
                { "hy-AM", "d4e80fb2e796f384cfc24ebd7a56bbd839d1988daaf76eaf0d9af38c2eb658e620e2157565bb310b78c1f42f0ef4cc37a76684cdc1a7c6b240c6cd4c139eb3ea" },
                { "ia", "aadf979378b51855c086dceab81f5ba02f1985467b69a30330a27e4cc643b7118ccb560cb91418f5298ecc3d2dbae298bba5604164064b49db41e0ffe808c1dd" },
                { "id", "a0b9617c45d74675766e9a80500b1255c3db67003e3d31a952266e5e30101b17696c2a82e7f514e3d0e1ad8c90a6f9a07861a3891120c600b11b265413b58073" },
                { "is", "340b101a11f99e4ff228bae355b7e272c98d51ecc29c56ad4d7b6fd966091e94a99d04d4f35b24451c777c7295ba5df961ed6e23a373830676629ea090402874" },
                { "it", "6ba175d2d44e4a2a98d9f530ea9643062aa962ec3f89426738f0543210884690103ced36071d525ad60710723be1f3adf80801ecaa18e3ba34da057da18d8b3f" },
                { "ja", "eae81fb36fa3f789966bc77d06491e304ff23b23d2f8059aaae75fee8646ead6713ed2194b4f17d2097fb4819a0620107928ceedbfa524a95480d4becbd34b9e" },
                { "ka", "b159add6a77878596035b50d2a4706a47cecc950bd088af294447ffc073537a0ffbc1a12f07b598e52a44bbff59524157faf0717801bd18111a140d6ce25019d" },
                { "kab", "64769749d6d9990f619706dae8767fd0eb988ecf3a3edb41ab6b4b74534bf9a011bd1f6bb43454686165b4326a47cb3ea8513f457435aba02ec1f248973b9cf0" },
                { "kk", "b113bf9bc33bfeb1ec50e50be2c7f03b42f068f953d374ba9c0f4b673c7a68228ffaf860bd917965bd279b55fca919ee36dd003e47680a8bfbeb9c306d284ec5" },
                { "km", "6b419b5963ff44fdaa0158e2c2f001218f8304a5880e09bf23296ce9d7b7f22c6acb02f39f221fd1d67d6b7e017619f5dfba3fa565e224333f932e15bad7dc2c" },
                { "kn", "275825bc29fbf323e36a66ff5e62d678736d726030b749ef9b6a598367ae779cfd323a1b8b425180ac39670dab7de0fb3d39ee54b8ec9419dc5bae9ebb77f4d2" },
                { "ko", "231a70f5a3132eaab604b253a3f8008626e9df164de49d9f5ef3eb4ef3060aa7936b3d6247f1b30c0a723844a182e89827df92d7ca8d3788acbab05d92f991d3" },
                { "lij", "3252f788f5e1e7262bb4d291ecc0b5e2d77dd7c5a3a5b433dd52eb398fcaca1fe7e166ddf94f4a1b6604ff8d0504afcfe4d66432d33c7c04755f6d6dee37d9eb" },
                { "lt", "7b01393f0b2f96076e6b77516e59efb5c6d3e42be5ea0c816bbc4f8249b83e4fe594e43422cd503d62d45d7ab311ed0b83dd5c941a2a6ab8cee8b211da661fed" },
                { "lv", "8bab690aaadfb5cb043c049218736e24dd4d0ac1786f08308ac7e99acfa9376409981ebcbc5f14519eecb5575d79780302ea6876495af7b32170227493e13eba" },
                { "mk", "a6fad47c1c58c911bd38b35d787ab2c1f73b473f6a70a050ebf090740b4ba7e7a09b2df34e306755c0301cac221f0c7fb7e42fa6fbe9800f52cc10a81e8fb974" },
                { "mr", "d812c36886a66c3faddc2ceddb6b450bd565b40bc4caf6cb5aa4a890b10febf05dc2c911aec05644625f2ef76d3a2d13485a005dc6a2b1606bf0d782d5e529c9" },
                { "ms", "2f258d0b357fa861fb87ef802ca71badb32194a5a69c7e669e2e99c160dd09e4e863cf878a9d4b3c122d0f645869f67b8b267eb66736f18dce86046d271c2b8a" },
                { "my", "0f6dce78cbf526d46864f15f0d64358a8c771b52695f9a1a2d08db6ec40393242aad6dd4781913f556ffbcf83c212ac5c70971d1148b5d274cc697230257a0b3" },
                { "nb-NO", "d8475b84f3a2f8499fde3fae4f02878e469c4e05d5eb24bc9ec421867af338a733d38081d25245d31c12aa232bdeea24d5ccc1431689f2fce9bb2845f080d3e8" },
                { "ne-NP", "fba1a2fb797f09d634ae12c9eb0e0aeb4f5a87f1d61923c9a080161c0fc01c4b1781a64d5a7f929cb83f04f84e073c0514cb23ecd315e93266c0ec103d11f00c" },
                { "nl", "217e8d1d9a98448bf5292aa140082fa5995d20787039b4d0ab6e5cd1b00384360070743cae463999af842f68fe7eb7f628ed3c7ba727a32d64b8e7bab5c5afe6" },
                { "nn-NO", "93bef6375d94290b01590648ec9beb3c73ed5846e2d9e113e95b98e46cafef92668b83896726e53249d627fefbb50c5df3f422aa59e7f0bf6f09a1f82d096a91" },
                { "oc", "2eee030062ef30ee61402c110ae7ce2cbe3f6e433c77f3a6d8ac12dcd19cc54d716f6f23a8c906bb2dad116b5bb5a91e51c587befdee0b28429b94dc6318ed5e" },
                { "pa-IN", "36ef6f005f6756284c274d5efc6785020602ef8f0ac923ec2626a27ed329613a6ba549f5bccbd2651195dc387b66b6cda8853b5b2e0c52984427d054665fc975" },
                { "pl", "b0fab8f7b5295b693996d053b4796d95abc59bf7afe49203e6027c5d00c9f7d01e4c4c7324cb8f02d13ee9488fbbe2f388d3c3f07be96965c17ff022b15c3af9" },
                { "pt-BR", "265a01e6346f36b78a36dda50e88dd761bb0a9513ffd81c6a5b204b24532fdfe2ec7162bd8e4ae5784edd0ff4e7589496df53300cff6e80ae6fa0bfbdae73291" },
                { "pt-PT", "648a90ff317ff74c6d55d0aacf7a4a1761bf7179ee0c528d98899eda7e850de0943871deeb730a76f5e3633e647d7f03e544e28c0a162cafc97f1c40057755cf" },
                { "rm", "3b6d15c4c6e6a23d6c4367e6fae179857939f5ee544cb0b7ba9e7db5e2f701c3eb02c45035621feef098bd67b7c46b0d3a17aee7cfa52276a4108fbfff25a4c1" },
                { "ro", "0a53f7356891558259cc704bba794670499a6b2144abf00d0f815abebcb056360dae58f19872169458a73f96c2ccad5d5746b2fb3734752aa6ec6a3a65bf2072" },
                { "ru", "be19ebf49ae4b5eadf22fb18657026c495905bf96a709104b6acec16224576014a071d7dad017597ad9b152faae78fdc432e2b3f89aebc236d6ea08b26d4ede8" },
                { "sat", "5a9a2e7d343c532ba3b3a33174490022a800b080f98bd3d8c5a23faf1fbd4012d39bf7d291cd045eaa7024b8c6faa6123d76cc1d54c5fd04ad37b9fbb1a38fcd" },
                { "sc", "e3f948557d66de6ca998e25065b570d10603cd7f0725502781cc4e222b14a4ac83037a331221402d6332c116cb333adf42d9fa5ed491e14d046bea3b7e07a30c" },
                { "sco", "e296ac236fd1a542fc45d1441f7a17512e7c3e2d5137b97fb89a44e499219424018716cef70becb2b16bda67fa65a23c46ba13eab4a6b5f8532e72c03424568d" },
                { "si", "f5ebf2c376783d42691a711ffcac315e3bd0372769e3cfc07eff6fbabcd045a798f5dfe66d8dc1a24d1e06f90ec70a490ba08b1423158a54bae28598a84cc83d" },
                { "sk", "3934a237cdf6c346701ac30870b183864507d44074d0a2d0c36277892bdf5e75b6f31cc8a575073628924fbd39efd2765e4f2ee5de39d7891abb35e6ae6435f8" },
                { "sl", "90af8e2ac8fc9bc8e68643a669c953822f0fe128a8726ae77ad4219fd78565be54786c390a5fed9ba945c6b39da4273c5437b5443192d36eac24f2194fbc35b9" },
                { "son", "98c8991e4d4c3a4a6a318ad0deb03087065bea1cbbfbd225aba499fc02eeecb0a4b1eb2083558e67dd972ec4bfb86f2422fe4045236b07c8d5f82d40b000348f" },
                { "sq", "f5e618318a7d398da0382345cb8ea3c98024d4cb58a8d89de766568a0470a8c33cae81cfd10f89bb4a1a774c142248b1dc253cd29034a25626594cb63d745f7a" },
                { "sr", "a26e954283db3960e0dd3ff9c69ce589b19c577b940d78def55b6829e643a0e5cb829cfb9235cc0c83abd585dcbc5842ff2c3bf7b4e62b14a55ed84bd466e8ef" },
                { "sv-SE", "3aaeeb806a16599a63f66f78af5e8fb7b2b60d1e3e05cb0c307af15d0520238510f3931794a6f10ce47b04f5d307a5b2ffdd98c26ed3486ec1125feeec2c4f30" },
                { "szl", "928a16d95731bb35ec640f64991bf43c5632f85a05692115d614dc6cf24ac2d47e691ad93b525534f2f8d2ee74f4e8974e5c7e3f1999d012c98c418a94ca8498" },
                { "ta", "b80f82973ab495cb4299c4ab3edf8d333c330e506025fd33efe8b3c0a00545b8a72a7bb070341e566281fca9d4e8fd85a1ffcdc7f3c1df493623d616894c33bc" },
                { "te", "80efca0ad7302970908713c383ec3d003174359386fa5c43eecde78ab94fbac3b4081a7e373b63c1c1d935ae06982b2652364944af2b0ca7d10ea2631193af9a" },
                { "tg", "d3aec474e9360fd1a16e231834883d7b5aa95f12b907e40222178cbca2c0825c7671a40c2d5225b4661c3531d15668e141b07d81e5f4bb495260be866b11fb00" },
                { "th", "d55404044b2d2284092ad50eb96dbe19a0a73571c70773e594478c9ae1bac0e55e504efbf4cac85e6b15bc216fc39bdce3450a672226f85346aec2d368037c1f" },
                { "tl", "790ae4e04186c0c9e84bdf3c48c2c549755e549f5b9e7b8d60726447cc38396ce386693d9cbf573a101afc631a9055c6a2f8b0f92ae438bb814e50da5c4b6d5b" },
                { "tr", "6f286755d9e37dfe280ed8dacc6a521c30379ca3897a0d622a72937fd70d66cfd4c5b8cb55409e81d99b15d8a8dac6084a641932f87ab11dcf28edd58679b39a" },
                { "trs", "d092aef50aa9da45792cc4af6bcd8961303c1be70c48e9c9538e54736e392dba3249814929e8c7112f02d2031bcabca1d2918a66513f76a11553e61c7191262b" },
                { "uk", "88fbb45247a939ceaf70db7e2923a954a95a30c44250c6169894134a1c7c3c5f1ddd0676384f4c0bade5421ba88c2d3dda7dec548a18b10883544135cfadae4d" },
                { "ur", "6995977171e984ef0586076502770fd1249f45d66bd717c5487b1d4fabf6c103ef12294d74f8e2e9b4b13e3af6ddee79e071734128a532c4fbe46e3c952230ae" },
                { "uz", "ef1f75295cfbd6921d851999ff5bbaf556ed5a33f3f58cc6ae2141284acd94f22e2756805c295e2c48329bcec5845a9db64dd6c968027387efb2e2a32726d1b1" },
                { "vi", "5e7255cdcda3136eb2c7821b46113ce447aa4b24ee2c4beda4e5b8f688ae9d06a210669ca9a006b3713bd6aa444f496c763fab03fd53e770c417c41a98bb51a4" },
                { "xh", "1030a99bfae91010f5da8bf5b0b54e5d25eb15bbd0b41cef4c92ce72a43f00e6691b67654439133d35eb2292a27258696014c29899a517a35de50032fca2b783" },
                { "zh-CN", "4342aa5877259444b36cb748d86648ed7778e1946d3567d076eeea69826da56530675ab9a193239507a2417e4d2687ed89c02514441d58cb6dedac42985fa7b5" },
                { "zh-TW", "90291523d52485363fcdcb943d1fe5085e35d7380ff791c3ec50edd60355ab53ffaca8d226f362daf8409e4a6f4c984990fa0c133dd5612691e4de4848d47f6b" }
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
            const string knownVersion = "120.0";
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
