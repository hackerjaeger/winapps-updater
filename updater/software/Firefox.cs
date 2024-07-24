﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "fb001d6fea8ffd5e762c6a4c41d6e1f61412c7b4a13195e7c9fcdb8808c9756ed16cd3225cae22cdd71e9c0ae871ef19aa341f916b9d4fe8e8b71ae4567bd7f4" },
                { "af", "717a4b35c6202ff17304e5e0f587d723e63f7eda5c27ce77eb3d1bb14e86fa884cd44347094da4bbb5bcc90f5765bac1588378fa08adf044b3a4180fe8fabfe1" },
                { "an", "785eb896daf581b49bbcdfafefdffa0d6516211e4172210bca7fdef8011e402b21b4f7dbc03060cecef209651018fb2b3b771da78dd562e201a7f4b9dbeb4afa" },
                { "ar", "536080a38c6fae4affe30c4c4102726d86a9072dbc2a5c798c5d27758f6dabb015636c4e7aa7f4be406465f14046fd033c65366ee0a54ae0cad4e831d537b5c8" },
                { "ast", "f20ab10f46a963c09515f41335939a0b6f4b533c7aa60030c146d2c155d7da8cf4610f35de058999efb2d083b62a33b45eec42d962c546812b0ed53fad945af1" },
                { "az", "1a6047e7ce9886dfd6f54bb71249fc99f7479b42dda2f5f07edb97aac499bd4c3e07101cc09b11fbee3fd177847cda840f86f9a4f3f97d4b0598968e4de3931b" },
                { "be", "c2fba4af2dce3109a2684ff28d109663e7a94e9359a7f7b57f8dc81c488e892c6af0b8c4c2cf909d928e942a37aaad3672d6df85f4904e13dbb0e84bc1f81013" },
                { "bg", "1c892b2606dfaf6a0b52a5d48023bfd9d04826c0eec5ed81fcedaa84cb1bcf17d74acbddb541cea8a5f391f171bef8cbe366e353aecc247d18685f751a4f2039" },
                { "bn", "3d5afb2c5b6887dc2a4ce073841f1c3391e1c20786b98334d4ecd03ce95168b56ee318c403bf750fbe0d0653845c7df8f784fa25a67ddc3605203aa3802aab7e" },
                { "br", "7d88f77741d1995ccecf867646b5e1f01cef39c5670ee960bbf3a80a8358154cf07fcde42878a395f7593e2d9df81cc4077202c88968c58c9e45af5a2fd0798a" },
                { "bs", "0d872a2a8e1cb939f7e34041299e8e56a601e44e35d83a03bfa9fbe0894ca82d087ba145c57d872743471c1f24ea506ce8810d37495f728a82a108a036300c4c" },
                { "ca", "80ef6799974b12dcf2cf61cca084552fc71332fd12469f5a06031e78be3283a8c943d096f2b0cf050f84cf6ae147a5b882f02b39b0bed28263b653d5d8d7cf50" },
                { "cak", "b7142db4168a02db771778d839405b72bb93f5add95bbfe0930ebaee2bf972beaeb3d9af39e5aedfc1a9381dd794960d1104efbf4c1260cebd5a909f9c0a1f51" },
                { "cs", "01e34f6872741ad31c2506bcf58edc6fa24cd4cecdf687a96989363a98a29cca4317c22e54cb505f8397593b2833c2a0959c90fef373675dc6c6156d8d98ea18" },
                { "cy", "250c0e4d98350125a07cc223c16953a6cce9f0c835e50911d1a98b9960e043eae236202704e117c1e2c6223c6676e24a06b4b242fee0b4c4880072625e7b1aa7" },
                { "da", "81b2b67650d74427936f0f5d11c9a1d1282965276bb876781e9c7dec313bafc7f5fd2eb0fce94a30942e3b8cf36605b2616515e86cd2594ad5ea4506a7ab98e2" },
                { "de", "9f8dae29c13a68c8be5b7ff395217c4939571d151c3c82ec144d2881a04df963ea32309eaa5ae514fefbef3bb426e1985d46541fefc76aa1407a7b2a704bbfeb" },
                { "dsb", "5d93c88f8887ffbf0e83fafd1aca665dc60eddceb11d59e2f954bc065339bae328154bfdeb073dab3fcb5f4ebfe81924e0571aab5401842446211f3f0f843167" },
                { "el", "cd2c7fe910245c8dd29f5387cff2abbcecdb41184fb30a36e24cd86811729497000e696b76d97cd86d36a0104a5cda43a84b9f51e1c5becd1a9105475ce65499" },
                { "en-CA", "0dea482d9327eaf04c5d4c4496724924b56df81f62a68f52213a376de9ad2e851c8265bccbb9f8a1be63ee580289f32d6fffeed4054bf48522867c8cf40940a2" },
                { "en-GB", "d62510580cdffa7283cf5823b5e4fea05c2a71c1ac1d2e5f7ea7d58bbcd7545f55a29ddecb325e0ddf41dae7d7f48750e53a246581789ade6853ce6285cdc6b1" },
                { "en-US", "114a342a2118aa38c3313baceab90e1d60112520a735124608e06db217b0a72eb9c6d148b9cc220c9cf34b3320bc1f4e35aaf7a6c24dc7454c779cf20bc83f8a" },
                { "eo", "dffe1cef9a091ac87727b1edce326833c454fe7a1275ad0c82f2cd14542a23b8ac1af3bb60f591631a624607e63ce76696a0b8f75a5eedeb1b8d78c733dc7887" },
                { "es-AR", "93ff9726de94e3c5be61fea3f0d566fc7d444e69aea235597dc4d2236f00b6b3ecd2ee205bc1711ee510419d1023590bdee2c0606af60a43c27373cc5ca29825" },
                { "es-CL", "6ae7cf422cf734d4e4a5c5bd804aebe455d19d6066a08d5f4a6c7d0e69e402af593d13691e3556f9cc82857ff7f6f27768c73b8c56bf459063c4b6b8cf80e930" },
                { "es-ES", "af79da1b5f27bfe0e2996c9523f52eda52a76bc6c990171988278c4d78c1fb6128d86d6b07436bc3c8f21b853f8631cdc1312a44c514dcc357de3becb3af6703" },
                { "es-MX", "06949f677b860ab002d028ad53c6f70998eded87aa38188471d4ed33adb7c8eadf62435be5f9db49717f531e348bd8a96ed2e99680be203cb3a39ff68e7d2c32" },
                { "et", "29ed2615639fa86d6c9b1e8067f2a0c91eb24752391fc6f3d8b8c2cc907f23fde69fc55c71dc2ee9dd34b825754c2c71775067ec37dc1c490474b462e186066d" },
                { "eu", "0f40eaba128240d5af79bd35ff5fff84ebcb53bb50e8b05a90f5c5d69c7d48e5b6c12debe02515e1e0555558875677f813471c5e09183b67a947649fcd7b6d62" },
                { "fa", "610fb96f762e4aceddc7f6daa5e93735c4d3fbbffdcb95b44b576465e61911bdede01fa2bb9f71aa2ccaf80d9cf38872dea9c75024ded5f9c8fb9e84fd1fe59b" },
                { "ff", "d9a3c48db4425c3e83e0b023f2c639bd9d05500715a3faf143b59ea4ca7595fd7794bdae23750acbf14552edbcc5af9ea79a926131c31070ba127673bf56118f" },
                { "fi", "b27177953ec556d07f3f9081c2a526023dd986cc4b608e3e6b7b07da4c9a3d5367bbcdd1835bfd3085e21aea6cb624b6314f03e07c2825486bc3920a134ece4a" },
                { "fr", "216cfcc99bb5ec80b255ee9fc9558efe3c639666fb08a82f5a918941bdff4a8cba9555768cf501c1969c76bf80b9c98050d157a85d2086ffc2eb89a918127724" },
                { "fur", "a97def4a60295791a965ae91271fc1b15e0d1abc90e32b014f017407624010f9f702a9160f7a4a18e8cab743c6f38aae67de3df3b08d7a41058c873f716787a9" },
                { "fy-NL", "e5edad81b356b03ade945f061f25cc6befcaf0aed8e1b1bd1a1619140c5ca01f09590861a8601f8c2fa7b6e4d5e191652e6dc582bab174a11f24123abad15977" },
                { "ga-IE", "417d9b31f2261e1581a7a02f042e243ed3c8afaf2132fb9b141221fbf026995e5f9b902641008725934f10b7b0da3f3c166baffa7c5bf18d6113592d338b731f" },
                { "gd", "bfb3cd39d61d11ed0a2f449cb5dc0b9fa3b29c1e29ba488d21610c951528167ef8eca2ba5b46a15ad08a900bc629d82357e40570b03a99b4a0e00ae0ae0b995a" },
                { "gl", "11eaed28087abfcf0cb0df9b3fc07995ff46dbafb81928dc0ba6abc52368c78bb20b9cbef10a74d06cc71583aee662f3f0df65c0da57367cea6a4e5c1f991a69" },
                { "gn", "1fa30300c7ccb6526639923cd9e21906f265254dc23177bb1c58a9756b1fdf72a7f4a81fc5c7c8deb3f1888b0e4f830677887256456c74a7a102247e3f184568" },
                { "gu-IN", "35bb6fb345c29635d2d2f6a318a1d86be52d47332557a2bcc9afed35c29f29032ef02dec86c47d488b421c04a3b2a12eae04efcb87a27bc880b08c6d4d446211" },
                { "he", "6c1439d6c410f0c7266f59df7db3a897574df4425554f45964ea8187ccf6b545ec5ae87eeb2c54a5a9a59e7ad5160c21957e51a6ea148075c502b0b84f9d215c" },
                { "hi-IN", "4ecc3d21a3a82f1c218ab08858586f461f1cecb8014f84e8926f5dcf6eb83b1df5e763d71567381e225a373d1c5c61d3eb2cbf7d76467191c918a34b2ed9ae6f" },
                { "hr", "3e72f39a8297452cd93de57cadb487c25cd668a4c946a39b1b8f65a983def82a3bad2ef0c9d39205acfbc6cc7ade5bbfcb282184e21305a9dc3d3156c641d27e" },
                { "hsb", "83f66ba5717973d3ee70c10ab216f2fd32aefd7db2f01e13b230f41a6ddfffbe6f2afc9341ef8914bfed21498a0d33d2f25328055dedc2f81b3cda0b4a5f22d3" },
                { "hu", "8172be202cd1585eb2536020a8c01ebe9701b22a42d74718dcfb3afed3ec1a3c3397b4161fd75a64864d99aaab2cee8e7bb6413ba51afbd83c81b54608a70bbb" },
                { "hy-AM", "84cc9977eff9616f173f741d962c416a58e24ba34256f3d330072c5da8902dedae133c8438445fb608b8a72380e11b71d2acf9db1073b181735c7c08f8292a9f" },
                { "ia", "dc65fa6092bcf39c520cd762b24a1111ed751d226ea6022e1bed98ddfd048a88e94cd152c4c93bf3025cf11b05465905f77f5e8d3f12c3f7c236a0c0b34c5849" },
                { "id", "930709987349be2f02df7bac104aff80aa57d7b13ffa11febe6872de16aff397e8355eba5d0b74bdf5bd289e933d366d064040a64d4c3a4b7d16b80afc4fe517" },
                { "is", "8fb4c01209775c1f64eece2d8c12b4bfdb76c47a5edded20e96b574a70fcafcee9315410b0628c95acb3082117767a6e49bfd8f0cdbdd663640a15221879711a" },
                { "it", "5b3df18387993728be52af2b2bdd551df6c09e058137c93ddd9b1761008044a47d179b39d763621c014c2143c836af374d954dc991784d69214a6740729eb935" },
                { "ja", "baabf4d17217219dd12284a6771e5a503d6c2fff1c3a49743d5b42bdb405d6371232aa3acfdcd51a10dd84149a83b1b74850c3368f3d78d50f596d8ccccbc4e3" },
                { "ka", "ae3535c5f8146ed5fffb0b4b9ac25534b056c0c65360f47ad8d50319361b68cbddef5e49eaaf7aed139cd71c4353e7a710027379704341d6d0f486e7a54d9c58" },
                { "kab", "41c4468bdb2ad6e2b3ba0876937f1979271da2f92ffe5eb74ed4ea9b5c646fdf9599a9aba3e7aa23503f372014cd4f6c0739e2b553651b7147e183a61794eb06" },
                { "kk", "7ad1eab2efc7cc689de32cec772a4d14cfac2ff2966365e56518fa6d2a9e8954044ea4c91cc091c06685cf7343ef90e76ef6372f3a9d771b4f6f4eb6b8c744ac" },
                { "km", "4d3744b51578122ef40f98aa627209c8f099db0b962a7180643f1beac0ea8a4e695435bce00c8ff0ba4c5a1959c072d2a8e3b4690b614b543611e9972fc787d2" },
                { "kn", "60e4b319c541eddfbe49f20a22070bfa395f24ec64addeb4fadade2d12c1f3d174352efa520675386407a8970f0a88e54346dc5f51348b6a8cffb2db737ae7ea" },
                { "ko", "9707a85dec8b8b24e7a3083f56c205403c9a721cbdd93aeab79763b70e0cc27becb1e4215e37db689a42c448f0dac41f6e3adc6008092dfcac09451512e63f2a" },
                { "lij", "471315e7f4750781c31ab6b9cc50596244fc5c821fdd7c94a99b99f746ccb076d9a208886d555720d32eacadb7c4b154b4c3b3f8b1e09ab211f42983fbb830cb" },
                { "lt", "a666a5c774f2d1c1bcf0540d40e6aaa97d429a3b12629d8e5c8966d1e6d74bb574b67d85ed4e1d807e6008efa88aad5cff5169032880a77d60df1716e386535e" },
                { "lv", "b87fb3cb4b7fd99cb2dcca637c7eba46eef3ed9d8b6113e3790ef37df0c8bf2baa68871085ebe5a52372f11afd177e633a0cedaec38382bc4ee1730f0e770f30" },
                { "mk", "200496b106db0e77781ced8e99c42f24b302188ed3ff1799ee68739e41a01cf0598ef72dd8f0699cf74c9a62e539fa7bcc1e4018df5f790c031b7076ac89276b" },
                { "mr", "cea60e3890e897997868d818ad9ac54cbabe93dc8e0417b26bf750d91f34faa3663d4f6be0f433d92551fb30ba78c589dea424dcd3660c3940855df916216381" },
                { "ms", "ba30ff5921d6a358f08b94b141af08d93495eba5940e3752fdd38c04822db52df1d0fdde60eb4f8f5c4f0afcf9708704a6ec137c30e10348d3f7bde7dc53dfda" },
                { "my", "86c0aff8f212e311b4cc72c88ef7e07042c887ab04f29abec7dab42208411e8712e4e801fe01c355976d4abe4d44dab5f3453ea15b078826d2de48eccafd6c56" },
                { "nb-NO", "e537114f16d35345bed72333af4d71f7144af00509325706028942484e6c110404884122832ddd7725374fd0a54cf14ffddd755c6c5f1f658b370bbf48f5f72c" },
                { "ne-NP", "fb88a012c6391085204fe22b5e081519a5d1d0324ca1699927faedcbad62462c1f1c214af70113d310e004dd63374a3e54ae5aa3690f27e6581b1da96662396a" },
                { "nl", "de2c3359993ecef978eb69e3a717f3a14d419ead6fe4aa6b67481d32e4e6622291b556b05d9e4e7aef5491ce191f72f24dbcbe54b5a1ce52011d3fdcc0459c11" },
                { "nn-NO", "8d0c340bb3c5809beec0960847c6bd3d9e017ab5c04b9636a28fe44fe0edc2cba0c006be77dacb1b95372b7e8286ae2f05cf129a02151f76ee207ccbf98a77c9" },
                { "oc", "59f34240d263da8c816330745c93f689fa83f2fd6aafdb546452ec75331c30698202fd6b7b4d7c4a27af0e7fc0021a113f8c429847213c64a911286dc9d334db" },
                { "pa-IN", "7b798611cf60b52cc9a66f63e58f151546557ca858cd77c5ddf5482bdd17a94e1bad2bfbb2a18466567fd9211bd67f6f2df80339c78c063697bb5a9041b29f5f" },
                { "pl", "cd242a85c1e18fdc69704792547b2beac0e08fa9bf40b72e942b9447559dae855e5b092a70171bd3b8a5802cf9f9d93017fbbe518aec7ba90405be889dcc58eb" },
                { "pt-BR", "28f2780128977fa52650696ab9e6ff982ff32a2783e9cbba22bc060e232f2545b8967f8434031c00d02e4f396a0c57551fbc6fc401b9cf80e2dc02ffa930becf" },
                { "pt-PT", "964962a45ed023a79e680e162ae04d4bb6216c626f195b240c9f083a6b22c30928134352a59ae4a4e223584b196ca7f99a335ee5533e0232ae23093d1428ee95" },
                { "rm", "a624676ebe5506d39196a21dfb4ce7cc79f097d887a85ce646b3da6a6599f244773244a408c796ffa5ec8c97a3d37e59e9f2e78ddccc98f698c180e1e2bab98e" },
                { "ro", "dc1e16c5187bd77ab9327c5096b02e2d60171e0a871a17ee6178b6dfbbb359483e5c604378822a4a18f707c1496dc7fde201c78dfe41c52a4eab06941280f511" },
                { "ru", "133dc7e169c2a1986198f35106daeee32092d6ad801cf3dce0122c8ca2dc7046f9e5f10f6f7c9486c86ea574ee1dc89ce7bfc94596d3ff493b7712a350d2f28f" },
                { "sat", "9ef1c644607aa44b38a472742b965aa6950ade4e69ede87ecb2610692a46b02070359b08a262c1046818edf2476f4bedc485982deb51a4061f037c89d2737999" },
                { "sc", "5c5a9242edb84d249a7a24098698720ad2a55a63e2b65d578c3a7ce8e117582dcc7c3376506d36556c7ddc4584d7b3dacbb113643368860ede1cff4ebeda527a" },
                { "sco", "b0371f3368677b94802e5b0dc2914b499bbd84737f71e9b5b37fd65842e61142e3c4228cb6462836cf5f426e64ab2be02f631348e2175fe986e1ea055f756224" },
                { "si", "f150d97809a75af3d6003faca5fa80db1029097a46658b9232793cb06348ba534d0c4ece0dcf65e12e55dad623c8f12f562af0dbfc585dd89439d43eb471c958" },
                { "sk", "7fbace70a0b202f249a036d7213fdbbf31fc5f10d0de209712f438de6f76128b6299597f2f41c4b4748aad136588072218a049147e29a36db667aac31ef5b4c3" },
                { "skr", "56c234da50e144ea22a4a6b659ddc8eb5f1b230228fee3f86996dabdca005ca4c94513a917b962f53110edcf15ba1fe7a179b1473f94fb34165ed3342953e3a7" },
                { "sl", "c45835252ba98581fff387a2b6d6fd38925dbcc8c0c26c75b95d720d95bf7e4d34600cfe805ad0d940042ca207784ba3f15183511e229e4a434fe17fa29dba1e" },
                { "son", "bccb5144a23f845708d9a2516ab649c5e069f37a791543756b3436c50c1c7cf69f273b6d83be735493af2c4c34e8e3d9196346d419817c1eaf561c55bffb7421" },
                { "sq", "b091c57fc19af59ef7bc418d37230f96274e5aa96d1087bb821b7d08b5460e82ce659e996d1765d7ed9a8b94b2eae24f1129c00fe69fddb34d39df0b7147215b" },
                { "sr", "00c8e9ea94f4a4a3a0a3b24557c4c1c59d04fe2ceba954e05dcede16de5f816edbd6c3fb9c3de8a16593bdb8c4ae83a50e0f744dc1b96e2c61b6f3fe42a13660" },
                { "sv-SE", "613a2bb31a3e217f16587a4749e393072148fe6217fe69fd4810646bffb07d67812b71e06ff7097750a89526bac525651adcaedea15ab0dfcb7f29a247529d8f" },
                { "szl", "4bbaef4e33651c2b6bd8186130d7597eb68f3ca008f246633a18fb681f349a87ab8e1bee1fb568bac1c9751ce5d740f24d4887241aa8adb04222c79ff6c1120e" },
                { "ta", "97a0cc292c86833f1d9c9988fc62cefd9c88e88d9c3d2dced15191af1335409a1bb22ce34e350b35081e1eb388449dea442f6db5baac436bfef40e25d1dea826" },
                { "te", "9ad15594b7f08a858a5f91ac0828dc943172adc951dca6ccacd70ccda1dc263a995ca4b5c1899a63af9c5dbf9eaaed099da66960d7d5c55efc1e31706d9588e3" },
                { "tg", "fcc291c52d69a5ffd7a8914836faf0584b8297c29e6cb40b629e6a04f1c42acd1beb0fde1ee16b413fec4204dcd4911f57fb84581d34afb387ae170149c0ef68" },
                { "th", "17cd19be5180ef84fa952b5a8e496f52e3f968d5c9ddf9fdcf6e27acff250a4560dcd8e980d80cf7ee8041a575ed45b46a5b5a088b69e8e4147af4bf89a408ec" },
                { "tl", "708adce64241b92e417830cab951d4913e835f08c578ae0c8f780105c2d8f44082d88c1c5e32c0ccf616fdccd67e82466505995d1af1fe246e643fe75c51c106" },
                { "tr", "b4d3f35b07e58be98379e0fa486104e473ef7fb7ac132603b1fa7761770f89f5855c6c9a50f35a16aa5c6d24dce1afac26fba30125e5425281444927a98f1683" },
                { "trs", "9eeb9727157b96565e7ee50a7c14bedf50e79a5ab8eb1eaa75eac30f79db2f48ffc22a70c2edeeb8307a3319cc4ee294fc3c5b47cb3f01fbcd06bf35b4448c60" },
                { "uk", "711ecb61368b03b2c4f4980d67bc51da99ad06f8a2b8c1f8b45d0c080779def9e2979beb6f26559e2bb270c3bd538833ea896c788c95a2f26b71162995361417" },
                { "ur", "976d9c15749b970d47cfce38f748de90db3dabed9cafae39f3b01b49b764e9ec2a119d43c4c384b9e865cf9d5fb2120ecd87dd60609e183f25c4159a57e7b61a" },
                { "uz", "525a6e28005cf5b026eebfae9750dbb54cf44add3776f3a773b90fa49a51d54859623f46b42d955535d4073871df756527825881c06c5ab466851ecfd7027bf0" },
                { "vi", "fb5d26ed26eefc3acfe713d22ce0dd64ddab8c3bc4794be3180db40c10e504fbdfba359d1ef0722bae98385ef8fee4b48687630d642e97007fb91de5bd91cbaa" },
                { "xh", "f4dce88b4d1a034190da06399f3348c64151153621d4e60902dfbf87e9bc77dee5e2d28fc29a501e818bb1a23d7b9d0c61cf6dd57d3ff1cb9efe0dca2790e807" },
                { "zh-CN", "63b33808ad2cad24bcf25a62b864e6b8a98e18656c041b9bd27c02e659b607d21464ec26675ec9f26f86b081068e376477bcf3fa0d8016ca6d6263863c7d8292" },
                { "zh-TW", "f5e9556d4f115037ad9fada09c59fb0d23cde308d71aa7f9d97eb483feeab919baf4e67265e582f022dd65117fa520d66afda8690b20feff3f568c910d693ef2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/128.0.2/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "1e529ca9456c7ec7072d2e97ef71dcd521e58f3dff744e764f33acf15d97bc6e859f0ddf42d563097c925c025a404d5e1c3bda6e9e463ad2999b5b2aefa4feb7" },
                { "af", "1049ca499296383ef5ffee81760f7dd5f6de23b9bde4779e2aa36068d0cfe1486f6d2ceee32ff6ef9f1ae253103384a7145ba085ea1774be17a04704ff91a39b" },
                { "an", "5014c77bfeddf85fb0534008a5a9b16e045484b6008e29b5202ef6a9797dd0b8418a8082a136da4e10908f719d38379f69e9c86b8b56bca1c9d6aa2954b1dac5" },
                { "ar", "d243e17b00b48bc09bf483650a3426339a66281a73e6f2b00e9c596c6c76cd4e1270c1cb417f5632f1e3fcfe1df4277a509c6dfd5cda9c1c712e6ab11ace3f82" },
                { "ast", "ba7d1e4d330fd70126ed538a6c1bfbb3efc293d9059eb7a4012d02cca62e84f4dac6e21692da3c8e149b87dd6cd0b569733bcd9920e24f45599eee42a412dd53" },
                { "az", "f29728a04c82388cd0a2b1c227e8c624f535a4ed9914c92ea11f0b42ba579a3467035fc4108b461452600b5e6d43f48521f3eeea983003c30de262e0dd955911" },
                { "be", "3b3f88d6b104b588584138845cd9842cdc3fb151e0d3c611b772e3d135a643235acffa21dcaf1f2579f17f513f59af527e3ece9670e2e5f245f11b484ce2dc08" },
                { "bg", "726c5243191c7e10c4d399f42a83c52aae4a53c0464621a800c3c8a27f8cf041e9b00a977c4f5b451471196774cf23de76907bb42335051642379edd1b75e151" },
                { "bn", "4fad083e961e233ff9804f14ebfcca90898a5fffceb5d15bf4ac27aef4b31c61662cfebfb3e2fdb0c02df3119bf5a55606c669cc7b3579259c3d235c42f58bf7" },
                { "br", "1d3669011444553d7b2f2400a9cb80871847ab8c9133cd0973f838d404e3bfdae338663099c659d4bb239725724ef9ff0c1562e4afd480f7ff8cf22717b28c6a" },
                { "bs", "c0e69e99b84cd8fab08f983a6750921cf9805add4af1d656b0351fa48afcbaec6c6edd91b799d86d18010b398c149cbc909aa2046bf747111fd6e4c5c41d7bab" },
                { "ca", "bd5410ecbe598fa0c3cff960aaf254efae2421491a0dd4110febd1ec25f68308f404d32d1e66f17d4c07a5cbd584fc7c58eb456c0cc7af14095be6851ea05988" },
                { "cak", "c8871e6f79f65a98d51d3c0bfa61c3f60eeb84de5a2425b4000139698a25adb4a4b42ccd6ea055f98c6075f43b9fd987d3464a4ee5211355865d554533b0d6ea" },
                { "cs", "f7a1cf50daac01fbececb520eb9e1487689c72c08ce88b8394b5fa0aa73cd1906e3748e8c8483cade946825994f3aa91b77bda39535a0e16e1cf2f5a73b091e2" },
                { "cy", "99e3e1c333e74c8bd9bb59ec95e3c2531eefa4b465a5d8b6329f3e945aefa73f4ecaa9073ef04715689c0e5fba3a586848e3b2d9156aa9f7e73ba9de5014f1d0" },
                { "da", "73e707193f6ae19995a3e242e380e1c37982dc80a5aaef208f3b1fa6e344d53eab0b9a6324d578a50ba3489cfacbbc128436064487e93b0870f0ab31ae35ee84" },
                { "de", "d8b62b8284ed7a6a76d39f0036b8850c55c37bcc1895472113b29ef3dc1f7b8e4a3ba5f0da8572182619e40cd3d32cd00fdd6abf7922e293f428ba204b66ac51" },
                { "dsb", "9249fe569df2c4ec5c97a11c9ece02150bb8cacea6466ccb712bdbe12a9c21da47ed3b637562626f96e401d3a6ed9d00a3253304bb085f0a1528a6da2216ca1b" },
                { "el", "a1cfef4b50e3505b71958d22a1bcd94a4147f07e5e99ba4d5699ba016f6161ba2238fca6b2f81b109ba7e61f9aaae0c500ecabf52a3666f3f4deae54636b607f" },
                { "en-CA", "d031ce066a206d81e1d69822f539c836164b6fbf08a98896cb13e1ab19a765580c4a99ae0ae66c8d7be910d3f7168eb07539388c60d43e209db952c89b458754" },
                { "en-GB", "af37fb60e6c0fe90684194b13387d15359a07e570a9f34c8e9e2efd64d1544d01543f728431b7dcc7d39c985e159754192a475356c8bc1b0a305224cc3565f38" },
                { "en-US", "136444a56948f3d12753850abb571dd1bc6859957d4257145519dea482565f916ed7308bb66b4cd5634794ba90c033e7c7eafb1caba2cc9835859b8a5ee3ee46" },
                { "eo", "1962dbe1545e5ec990537ff439de35656f4466f2b1a08164dbe028afe6df2e6464601fde1a401ed060430506d12aa40ac8b1ece91b3fe65b78b1ff0cf93f047b" },
                { "es-AR", "2e0ddc9ebd7bd37c42a982d8ea3449c1897faa4badff5c5ecee6d0964174fbb013a463fbf85d03209ab13b251fd815089a84726da1b4a5c8f657a21db54a27c3" },
                { "es-CL", "3f3e4116a4481f2f9be0861f4aabca7e2a15d8ef419912737a57025f354e70b2dd5da2444afca6bc9903bcabdc9d498afddcf4a9bc44601573e18af158590bf6" },
                { "es-ES", "3cb7b7284f15f530f7cf969db81f51990bca9b82361ea9484c40ee9ad47d10ac28ddecebb71ea635bf0fc82a06a2255e56c7c3ec39d0334f1eaeb6bcbdb5a906" },
                { "es-MX", "b98a9233ef4f175c4e8e1f21d1cdccd6d2a448cd6a42c594ddf9f0519ccfde5962230907966ba915a40dedc57d0a5dbbc51746a8618dda85eb785b17d64d2c42" },
                { "et", "cbccc26bf610b171b9cb92bbe33a8562e69809488937172b0cdc514c858c19fc0c44ae32cd5551dbbe7935d46bc046cf47b8a06f192817cfe8224341b83cc761" },
                { "eu", "1ad777e6e785ffdd689649926081fa58aebd0b3a9e91a456f311184fd0e119bf29796426b1a9aaf2662f5dae1a651e914d63b79b39c1cafa9313c401cab28c09" },
                { "fa", "efca16520fb16cf35706673c60c48d31d6171a3e687b0ee03929cf87c0799f70887c3565ebfbdb7c8b810bad3317417826d66f6e2172e15d10a73c8e8a53f093" },
                { "ff", "eafdb0dabf823e24aa4ac7802e860851789cf10c5469aa29ffc2e407052e50ba3c898524f2c8558eeec5fefb704be66a62f5eb8b6eb9090600efd4bb6c4b9081" },
                { "fi", "3ba26a19b4f3cdaea1967011e04b78d72918eb491174ef73a5f448bc5b53e6800769b015275de81a570a164a086cf7b16264bb2a7495b46daf8845ac27b76a35" },
                { "fr", "b68834f13210cacd7777cfe2c866fa61eb2aaa15a8a2a4d26eaf525cb09125235b0586b45523308cf1c86f835ceae2b831085a723dd6fe0680eef54aa4601e87" },
                { "fur", "913335b403814c5762935c9018fffe338cf8a5d94af2c1700ba1904b39edc7d0f17d6d284178a91075707063308fbf664d3cacdcd70f953a26a3651fa43b9a92" },
                { "fy-NL", "eff519c75b3a535c67dc8366b8041d12a6a74df2e149effc432dce495ca39113166b29fea261e93e0cd172a17c0977c6c9ab0803c3c96ee8252c17b46e66355e" },
                { "ga-IE", "9484e2e7635425835fb1111f4ed81a69160148ce509331c0a2ba378977fff186c5011b846cfcf939521ea3f29dab5d17a481c7c70ddac2d00430f2b16685549b" },
                { "gd", "7e3e4b59bf21e30df4a2361c304583b2d07959a957cd977be0f6200cdf66470aabab3896f9e9ecf951653e74b2bc715129900098556655a1e4839c2b6e2e83f4" },
                { "gl", "7d17b4eb2b2330a617d116e845d3601d1c83476d527fef0f13153f07f78a894902bac6e1c2ddd36e82184ba9b3cf0936ead41baf9844a15e0dcf593745adc4e7" },
                { "gn", "0409627631fc03af15faaf0dcf566a1a72a0f561e381aa7382e35a9fd6a51d45d08e58c137dfe198ab47c5fd7208fae8498a3d9ad466acda5ffa41ea356073d8" },
                { "gu-IN", "259e38ca174a6f9c41fd33d6cf63a37292b71d74052d1897a21dc52d107bec9ed2963bd9433f2227e75388a27672a1d1d5c2f02176a377f7f3cd823f7193b6dd" },
                { "he", "2a90f32b1e7c85ccfeac1c45b926e002d1d9dbb30519b52a2e6a50687287fade7c7ccaf657c08d386724700daba9b38b914c9a8f15d322e8d775880507b204a2" },
                { "hi-IN", "00032a11578fc018ddc188bd50b38482fcdc12283c26ebe74abad22430f933e30207948758593a1ce00b384255664bc4e9e38ae615176130b8eff2968318583e" },
                { "hr", "f6693b5b1cd97a3b2963cc68fe9ac87eb555cdb2cf7b26e353a9ebeebeea623cd981330a70dfe23d3ecbb18050ca3ca496d569c89093460b25f78ab117fd0103" },
                { "hsb", "583cdee3992984323387e699b34f5f6662af97cf49e5f4bab1b677e6616bff0448acdeb47dce70650481496107c6f46bc003a619a5e0444317bcb929b826367d" },
                { "hu", "94bc1e4e6a01d88d48cdf912d4c89bc7f276dae41313f37258f4dcccaa1c24bc9086983832c172c9dbbdc72b74a6153d6ec02d1412f6ed3bbaace0e13d2c53ca" },
                { "hy-AM", "3823f6889e19ee9f2807073890933a867fd4953c5b03774f5cc078c0276ab41daa923e802d91e2f1fa8aff0797d72307192ccf74c5642bd70df3acf14c109938" },
                { "ia", "e094f9ad412e1ec09c9e1aed02b7f8a1dd20c1bf7b4d5e01cafcfd9b81826d1fd7410b84fc838a1cefa130c7271cba534e49ea2a3e13f6677b78fce9c32c88c1" },
                { "id", "228c0a99c2f7c614ad2fad4ed7bb25c007d1c9723f35f66a851d95bfccee1e6a7de222edc1ac1d5ad928be9a308576d881625a4420575fdcc03b2ad6fa8311ad" },
                { "is", "35d0d0168383f7650f4334090a2c17d23b89676b1e74b654bd7d1bb21d0de2ca79e951de7f020017eba3660678e2915b565c36ab5a0b893a2476ba54fdb65e55" },
                { "it", "ed02db4b514e30f7f4cd40615794064f2908418cb86f9cc3a52580d0a659b879aa42bc3215992f4a5cf829481d21e0435da507f9006151a136202449419a8ed1" },
                { "ja", "faab8a829972daecef3c8d225e6d6693b33c98b67450269daab10a594f5388599be9a5daae32d0f61f4d56f07d8151a0d75b4a18e2c3da5022fc0379c720ff8f" },
                { "ka", "6a870022e20c57dae49f58e24cf170c526ca1df00695894d9e47053a77f1103ba7b0c53486b4e6ddd233f81451b9ad414e63b417207c3c2e9920795c0cf19d47" },
                { "kab", "e1029bb9124cc8bf9b6b6ccfc469a654a63b2506cef209367e754d4483de4e41bc323b4e72544c5c91722042e57088285ff1d8a63b642b9a630ff785cb3e557c" },
                { "kk", "e91b69071701dd11162290ac6c1c7381fb7c00e1f6cc0fa0f0eafa67f4b6c375e26a9e1fd0fd5b88bc0208d7e195a240bd351235f5b06b31b340132fb371da03" },
                { "km", "b5f51084bbffee203013185cdf429fa4c2fe22fd9c67035722ce3fb049d4a86efa297784a8334415efc0e1df783d18fdd62fb4a2e7aeaf0e50789d1774156fa3" },
                { "kn", "e757de5ffd24f87e68f79d5b078f8eb3ac1e2912b76d143997009d058ef1b8c8d9b2752faebc5ddbb2500751e9514d20bc333dc6cc8125722b02f90b24d5b513" },
                { "ko", "1ebc8bffa81e48d7fab0741540f089ba12b312c46aa2ab86f85f5d554827100b9725841d426fc28891f7dc91eb5c349ddae6a665d3f607bfec89a7e64dc8beb1" },
                { "lij", "755266e73f5711f67c2e129cad2db419a1c4cb82f39c9f7004c2fa9959c4f7388a83872386cffddd3ed76a05162522248e84bbf4fbf2882f6a23f31aae834fb5" },
                { "lt", "c02fc3cbe79dc5ecab61a6b70a162f4a8736d37d4dd7dce801d7b52a754e8af86fd93fba4c7c1536f4f2e3083b1ba3b4d62991ca1bc0cb09c827ffe66c73d02c" },
                { "lv", "33f54fba5429c1427b15c41bfd0a828918590cc485f014eda1bc27ddb78158e7741335148637a58d57116cac87502311e964c470365c4dd05ceb919cc50706ef" },
                { "mk", "d70f727e8f632905acf3ddb9c68041b948506c44ab3970655b2f2d9f8dfad4016a9f85552fbd4b4923c764d58c742655deb704d9762855449d251af3f36a6a0e" },
                { "mr", "995161c936f9b316bf3f412d1404800d74a80c9ab449de474b6020ebe11ada98d011f8785d56cf92ba9522be253095afc7be57d89c3de8f3a59bde43d16de364" },
                { "ms", "5f32bc7c607f793e52c293fa2f16826e09a54557b7b409cbb64eca29e24ff74c3c5a1358a41163e27898b7456020c5d09ac0656294ed18056a393b810e4a0084" },
                { "my", "b4ec81f073876add70f016511b70c106bb9d576e70c314440a8f80c5a8cee9c85854ec4f6048dc4d639ab995bcc53caf8940ff207f54098fceef4cfd684f73ac" },
                { "nb-NO", "9115a9027093f1252c942ac7feb3ccac405c5478299c1649dd28a61af14efbaf5027be904bdeefd5a5590d162c9c9854acb3efc4566a7809594bd4e98c801925" },
                { "ne-NP", "69ee6639a86ec198d8eb456c6e63c598b2298f4df7fe159da732dcd237a4e443ded4da9f97dd61b85d167f33a92fa446d7198f1b661444dddeabfd3b7fff4d01" },
                { "nl", "ae1a8bba271fc18ad76226bfeea188411f41fcbbbef1d7ba3a24c639ecebaae311393cdc92cab6e82a49e3b800c37971f3ce06eb11cff54df087c070e84c701c" },
                { "nn-NO", "1c8635d6e4007b91eef7cc1fe317f8e2380355ab00137e65c8a8fa1f6b5fc8f76a631faa37b86f7edb6bc7056b9e0d43fd904740074f94e268884c1125dba678" },
                { "oc", "d34f4317696074ed71effc15d1df11ab358db6e227aebff0c83f57eed518395503a2fbc75691e04f78d06491d4e3ff02f1ffe51675ce72427561768b1535b483" },
                { "pa-IN", "4f27e97a7891b2180008227ae3d1e8bbec3ad5d23ba53e86d3835373cfe3059763e0c18737573627629f20b446f6399952f69fe554fa40b05561a2cc7eb8bfca" },
                { "pl", "da24e79e662f8e6997a5fe34acef963aa14034e879f9f18ae85964ecf8c6676342d4163fc366fdf6605a1a0321a244e40f0d011c2a760d0a7c87c57563b937b9" },
                { "pt-BR", "7b252cc53ae22fd416928c70fd52b8da31b71ba78e0bece1add169ea4aa981947bb19009bdf1a1b194730580faf4a5570d52cf779807007382840510674d2930" },
                { "pt-PT", "82d9728c3851f6748bb950e0b7c4a9d179c89fdd94c60e5b5abdf7a68302abf846aa6ffe967b62cee0027499bd02f49ee79d053a30e886e75bf04bdb5a45e6bd" },
                { "rm", "73a312a926844ac5d53356f30375ff55603c8f910d597a2a85180b8c6b3b2289b46039e861db8930779a70613ebdf581c138a2ee8aecec249391cd7b1632f495" },
                { "ro", "256b07da8816b328bfdbee93f150dbdfcd541452cde189f3e789150704f7b1de57e180ff69e988936cbb890642db778628c3b8ae833ba764f3f2ceb188288335" },
                { "ru", "626cfdf81c8a9fe3859e763cdadfad8a5502d618cc1f47b7a1254fc46720e528b24ed6d31d6d42c0a3b452cfc6eeffd519b48231f376f7743914940e99a2edab" },
                { "sat", "25a3f943d43a8eb8657082bcdb490afb5ca91b757b1f73ba98e431ff4ad41259fc7f08a9ee55b1d6709147cdd49609ea20c6fbc6854da213fd5e6ba98cd3e4d7" },
                { "sc", "aba6b64e905df492eec4ee61d48a6f3f9f3bb7e15d6aa211ba686241f8feae877e2d7b7af55617cf3c8b53fed365c5bff90e28be56d5a56c89b21302710d3e77" },
                { "sco", "690bee6455351a3668e8e44c96bc13ef8b641834ad0260f8b1433e1f34ea4ef5953726817c08d5ec1bfebc52a3d4d33b226bea9cfe368796ff6dd2255ab3d4a7" },
                { "si", "411412d6005cbcf473fd363beb7491436603085c7ad3fc27b4b2419a16e8181584a194768c5bff366408c9df242ae444330529aa8470cb456dd5ac7f9a860d2f" },
                { "sk", "8d96f31609401e54affa05f8317a4de6123ad00fbd53ac349b5b8aac8709e869f8ebf401d1e2b33769d153dc8d38f82f16edffb6c63f3f39a12b47bbc2fa5e59" },
                { "skr", "07a39836ba34daa1654b0ed4ad2a2b5df710dd8cb53a2bc99cce909cc86b4f84596708f105804b48e10caa4ab7630924e874651b64cb0e6c2357fe9625b97b81" },
                { "sl", "32318fd548517d7cfe98645bd40af54e276b95c2c3c2615167cac71b875d70c3c82268778b43c024b437f0aebc8a820241d433672c5ae0bb85377b28506bbdb3" },
                { "son", "bc679dfdb6534af80839bb0a4ae2171b8289d26a0c32abc5f35d4197b5bc65dad994dc7aebf2008d2d98431b3aa00b328f42f921743f1afb9a2add99c4423554" },
                { "sq", "d29c41aab82f3d2b4c87ba4f3fe97ad0dba854691e031c9c6ef56f30f0a601a9718a741f556fc43c728c147c5d03290a44e802265eeb4934d62bf67023594549" },
                { "sr", "a33d77cbc6c6421248833dde0b9cf27a46e596ee6545b7fa95b93fa8ca962a55ad9c0511e1db0f27353a76907e4c3f3dbbc2c36f3153ef1ceef66d7e97d92c13" },
                { "sv-SE", "d0af90d68a44eac092f1c949ed381f88f7ce081e22db8d14b4d89c774ddd616eb17b763db9df68ca9a6fa46d9307a338e6e7d0537dcd8e1f51e7254515a38e4d" },
                { "szl", "2505c0e1123b9af2aa1c7a75b7d9c1971657d9382f211db4921e1cf052cf953f52e2e9df8482a018f0990c1830399d424fb4517c52d40098fa2f2e583dfe4a63" },
                { "ta", "42ccc1f46a096fe0aa580967c109a262d287a6cf24f977927c9cddf37bb307a3a7da6352b83a163bdb10694927fe6a1ef542476f8e2cae642df489a3fa620873" },
                { "te", "3158bc4588c1e4b85f93479e9c97015fccf1e664b72ac39243068eb6f341c845eb1c83a1955a6da03a964b35d1853a6f99072fd75269addd3ac5db69c0f9470b" },
                { "tg", "206b0e7601c75003f1ffd889dfe453fcc82a116cf83bf1b188bd1b8468b3b05f1da8024ab177df8c8bd9568f9b3d24675c6da20c160a87ced01556ed5fb09fc9" },
                { "th", "5f5f121cd7a5c853a749fbf831d549095ef715405a648ad7e1e786f7a504a68a723a0b3a160c60390f67be71ac50b70dce6ad3ce413626fe73bd5e4f9e534524" },
                { "tl", "da77bece232bf0d68c5b42d149e52a785b020d19be7884ca606a66c69ad8ea6a4363c3e954400978fc3d94f1d7a2fda5b10b3cf5984db17c5ae5985bf3b6f149" },
                { "tr", "bef4e11c42b2d8717570ac60e4112a9da8f8c87cdd982a567535c0efee5662ca8d778aa751ab189c4b9fff32ef6bc0cf31980160ee555cefd1aad160ac7be09c" },
                { "trs", "eff577708431cc536dd590a51d133c429a24fc38ebbca3a498f67286b152944d740b5a0bbaff9e0c7c36eae2ae0f35625b295c1009594e5bc7f05cc8cf844bc7" },
                { "uk", "80be8a34dd31c298df9652f5f5fae4e1b5d817d1e153679cc68667aa3be650a4ebcc52aee349720d660698512ef06c47a00aeb2f9e0d3653479654b3df8f84ee" },
                { "ur", "b26f91bbd454b552f8fc3bc694c18e01649107a57333b890e4094cdb194f6f45cef68e8d1925691889a0297b238b05c588781984f84264797f57fc0b69875110" },
                { "uz", "a4d05d2c56cf3156df4bb1898697e9999d84886b46aac7533a594be26c41ab90c5e9accc1604a9d2fee0b5340d400ccb1028454e9c3d5f717fde587363d0d25c" },
                { "vi", "2375b4f13b272c0663fe65537726c15ceadcd651573cc4dc9e1278e90c0ebf47a3a1218d69a873f139a291385eaa9a2e7c1f4849f424e71a5f81989b90b55b7a" },
                { "xh", "4d00e587a101fb0200105aed4498e36089a6aefe5823ba868e57c5d15b8ca83f8827bbf81e84d3a91b5d1e949f3a7cdb04217fd1d8237709377e09435b032a7b" },
                { "zh-CN", "0127f83e5ebd688ab10b4bb7e2d16dff1beff1cd007d07ffcf47de09d80ee249cee9e43a0b563562ecb2d894fc37f6a79cc889194727476118095e3ac61c0154" },
                { "zh-TW", "dcdf0c00886eaec91e225e5acd884b4338be0e9bc58dfee172fe88ad567ac2f8e3820f99131e3f0b48f4aa20e48ca44c6dfa2dbf22a04780f169d05bd6c1c525" }
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
            const string knownVersion = "128.0.2";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32-bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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

            // look for line with the correct language code and version for 32-bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64-bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
            logger.Info("Searching for newer version of Firefox...");
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
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
