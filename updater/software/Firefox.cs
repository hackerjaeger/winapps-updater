﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
            // https://ftp.mozilla.org/pub/firefox/releases/108.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "8654922c7890aa9f405a0af2d214fe70b087cabde69c1c5f58f2bfe31f74998f0949500a85fec1f18c97ae821b9de13eb0d9abbee24dcd3e44e583a54295686f" },
                { "af", "86e1a66e88e27855dc0f58b22f5a967ed377a6944527bd02fa259147965e43ffe22b7d33cfeba3df98f76d46b51e6300a8557be41ab69043efa4c3ad507a59ef" },
                { "an", "860c45aae46c69618443e789bd166faa5d16b1040c65e3d5c9a094b8e49a1f8ed210cdcad0fab50bf220d2fa0a9537ee184243ef9741869d1e5ff50a6971e87f" },
                { "ar", "abb3eb33ea1de446a91ef63fccc692a24717bd3226cb04240b679d602f1737cee816aba1e268b0449f8b8486aaef36543b3206266ecdde20cfb6992d8e2e7b42" },
                { "ast", "b1a52c0e07a6e61550be93b95cd4094a1e3750259e66f421b78fdc98366d2ed21aaeb0f1fe34926d9f00496abbaeb1e2412ac96cf7325aa320396905c5186e51" },
                { "az", "b51202b06e49fed17c70f1358993d2125f9da2b54e578f0d35ffd250b6ea07e9d5325aaf684c34cbaed873672dd602fa81ce4d1151af0fce55d3fb1a8481cd59" },
                { "be", "9e411ff836734f617c26c089257951a447abb7eb2ee706469778a11883c9d9e0f29af803e9bdf3f7d9d0d983f08588f15c967740e159740a4830a29825aa89b9" },
                { "bg", "fa9da61e6d05ec767296953876aef8ff4a9be4aa121923630d0da047bd5f5a09ea72fc349e7c092c625265d84d43ddeb0be4a504616b2356087722332fa0e8f7" },
                { "bn", "913a0fdd972ab4a24eac1b070666f275071bede8f1229c0db99d3d8140dad345e07b86ba8c169e05b210707fd1c7066558030befdbc51403135f3bec24727100" },
                { "br", "e72ec36bb79069b6e8f191b8b743e29bc6e8dcf5ebc4532b257a7b01e756c16cc5ae09411ad4fff4ce0d84ca30032c37a3a13863242580daf8f47586b09c176f" },
                { "bs", "eebbe0b6843eb9ab69a4de8a0c85e844984f2be3888a9c555594b3effdb9a625f697ad1ef8df3266a9bf2b7b6c5592ac0a669ea9fd312d1f7d7ba1195cc5fd41" },
                { "ca", "74c8422d1a7621f1270b1a459f047c92e2951c04d8a6bd9528e618eb4b6ed5575d8cff212510cd2df45d574f16d1607ba761a237fe970e5a63324070f47159b9" },
                { "cak", "ea8681759fe660fdb0ab6d3b9937f797d438df7e0db2967ee59558b0c5a94823f07b619aa1cf6fd8d6225691b6c48dc03c041c0a32e97e0052a04e7de50b8a2b" },
                { "cs", "cc4e4e7b3f8b2a4b852e6af8f87b0cd12f619456cf6471f487ae4b5ff221b636fcafee4119e22c37a123c8219ec3189f6b1e35164bd15d45bca0dfc6dc185e80" },
                { "cy", "1238e5cefb5725af99697071365a9ad87dc66f1fb0fb12205c750741555d7d510a4df9883ebfdaa676ddebf9bbd9eb3237990000435108b495bea386bf90f0c4" },
                { "da", "a28f4c8fba5603cd2499d7ac14ea8e32f9384e5b97c80c8bb06d46c3763b0006f2839e31faada4de9c4a01acc687795d723221d35e919a9bcb8f8e7171b5fb0e" },
                { "de", "2e0b5d416b41c524d3556ce416e19894e0658d779b2d124b3c08c3f082d3785ea3270ae8d12849960583252d87ededc56968945c5334f2f3ace3ece950d9a510" },
                { "dsb", "2024236ac787b4e4af989f8a6bb44fbf1c43b2f5421c716c839c4a83ad200cb1d4c9409f5bbbb2d6e820d931797ca8f91638839f2c930c913fcb1ce3a6315139" },
                { "el", "99fe82bf55d93aa0a582adaeb57d4cd210c4b2fa3feadc658b7e7c04bc76f85d7ef41f08805534f15bb091719dbfdf577e1c91f932981a949d02ef66af250b9a" },
                { "en-CA", "6e19d91e234fc7cc462b64cfa75296a80a462b333f373a2da53112d63c6864ae70bd436ae07ee1966c56d4757be75362783c6547c9a2c5eab1bc3516b2539145" },
                { "en-GB", "5b439ce0fc8541a451d9bc644538279f79730d6c12e5f9d2da6bf6c4e79119537eced81f5cc5e370c9c216bf44da3155390be7d04562e0ca93685cf936f9a49d" },
                { "en-US", "02b556d69006ff5766c309ab1c51c86d5871d0d0bd26cb5a2cc2fcaf541167c825fb06cb54fd208747ae8aaf2f08a831492010f99e97c94e5c6514cf505fe3e5" },
                { "eo", "23e5bcb4aeb80307f1e184ea4d078321dc7a0eecba4a97110046ae1a8589c1e555da6320b96d4346a5f9cba30955af825d767810964d7c54f6a49fc1fc2b9576" },
                { "es-AR", "a3e3844caa878d212b7ccdff4975d266b323d077c26ea718c2c7ceed99dc80775d7aac0a564d9f847823116b4f67f45477351c7cf293e7bb765be7315b7dcf91" },
                { "es-CL", "ec749da3c2cbc192e714913ff4946b5094974e8de0bfaae1d0bf2f9da902df78ea1ef8ac2dbaf485b83d10e52a39c445c96b43848aeb28a7a095a48e019c2ccf" },
                { "es-ES", "0ea15a192c2340e5738299e25bd8b40baf585485d1f7a6f1fc834fcecb3ccb43dcd6a993e3a3781577411fa27d1b7297907198f53f82db300262cb0b7b412326" },
                { "es-MX", "afde300ae125d6e8e3de591a1c2d4d4c51aeaa5d1a1d38ecdbe73ac1d634eb0797570cbaba55c5487c48312a595a6ef91c73d72903c94d589710be4ce4f361a9" },
                { "et", "d8b25b3aba5d8e5a5bdfb4656b163856c0b8d6caf96ef78606c030bc0c49b7e9d56f62d32c15c3fe6d7dbe68dd297bc4c9d7276d74e8fcdd4c51757d8bf90fb5" },
                { "eu", "5b0809585bb17e81142538d85bca62c6ca8e347dd830f39d1da4fe9074ce282dde072afd91cd157ef94925f90d0dc6e3a513fe3935d501979e827a13b8ee8cf9" },
                { "fa", "161cda2a83f7ca94b221fe91e0c20e9d7ea740d921c1f276e0b8e0890ce3d1807ae9e091fa004f8ad678481c72c5d08c3192cef45f4aa7f3b2120cd9eb499af6" },
                { "ff", "0c36ed5530921a4dbee9db099e47ffd15915dade381c293a841a4efa17c02c8c544b949ee56a6a763dcffb0ef7b2ffdadafd2d3bf269c48e605aaed8464c2b65" },
                { "fi", "2b892fb5cb920f16472c8592b4ee5fbcbade73c5e471d0c32cd291a87ad32f5ed6fbaf39d5f0a0a98607fd84ca0c0fca72f62a38e86c52a2dd886187f62c1d6c" },
                { "fr", "dd63ebb6c414274079f0e1a65896b319fad457e57f03e45e904cb2ae030f8ac7d546ec41c042266a7f1439e982aeb5d2fa7846bb9e19c6acbc01716c16de0cfa" },
                { "fy-NL", "55a7c3e09f39e07ff1b5d69cfce33ae9c9a67f573471a1d59bcdbc1faa8ff60f9ca7da0c5f070906a89bb19297268532dff913a58bece1dc7847e95a6c599db8" },
                { "ga-IE", "69ab637231f51d2d611272c12f506562dc7de4ee760fc0b6b9a5f218b0fb1c1bd8caf3aa7f908796fb9fe663572cdfa268f4e39784abd2a9e059e0a6b89b7098" },
                { "gd", "79c7a8e8a1416fdf6c939b8534ff1eadad615a7d4d0773160acacbc64bc8e374447e6753603018d232b0d43fcf5b4204e1490841e14ddc984dca2305f949f0f9" },
                { "gl", "fa2cd1f2cb2de8ff11fa20fbeef9b9fd99800b8ce0535ac11546500f3f5360eae05eecf79c727d1ab525faa5bb78db59134b5e4ab7875634fc1f18ea46925255" },
                { "gn", "9830d5694dc0c1696d8b1740d85cc8e3b9d386e9f827c95b008d09776b6edc232385a17c2191d549f80b79bdb99668b368aeac1518f63765cb7f5573b62137b8" },
                { "gu-IN", "32a66878346f7816ca30600766700a778e00f1161be74d223719400c3471c1e856169d67d2f8dc946bc64c4c195d957b9a877ad743a9a42a437a4e81659c77ca" },
                { "he", "e14190debf8bc24abccbdaf3b75d57f0468aaabc432357355ad1b985b4e0c1ac3e45eac7ed6521fcd7faec97efb05f39e15a72e67e1bf6acca8249a10f2a3a82" },
                { "hi-IN", "4b0deb9b80ab635d0c7648bf5caa91ac0d314753b414b388d7d3981e8cc3c646d6d529cc51c3920ca2dfb16f88189bdad23c92dde75ea2e23b5940c3dfda74aa" },
                { "hr", "501908494e0ebda7da99c119054ed9f120f6c98ed0406a08686916ae35a40720391034e1454fd4da38f81a2aeb032635a0ae888eb8fdd5788fb5f3be2a7af591" },
                { "hsb", "a85b02ed376ae0034c2d6c5c7130b1b97ce0c2f8b956586fba29c2c111d5bbf1ea7403471380bc3371bf51ab2189e7c2e90df2c81eb843007232d8f9480dce71" },
                { "hu", "80ad119f87a47ca189945720c54eda313a798b20e9a766403fff464969dd35a45d1d3870d814ffca33a68185b5eff46a6ff8a3db9881edf3e7fca849e92e12fc" },
                { "hy-AM", "986dd4e174f5ab1d91fc9728373a4647d93eb704c076edc4943b94f698c142f9999a8fbcd524f11af96ed4b7519035ee8f201955fdb119d870e2e4a42508fa9d" },
                { "ia", "344723ead3f140a564e1ef010192edcac21ff8c32ae4fbba1522d2f6376ecf0180e21065bcdb0634c134f907716725d1db1203fa01d5019ed2ec7bfad7b9f2ce" },
                { "id", "c6c8e1272d5e5a94dc16fc8a132fe000bebb33507de903e679ee4125e6260546d3f023b6694f1202994ef912cb9726ce992d5a59bcef7f3e392c8309bfbb9d08" },
                { "is", "5c1160b50bfe115c0a3717292e5d9fbbc9b915e2a2faea94107e8113f48042642662310eb1d5b3202f47ebc4f496a0463753f57a9be74afec9be05c2ad5466a4" },
                { "it", "c0392e853a3f43ed26c590db3f30b9bd02b971041317b9800bf7f86b5039044ccbf26916d296b42874207809151c0c04f00431f87f3f5e5a8704bbd10adb19b0" },
                { "ja", "b486cfc6a8ddf3139e03ed980ed8f9b1242bb072ced3b60d05b267083d04d144c715c7448cac3887be038a8baebadc8cff5d131e30d5f366aa674437ce167d8c" },
                { "ka", "1afa003637626dd60302e83013e4d290a1d54b0531247b3bcf972ae8b908fca0de2c697d385fe2b51925c0b052401eb2727b1b9ce0b117114e775d200c3ad2f8" },
                { "kab", "0525bd115fac5d4fd113675d04fe60623c584755956389ab4d18fd6fcad7c07a9c37e4d63e4e101bc955aa7e45ce684e7db565cba4da0e518eb375b566c374cb" },
                { "kk", "dbb56051324ad73844b5b2534ee1a7b345803922164e678bce6129896936666695b5ca8535cc2e2c969cf91e8a6ec9a3102aa591ca32ad3751e31c85cc668904" },
                { "km", "0216c4bad60f509b38bfe89d73206e5e118376a1664aebe0ac4b04da2b8995d9abba26ec0a8191ed6e387697abb68b1cc2b00bc36857abfad31658c14b230e64" },
                { "kn", "324082b77bba07febe4d00f773d6cd552f03c68a689db7b41392adb3c85eb6e9d6c69a48eaad4fc6949e510517fbe519f5314956cafce0c1b7dc21f39ff358a0" },
                { "ko", "0cfc15102d5e98d9651503d289a0e30586bca699779f2dafb6b527bc23c83d2506f143179c8be52387bce442e48651e96dc4ee93f5f13ab12ede5456dcfaef8b" },
                { "lij", "719ed6603a17b06fe8e2f2d1ed94b98154007dc931560d4cc406773fdd6bff8b4e4d73b04a560b0cb7b95bfc16d17a44e311bfa4380cd2bf41303aa66b968ac6" },
                { "lt", "f73abf8cee52ffb8179ff9fa63c9c56cf091e77675bee32e7763856b4f8c08c2f8ef2d71264e387bb9659083f7bd3d37757978e38d944c8e59465ec814e1f657" },
                { "lv", "3109ed8fd63533275f2eacef6a4f502fbf68802bc9f8b1c209160c259ac05b732d3fb29844b2f4d3aa78eb313f83f95183326af9d2f763db5c1c8fa864c51804" },
                { "mk", "d20612cebf0a2b4eef803930ebf36ad7c136a39d31111c97145a852498428ba4b2da24cc8c1f0a6ac5e6ca0dc94a2b029dff2d5352e139d3e8568b9699c4e535" },
                { "mr", "11edce768cb3325a387c6d9eca62aaead2f4c21a31862907ff8323701bf53872641e8c653ac397631d4db6dcff4894f46eb54fcab6ead17c8b4c14febfce4a81" },
                { "ms", "bc44704e7263c26647fa2c2985687b90f7440b756e229c0e89187ae97f9fab27605b44b0ba63af2985072ab6a4892e0d3f135e9648903840b9c467648558018a" },
                { "my", "a5be80485a3e6e693ae841b4dbeffe995b28c936f691eee08f47de92537109352edb08e2cc18a92657a458d5b999823923c4f173b900686256384f51cc161be6" },
                { "nb-NO", "0a26a0afae03fe6648962f4b7e9d07572749f0d652bd4e516cc19fd436369c78f06dee1881c3265bc3c2f94a0b4d343e8e5ce30fe895517352e6d90acc51b951" },
                { "ne-NP", "a1bc6c7a67cf4fb4cc8a5c9cbf8163a1af5bfc3c588328c81b95e70a35bc556303cb4c18509d394b9b9ff2a2bdde473396a4448d014ddd40231f077acc7ada45" },
                { "nl", "0201576bb97ae90af5282752743f1c9805162a0af64d28b9efddd4fa8fe3fb9b917d1d65aa8f7bc4751c599c4e54624f2fc2c227479a8eff684d51cd3f1cc9ac" },
                { "nn-NO", "59605953feb6d166cf1c76ad644f1d3e7f6370c76d5cfe18679609a6f14a6fac53687c375a865a30db3ebf50e1ac1e4b51e94f4709e065506cb0769798501fd6" },
                { "oc", "381f51aa115b46070145bfd9d7ea3279bfbdd9a2bd7ff0d9c2f84b6e85a7dc7ab7893dc1d4d0a9aa82c60ed6c66a7e30366c984fe8007c365d92030beb45ac02" },
                { "pa-IN", "7690359f359d800ff757bf6ad31fafd61ca603a56bf15ae09df71e0932128e042c173e4f49135cc0cf8c38a998df3ec5d37d3c1192c3214f7099c4db3b3b04fd" },
                { "pl", "9622e3ec5a7fc056d1ee6f5d856b131fed0a8dd13cbf0a18d2e080051878d8acc56b93e2644875217022324c795d810e1b2ca6e43921502b6baf080a9ebf5a4b" },
                { "pt-BR", "af079c8216ce06870e499ca1bf70dda3cf36a96017ab024c60944e7b46affc0f6d1a836057e24538ba5222446e08d3acbada43ee95992d23dc675aecb8b58e97" },
                { "pt-PT", "88c241258ca29a7d6e359f35bcffd47cdd94963a869b2393e89faa64267c69d68e05758e3088bda4d0faaa0710fdf8002f7c7ba04d25acab9415404883c111bf" },
                { "rm", "ae2cf69ad7312ba16170a98680a7840a9fc98fafd5164f8782860257eea417c39f14847a8ab41759604d06c984af5692d1ebb866e99d64a99b04674403ebd36e" },
                { "ro", "b85144b73ff292908097b3353a339250e8fbf6d3d75b6ec2195dc268a747782e4f546bf279b08d2608079783e1e1bdbe51a57893865cf323b3ac25e37dfb5496" },
                { "ru", "68fe369f17a4c65edba7b3e5a2a4cc985bd5323538a7c845c6eadddf5755ee52b49548bed9b2b236b565093b530561ddf182c05da6604de938b875dd59c2f881" },
                { "sco", "0394cece79bfaa6caea632c274bcce6349a2190393dce56beab83629742c223bc7e903a5bec2f1e7cdbbce06c2c98ff872f24abb7dc82fe39fecd30c39d904b6" },
                { "si", "7764295d9ce7d77d2f85129e980938fc4b0814de5b7b9ff62f747f72c14af914d70834b65edf76908aa2e1965235ab013eb2c1191dbb5be50969ad03ccbeed82" },
                { "sk", "bba650bd6fcf88fc35ae3babd905ad301f8582fdf9d13912ec74e632b546c2e4b62eac3c0d6263adad8c5ee8dd7274d47ca3c8be9293bf0bb22827753aa722d5" },
                { "sl", "ecaaec3d90da24fee82521d406eec1e7adf9ade409860bc0d77155c089b91e8c1ad630cdfc47cec0635aae81ff216ed663d3a97147cba220cd75f48705703413" },
                { "son", "2fd131c9bf3ed7d05248be6193ca25521894b694b0dcd462c7bdd6feecd907dae6594d321f063d96a284d554369fd24e1b256087e4556d8a1cda7244feff550e" },
                { "sq", "c4a580602f748a5375a389ff93fe82662b813be52954f2fd890c9d58edf6eb409f40b1305a5858a7233505e832d3bd825dc58098d165a7b0776dae0548732ff0" },
                { "sr", "628ab713b60b0fa6f531d3b3a2ea2436b7b8ea429a52902ab046f565a5680f53b72ebe0de94d2dc89f2c1a03ee946300be018c4ded6d1e77f1656a490d9b56fe" },
                { "sv-SE", "743ea99ed6117909746c8d10c90c177a2513c86eaddd9292930f9f27b79a5747977911e161b68cb2af2bed8027cbfd52cd80d8e44ddd72b0be875cfbde515d3c" },
                { "szl", "920fb32ac6f2d5af21f65a6b7bbf0ade07a7ed4ee25a96e2b4e824b7ce19b774c514b569047dca56eb36472411dcf1c10ebe9b5665d857c787e3a5e3bd7d3cff" },
                { "ta", "5f10e2b17dfa0b0d6f5b4f9666b76b6b35db52082db5a840b51cf87b2cba22ba23a6b66f7022ccbbc17191b732eb2f9fb4401c1e46445b04642db1c8dac85746" },
                { "te", "6162d4db07a7d4f84ed79ebdb99b2e633b2fa16ceb1f83b44451566c06a2e8431db8a7b7ce03792666a87722c72d991b86b243be02cf1f7e27941ae9fee14590" },
                { "th", "95d1f1f041e413aa342e1a6e91c4abd9bfbcd199bccb8581ba7ee9cb34218c23e768633c10412cd9fc16b9dfa3f5c606b52eaa42cf2cd783de9171eb21d5be57" },
                { "tl", "dc8d8b3eefc349458da57df36ebc468f2ef432f7f9e49dcb43da8de99fdeb25dfe9babe01881bd98d09b0f87d1971ceffcecd1cd90796cca19f4f9452769f296" },
                { "tr", "3cdd7e60d25e87d41fd24582fabb1c532ab39ed12b485f26167e256465fab5fcd0cecbdddae377f505bc198ca51f677a46f275569a1b60bd454cb336c949102f" },
                { "trs", "c50de1e77340211b5486370e5367e16f65a35ad5ec21f3a869bfb84e8e249dd7ae35d40a6b871a00733ccfca7eb8b3f3a729d40cf7a2b50213df21bd2f43bddc" },
                { "uk", "bffc289c4b68caa1e8a3b20abf2b00f35b862b5c6f8ac97e081448086c5f77ad67272d929284a57f5a1147066d68154c887b850606909613123504a1f245480b" },
                { "ur", "f035e948ba44bc8733ad425c5a120a0308defc3f4f46bd6bc650dcd76ec227bfa366c287327591eefbf062bcc1a66c3a62b4c992336cad904c8a034bd84b5b55" },
                { "uz", "d5b3ae0c94a3118c4cd131f59f4c9cd44c213c680f24dfd785493b48922806055a7dc1fa36d7f09caf0dc28246338a12d01cce92f8648a9bacd6d2b21e240b61" },
                { "vi", "c55752efe9786bc8e9c67b1ad2a6faa931036282383fe86919577ae79d774347fa1249e2b7fdd9cb7aade96b756e2470dbb86fe5ea72778b482ff07dcb2493bc" },
                { "xh", "cf2013c8c26de4f103545c0fc7918cf9aa3d22ef6e2f75ced5c5e8cba25cb222f33ca21955da7335862204176b614269115716121f1b3ba01fddaf18503e0a65" },
                { "zh-CN", "6436e8715e70c764d4ef11ffcc8ab3c627cbfd15e6a2dee4f42c8bcde0108b6605646dac6edf98021523194444a0f008a43ae6eeeba4cf1d44ff8945e794536d" },
                { "zh-TW", "a863572406fe6ce0b3c73a9e866d1198ab7cc9bbfe8c1cfcce0b1445e7d40248b37eee8e4a2b16f3ba48c406a76ef8e8c3b51cf9947fd7658d067250d428746a" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/108.0.1/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d62f9c2d8f38f179bde22a1b8a1868b41b9e9099c86ecc4f522cdc58f3770b8edf7d00a8f794779d97a3fdddd7350dd1ab6e7ac63302a2f03171bd5e961de25c" },
                { "af", "790bb49dec990db4c4f0ce6b14f9a61b063f2f2082de7e5771adf936cd87c313aca6086f3901eabf994777934fcb2941dc566e6bd64a7dd5cbad3d0f3c57e7a4" },
                { "an", "aee4046dbc6886f737aae742ca0aa9be0c3f10e58e276632b16fbdac995e4baf4aadff50ce83ff8cf65cdf0ca9e5026fdb09d5a7e281fb673792ae714c877c06" },
                { "ar", "429c0252844c1eed0cd73ae39ffad083888fb0794dd044d788ab408f514c67fda98cbec6913ed8b57a8a24a231b51e3747e0102b5cd58df74d970e13d70882a4" },
                { "ast", "b3e92fc137a701b4bd3e5f92c543117af62558624fdf4e84f1f6764e8dc54b9e40f116d9facd257c3747ba68214cb31d9abfc5f7dbee5691ea593a730881173d" },
                { "az", "2faa2eacb3c4727f1aa9b1a869c48d0d2d02a134ee36a855a481bd3ded423f89f5ae1fe78a19bd06ed60ce1f30f6601136c51b9c03ec49e2293732aebe43fdc1" },
                { "be", "21202cebf4ee1e0b4b3069a6ae170337e5621638c5b34d695e2aed7f0a31165caf871b2846d87cb1faa0d266f7dc9a91cedcb77fc5f145f4716972a49ebdc716" },
                { "bg", "f00afe39af2b8dbddcd796b92cbac0c3a512a65f17dcb37c4e18745449918270dc9c8b20f59cb4cfc753c525bda47318e8ca99361618f9bfe45c8ce67d2eba2e" },
                { "bn", "46bfafbd314f693d9060fe8962e8c7887c3ce2c15c30e9156b65501a44fb0d729d0860e2ce5895832ed3abebdf073b97be473ec96079b3b61d1664301c020874" },
                { "br", "88d9275f0e6cb971592df2f1f5954253134ce82f3fe517ac91af6ccb2c49cb3e7f68f255e27befd18cfb3ea1a39249a5b2d83df8a85d08d1f5afe7b857c7a921" },
                { "bs", "de9575744fad135a63cfa524e2394576788ab4f6ecc1f4af1a8283c0e5b9d05d6fe2de68b6ab74c71d323778e1dfcd704f5e5c194d81cebd0af139a249888481" },
                { "ca", "1311cc2a6a41aec7860feb21cf2814fcff04fc35c9be172286737b78f62e5d31c8eae11e0be6b7422e78c581cb8dd5cc3659c88cb22ed58c4eb10aaf421d674b" },
                { "cak", "a097f37b2ed5de4d7cfb6d37a9ae13d074a0d830fd26a1499d8c99ab51175b86ffce4472995670c1f0b07509f56659af6aa1d615ba57c12f245f3a4c5fa4c03d" },
                { "cs", "b8d8167ccd27f7f3032b23b416ca38891bf3cdc357e7b01f72a0355673461851c81fec81a3374b2b5e42ade279632bd107998bd392ccdb00228a8ffb7431f75c" },
                { "cy", "40af6c7729ccb7faa2301d7297ee8277389cf99e86200fabf9d6b48ad5e9e53fa966c09acdecc353654b2618b800cb52f24054748538f0daa196c7b1d87ed599" },
                { "da", "a81e6a5fd9f1293b6e7811134aba144e636eb8ca15a287f2300288920cf226a41279d92d23a7ab53e6e0afc735df1ef671c4f0bc4552cb32731a826a2eb6d6b2" },
                { "de", "0673e9ab5a6d627ee08e96deccf3ca335a919d3a84676ef2171f19549db90c2b97c0b785be2a1131f9938d26ad77861bd6e4ac3c4ed891ed8bed0516db71407c" },
                { "dsb", "d06835a7250d88303b096f63c275bfca3f31f99c75d99d2740c8f1bf5011149a9492e6aa177544436d59c85b517a5f28995a3f5b4736cf3f201b234a2e0ec860" },
                { "el", "d4c4bba5be9fcc8d172c3ac7897b6e908049fd88adbd81cd0ef4c56eb74f655f152225e7ba03c5b257cb3bce3a512afc9e6e819be429710d284fde4c75471ef0" },
                { "en-CA", "db566ab21500469f3707f5e554f759f8555eaca966b6fcc2f2b43aff0a0eeacf2a1e6ecc176b43437ada59dcaf4ae68df5788c4b6fdccc2b3232f4a16a18ea53" },
                { "en-GB", "e9e8555e57c78248aa76934a067a5ab66508257cd43318848e417936d4479b7fa39731e87afc3aa10e9b987b59c386a22fad31ece134e5bb8d0c8b5677c76f3b" },
                { "en-US", "58e7be8376ec5facb77f8d0dfe46aad8acc7b63729a0d51082710a219c509660992ab3d3f32b7e4f22b722c941ec3dcc28a51af6e35434934faab56f1839c756" },
                { "eo", "9f2020480e6a49f2c1528be565900c7902dae5cb6ddc42230b3678f0c09001a68fb46e58ce4ae91bc1742988e44a25d8839c5cdcd66718e3b87e76f09bbf2921" },
                { "es-AR", "ec604b61bd10eb5e16f124cd24cc890f038af19a43727931b9858b09d1c0694549c560a08c7586b69b16d5650220bcfe772cbac974dbad15626006e9a50b95bd" },
                { "es-CL", "2b3dbc7e584f7a0d3ecea4a7fb45dd0e9e240d7b51eb95ab401d6b6b28cb86b9f1eccd434924944daac310e33ecbeb7ff242ee8a7a567fb0dbd2a47c0b28c019" },
                { "es-ES", "ffe6b4eefcb4999b6a81e78cf41db10fe431bbcec2dd6a31f39f2841981528f56622ba66385c57ba577a3626c40ba1d6ac103c3213d79510729333319d7cf16a" },
                { "es-MX", "4c69240addc494e273180e32b14f424f44966a1631ac4f4f0f5fce58e72e71bf80c61967da7ee29be557da6f85687db6a811c47523a964a64d23eef3e1624925" },
                { "et", "7dad579499bd4bd6d163a68e5385c6919c8977f4193241fb67468d0fc7adaf8266f46a32ca7705cfd54c5c5a5c5dfcb2ad78f4ae9f74070b7e1997d2ffab32ea" },
                { "eu", "5df85f660ad97ad53f9c1198991ecbe3bda1da9d6a9b7e19780a4307b663c8d6e60cdebc41cea7352fd90ef8b1ea8308b4c53e2993a99e9e98be266e687231d0" },
                { "fa", "2d4f429eb31d36c4cd2f0966d5dc233d1dbc4e43c1fe8e58736cb67cc009a56b04818d0cd6356ca19bda5a4c023cb3e5fbe598e015dcd46f79b44af61fcb487c" },
                { "ff", "58bdedb90fff544a8e7a2eb5917a9749558fd9f19b0f580d99213adbe7b1dd505e07e6435fc29f8c66e6465c43c6f19985069dd60232ecaee08caf85b48d95a0" },
                { "fi", "db498b16b541c7d981c173e9d6f23f2b820275dd101901f4ab55c1a66a76444a778c6e197a390be6d7fe8c5a5f1cc1e3406617bd41108ed13fd905655d489804" },
                { "fr", "f75b012dda89a69c0c30bdd20186c3613e2274743cf901ad2bd68c7b8df4776d69681dc60c7ff5c99de99407396608bc348d291d03fa9d7e0433b8fbeea94c58" },
                { "fy-NL", "95f05980ee69c29d53e8edaa5e655618b8c2c20266d3f1b01d082be9d24f05b870b09422323092bcd1f61a8c791283fbbab48ccc27de2083e27a79fc3e46ffa4" },
                { "ga-IE", "7dabf0ffa880b87aeaf7e083082a733d287bff5be244a2a1b67137153af11c182a2a66ecd6cfbe6ea7c39d68ebf13bcf5847355c0a6d1068d63a53a12767d872" },
                { "gd", "c74361127886c314b514cd9248ee6b440ddb81bfe7448071ba13e11899808c965fd232d540fdf86470fe6b331befb4dd8b71032c55f21663558d41045ec1880e" },
                { "gl", "111c7c6971c8dd5432e5b9baf2629a2387ee8d2184659bcf557fb3280213e31c87323fe3d45c0894a46d4b6cd8ff7e2b2bdf2e990d339d1283c90a9ee8566f46" },
                { "gn", "7d036e5a7d686ad0946e2bb74647112ad87b7ed551087c4ab0788c1bacb0ab240b0267fcb7988e35a27fb1fa4d08fd7d3094c5c5f65f567e68e3bd193eac1bd9" },
                { "gu-IN", "10730ce7566ced448bd5ebddea95f4b015297d4163852dfbddd4b47a5e21438a751d047b37d73e538b0b6cbb1aaf732530ff83c79ff25633dc3c23a5b826f598" },
                { "he", "7f93d3c586942d2472706017882513b98bac3031b197e753d4b31e5a4dcef5be9cbf480b299ce0e542353dffe9b5379aea596c900be78a633b102281d3ce3651" },
                { "hi-IN", "fde3aef75f92d7e267928eb887a10b0a16461e8b8cba4afecb521b75ba1b2d00e2013955e1cf6ac4eff217903766c88176727bbbd9716386585bad8d76b0c1cb" },
                { "hr", "640a8b8063292186ff33883dbfb4cb6e0f4861b8a9d25f272031fd6b3fdca6bbe5b47e72e609237260d9e6f32f7dd769397cf335653e0b6977681d044a453878" },
                { "hsb", "e6c46bbb9dfc8fd6cbe34f3763db9c3ecdd630a689935d9dd6c93e2e229a457fbe35c3014a35c13fccb9b9bc57c456a082b1c5d5659bf5b251a4ac892afaa8ee" },
                { "hu", "4514306581505f828f4ab1ca94c1d115c4a5633d3fa1d8fd0a85f97c72b8e04fe2cdaa736611e8334d0c9eb1ad0adf3252020edb0c0c99bbdbecbce812e2edf2" },
                { "hy-AM", "1690eb30adffc5e24ab413f192a51c8a93fd784bd1c29724a7158c88d7cc20a2e802f6b63e08235d0a42618f9689e9131bc43b1ab524c8984ab4e370d3349dfd" },
                { "ia", "6b446b43d21fa875589bf48122e27f7cef7bbe93d7a9ae4e6f111bbe5008a0140f4d24cb6a697c22c66974db2d56178feed0702d8d75d319c2a975c91cd9c0b2" },
                { "id", "2b2239520f1a92655446ee75f4630de93d569d4d16bd18657f8a69c08e783d194a62022ecfe002cd67ceec70aafc596fb7d6066187ab32c983421a9c2550a601" },
                { "is", "28e848074fbd7a25eb44cdfab2b7994727c06c34b579fffa0dbeaba9279162247d73e0574c94d79c80bddeb4a4fa7fe5c94573b984dd023bab32aa9919be1668" },
                { "it", "0400922b683da9162f376cf74ce7a1902828ca9fce2cb60e6a718e35949b2b950d5cddd906a61f26183c22d2e695ffa492d40d1d07c3a5af1c84b19254c08c9b" },
                { "ja", "8374f3e38942bde57af1f2c741a12f7b1e1a396528dbaf60b18fdca289c39336638539d1553e1a306402f0f4ea34b827cee118cf4ee14fc3b0084a9d31c94fbf" },
                { "ka", "9a449929765e992d5fc53d8badb3241c1bb6f4dcd7482d2d108de39b75bf2757253150932537aa2e711406192ecd4c98ab5f4746d0d8aa65d75855699abff893" },
                { "kab", "228ac17746adb99d69e1c04423507df1d307c9614ae40156afd2d467d84f4ce0e2158eea2b46a86934d365fb18dd9495eda86d2c1f4404c6651d5106a991974a" },
                { "kk", "b0d958d0ddd87ac767a7c4ae1df1dd838b7b4dc4209148fdd821f00c3320c69c8f4fad1272fa7a7f82d2aa65ba035b5bf65f358e8036b0e72f61e3b84272601a" },
                { "km", "1992b2ab676213a6311e7bd54642a94bd6834869bac340111b333383ed0f285d02b9317de3f8d7b73821ae608113748e4cdb45a661f61544c7e4326011ce9649" },
                { "kn", "6631e5ae6719c51a5dcbf7d4f3913efaceac824329df4aa14be42e4f3ec633aa4ce1e68625fa635df929bc3099991b6ac50c839818ae75428ee37448b84b6380" },
                { "ko", "1c024443eabc5ce2053d1d91ba40fefa5edf88d6e7307f8cfb8424a1230f271d1bb8aaabeb12fe538239f204a8c3b19f1c32139c9f25ae937767333bb5523fb1" },
                { "lij", "8964902f5751264a18c84ec0a41bbf200215e2465013faaf724d6b0fc4ac36a8ab2807c366c94dd6770b8993f51c542aa5c51e53e09936ba2ee91bb8f7fa45ce" },
                { "lt", "177ddb92da10c99a590b8226fe316f4207e422ee46b436f5a29862dea716acb0cc7b9d214cda958926540e10cb7b3c67e38c9a5e26afdb1c84ceb5bd2421a181" },
                { "lv", "4ba6541559962bddd8acf50d5740349fdc60f6a54ceb17d81f5219e4e3ed8425a12c86505fdf63bc125603fd73b9408072d071594a467a360f41cbc5c4f03daf" },
                { "mk", "30b0105cb013d3a5da29c88ad901adf2022c3d5471ff19a6d78739d8bc43ddeb7189b3c09466618e79640ad7ca707d02d35eb9861fac26ba99b9fd486f06d0d1" },
                { "mr", "12de1cabe479088cd27757e83433c211d45066240f07862211be1c59ed6771c23175b4cdff12412c32aad1da3cef1cb4b8c1b3baef8b2958c6d46b84e5955d3b" },
                { "ms", "8179596c8423dacb2bb97734a9e25715f20f81ecdb7c3333efb508334844392614972ef2ed9160a6f22ce7d428094b45b704db136d5b8c44185fbb27c8491a0d" },
                { "my", "7a9467fb62b526687f78a6ae425ed5582fc683f7f381abd16d68225089f4ee07c3172ac4336cdd3bb6017305b802b07d357631b1d4680e9cbf6292f095bc87a5" },
                { "nb-NO", "4461ebd179b7f084d002035022caa975fb80ef27a346cf2868d1c71dd7f0905f83336e51282f831537eeee2fd618e9622c94d6ef9b4d1c78ba71eb7059b14051" },
                { "ne-NP", "7c7c455729e84fe0d96973ca3f85a9774d242e6b7da43b3cd409edd6c1a4128d758a90aa8365c910652908201e32ce20ac37dbe6e85f6d5f52a5e0a913948e35" },
                { "nl", "4f4aac635f47f8a5ec2dc997755f33331da6cabf46f7706e72017fe9ee40841834c16e697a8b977683ca0269b396534b858e41cc01e3a8434b0065dc1328f9b3" },
                { "nn-NO", "227471a404778b0b05b1e6b6f99cfd64a43beaffe4053b9ec51a231c427896a42872621d0bce571b169d786d99fd92bb7fa5369dbc2b2b3a1e8108093d9edfc1" },
                { "oc", "54002e11011cbb5368ea5e3ed3cc563c898638105f3df195dd5b0083f2d63b8ccc6ef69841bb1f22db28fd2a86a5fd1f9866e83ab5e7cd2ca337699a996fec45" },
                { "pa-IN", "7cee2db65363c59d5d6367efab55273bd87118081b3fa83db730050df4cf77d2964095e4505dffc675c9c8e5601b5d90419de6fcf57d8269e0eb2b1766e68adc" },
                { "pl", "1ea3ceb02c38e6d93627011300e91465c871055ea484456c3e84958215806d2f6db45a8247d756e471bba6490c1542e5b936f1ba99672e7478d77f742d4936b9" },
                { "pt-BR", "3aeb9cccf43aa60cfe06e6254bf47d8fc9c3628dc200942d5a934380f746880359fba484cd1195900a0af2206da7ee92713c822648de8a153646a69703306480" },
                { "pt-PT", "d5d69dcb8b99015975189ff78436426c06f91fa6c01d0e22940aa80c0839649be5807f3c3ccd4dd8ec90ab60f7ea84f46892a7d500cf22a6541a72ab560447d7" },
                { "rm", "c2be1740864d90d2e74734abc9adeb917ebd2d456d879caab8cf9e45106c33af23587c8da2787f482466ae931fa31f20ba242722f0977438c5112a6971da18b9" },
                { "ro", "14d2339954383244e38e0388db804155d07112b94aab38f030ddbf124adeb44b4126ec6f52a3be52c22dd305dc68287801eeb02a7b5a9ff0f15c1583a665e052" },
                { "ru", "de30e707eff3d71ca9fed4ce94281a2b0a156678602d200e679fc3341af7da5b4a94aee3d9c4b32ade0fdd95d137dd8fab22b839b4f73fd97e9f906cb59750f4" },
                { "sco", "ab71ab07e8cd1b10c83be667444802c831f120e24c67f6381eba5e538f0cd9dda2f485187c319c50f094ec9a08f1068e92a8fce510d063919315f84f3d6fe57c" },
                { "si", "9f5d56fc116f3d3d4c4084be22f6d5916774b21dbb32c0a8b6fda7a7ec8fa5eb6361f559d900421be2f30d30a400433aff727d34c4e36c106e240798cc80d921" },
                { "sk", "1a42c49fc3757642236182eef6412b40549ed8fde67d7160947473c0865ce319a8b8fcdff43ee402e50d0afc86c36e187ffd3b17b9b97afcfb0f0fddfc9eb191" },
                { "sl", "9417e246c8660e9cf5dd28581b70ab40c3b835c97ffcd724cb1934153abd5dc11dfd63668ba7c07476138b10406a84bef26a103b03ae9c07441f5a447474b065" },
                { "son", "eed4b28373a08e9a67e06c9cc090405a60af15a5be811586e0dc610af7b1285ab2cfee0a828e1402d0c5e23f36cc3c77678d4e54d98267bc86ce03d392db2f3b" },
                { "sq", "c1f8109eeb0ffa1bde06107fa17e2a7da44d68a100a5ff4e4f830e2d00d07d6043b404ed9231631e556027522a38c449d1849b88d34f0e16b83ba2cdd27c21d3" },
                { "sr", "972e4914818228baafa5d7f5057c4ab5b3949f0f5aec5ed6c0a1d259801cd6b9e946b4a1f5e039135eb77a2fc841bd87af56cc859d956f107cb4b5c833ebb0c3" },
                { "sv-SE", "a6eb7b1d700fa28099c70f6dedf6e090bf30ce62c1d94610153c196086fdb24e22cc051ed787030ce46c201edaf70c1b8116ad985777f63ab87aa93f27b0a0be" },
                { "szl", "c42cc6856b3fa48ff0297653a62edff3972e988c3511586f60469fc71b4971180eef980bd427fb6b5ab4b3aded2caa9f2eeeeab5a569b8d52983bc167124ef2a" },
                { "ta", "aae69d386bb6490a4190084d68eae35fa3520b8859581f6b71d46a2ef16292e56dc8f4a2f7c5942b915929f5022cc5fd7953f45c8daffa8bb969ad24efa4d74a" },
                { "te", "4a20f6e226623f58758a6d7d3fd87a1007cbe48412c7a7d92c49ffb4928d9e2fa365556a72f8d2ffe6d13b8067a1888de0e6cd1abeb7793ef668193fd3fa1982" },
                { "th", "327aea1b680c6424e74f526fcd3dde40c6cca1aa62042feae2672f1c9b0ac585b15cb9d45472854b6316a67bfa5796f480888e353fa22b5a0a1c5b33887a50f7" },
                { "tl", "bf51dcb911620c15152fe12e6d5219e7eebe351846a18d6533702c25dfe736c602f63ad32855120edeefacae05ef2aeb56e5d49feb0f984bd6b4807b87a3df4f" },
                { "tr", "3eaea7a84e193f9718a1019e5d252148bf6f61885c3f85d37302e2f012f0f77e5e7604b4c74e6d5c449ac49e573b3291e9ad5b1d2a6b5cc1da9c7bd331e8a9a4" },
                { "trs", "6893d2480e1a42895d049259c5675ac1a89e805bf2aa47575329cb6d38a10ae2c74b0dbd031fe6b220c86673488a8adee5c2c74ce280232663d1075246aa0fbc" },
                { "uk", "36e83c39aae1298927b7a65f0263db402cc373b8a77a9f396e264df8dbcdfca4b6fcaa2b93cd41e9863070087b4cb9fc53da61691a28dedb9e6b5fa64e5d283b" },
                { "ur", "4182e2f6595cadd49af28ef9de167dcf647104b21f3a6eb987f980ee401ed3fd44bd6e09ea06131f4874f40403e2c209d85b745bd0bab7324046985adb2777f9" },
                { "uz", "266684477c53b076b02489bba35e99df1c896170e0c185ced976614df471356484ae252cf467613c34b1f7572c3f022f11a5bea08bb4875ae9eded0d42bca275" },
                { "vi", "cbf8e80c13c71fba83e742f0d93269faa1ecec4c514b42140946c6b1ee43fec024f44fb62f9d5f60e199e0f86d9e6c4557b1a7f84c6b5bb00df1c700584c2ae8" },
                { "xh", "1b9da7138c4a63e58e918a9346c60c3a0c50b5baf9c88d5c79838ea3c8a386f24e59ab460a998b7ab3ad76d77f9113c1913bf05e124a4161ef5a348730f9d311" },
                { "zh-CN", "e48b1eb167a050d86bb13cfbe4d2a23601202c2c5a0954aa8173949bdcbd56978c45cfbb427667fb9d11c06343c2a90e268ae597d85af9e2e55a062713e97445" },
                { "zh-TW", "3789eb422bc1a32d98191278a7dc3f9ddd7c550a50b86fbe0193773765104a02cf6084b82d6193cda902fe85956520f9b1c5e812741e80a9e1544dbd75e476ef" }
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
            const string knownVersion = "108.0.1";
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
