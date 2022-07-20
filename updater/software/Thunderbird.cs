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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.0.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "6d27e2d17dbac6948bd477ada0f271b1c33bed64a7fb630735ab329e97fab1e2e1d50790a7fa286093e0702e14987682ada501c6bace34c7fca84d5fe9f2413e" },
                { "ar", "3ec323061ae205fcc77172169bd7b10ccc2f5c58e0d7d926a6e848b42024162efd0c59d3be3a35df0e01d035df3d7154a2780321c5ee5a62e7b56eca57ad8691" },
                { "ast", "7b335ae4cba7456e9f68b2af70f10890fb15b4f278f3c55acee98fd34669bf8f31f3f00a5e466909b678cd96a1d2ac8fea4f4fcca523a472015c277f2d73a70c" },
                { "be", "0b5e30636046c4fa06ebf5af4ba4cc4f4b4062023375607cc4d95bdf3bafdd6bc1e8737f8db5f02ce8f8c6842cfaa1e2be1b64d9e7bf3c83a37a0da3826ff92a" },
                { "bg", "ce3daba38eeff3254d218807e3f2ec210a7798bc90f3621e3a524e7434af8fd0b3ddb6c17c7bc3a8a4a7bd88a9f0b60bdee571c2ab6807446ec333208f07a9b8" },
                { "br", "13fddbd5eb316437cfc265341a29beedf6973542e0748a11e602b645ba4b3148d04296d94ea763689962de839ad20917ba6f3eaecb240a27dc32faac87e4b48e" },
                { "ca", "21d4c9d414c1494cb9842d134ccfb200d7e107a41bf1fa4245a5886fc1f60bbb081ef71931e57e4a6a8a870b5d8d9394f1b6f031c7578b1209f6ea121fe90b7b" },
                { "cak", "f3cbf7ad0ad76d8bd74a9c937fd6c7b6def61eab9635bd7f286d804c1be8ec5ac53c716d03d9c73fd4558749f2e4c6896707f351dfdca7c726153417cd10a7dd" },
                { "cs", "681ba1aa0a1eebdd0a5684c7323172ae52bbb0cb7111c05122ae9316fab497bb56f9a2458eb8ea0646d192bd0985440dd3acfb0dfcb1f62303ce4b63f5d6cfcf" },
                { "cy", "ec819034d818c9251c950395c2b2c5a035dbf386d6830aa73a120c34134c1f9afade28893c7acb923744dff5c130a8679dcf854996a2905e75514cce938f3d50" },
                { "da", "c871757368952cd3304b98f32fbffa4b61205361a03fe2b750b49df7712e3a2b915c71012e634fac57d0b10ab4539708c47b1735aa7bd621a2f34c879f2fbf7f" },
                { "de", "32a87c928b33c962e3d63795baf43d670c3cfe966d70dee4277f9abebf48e390e4b79554c549d7f235e5f686df153c15159e2ae6fe4cdaa7cbb9679249c82c73" },
                { "dsb", "4f313ad6d25db5213a9c329a8af1d410b937c1a1422b3873443eb4431ff6e0e81255ec267622099725acab99efc481f3d9382a4c602d6419baec62808cf0e32b" },
                { "el", "0542f291b24d9da718460743367fb2e7dd3bd7a0ea43ec202469d95a541f1f0fbea2227e8acaf48ff3c367ac91ae6a6aec1070008add1b4f1c28feb2afddd31f" },
                { "en-CA", "92b99c401c582f2e0c5ca87be7c560a1cfab4302b0886c1260cbf82df497eceb96b99a0cc021e404f60e58da23c249b5d08aaa140dadf0db49a333caf0edf332" },
                { "en-GB", "96054241cfc889191ed29700b225b38a2e1e70867cdcfe5d828e47f3e12dfd45aa7bc1cde51300817d434c3c60d50acbf2f19b2fbed32ae2de626d4c85ab1ed5" },
                { "en-US", "ac109b28a271f27903cf1c37d75961fe2062eb56e5200d45c38c947ab0a9a40fe792d28624283eaf9f56bb86cd32a382ee08440dd9f3b891520cd4db266cf500" },
                { "es-AR", "caf305b465a69036455d4fca105523017eed0f44ea279c2d79fe5bbd5b54aa46945891571bdf136b40fe30c784a1f7be54503b1cee802591b90184afc0368f6f" },
                { "es-ES", "a7df7ca6b4441064e1e171222e47ce27c752814d58a8bee148e86edfc5eeb8f6575eaadd71dc1b2dc21d3ab3fe653eb6ae53a28f2d36f728106d3e3eb512b69b" },
                { "es-MX", "1047300515bea31f34d450a61269e4e579380121271d7117493e5f3cddb999e325e0eb1e4a50f79cce9d30aa3e4bd25cd10ed65424d6373e4dd81066d4c9753c" },
                { "et", "720b2af36860fe93a053063c3c4739ab4c301f444d969dd8b06cb3899fa97366c613cd9279dd22f674f5a2c6a699c3fc4c9bd9bae0c098eaa7e99d8d23b70390" },
                { "eu", "dee7badffdaa7f2e6f344adeac303083c33f857f29de2d52161279864c6f3521da88722fc47a7be0f87d23452fee87d228b0c0e020162190d3ed0653933e3b56" },
                { "fi", "9e795231ce4c00a9ccf5a8ffc7c14722c24e365be9eb85ea1d2a605742af2811d7bb641057c9c13d41fc019de8cf9bc2ddcf95df4b095757bcaac22d18ff9ad4" },
                { "fr", "d66d2b6bc39020f75397ad5c6b1c4f5df98a4d7516447a42baff6475918cad98436f042212859fa6bf9ee5cb0b86c0fd37c1cbd184d0676120bbd6afd2cdf9ef" },
                { "fy-NL", "46d2a7190dd4a0762e149f7b014732e574b78eac26ad95768cbd92622ffa33b91757946e88e9907b40a90d4b89d4da341698cb3d35ffc1aebfcbf781e0c0f648" },
                { "ga-IE", "8b527b262cbee60faf747697185802ceb5228e23c361eb78737fd9764140f676314bd03c55379ca3b4de4576396e4fcc29041024878e689ef8d3d10483fc3040" },
                { "gd", "b8ce857b2d39a60859b8abe0f75a818477b4716729e53aa890f566ec1243c20ca429c91aa134f05af3e05bde6e074a5451b0d3ebf0feb43e76ed07a55fb72546" },
                { "gl", "4a7b7d2e0a028f12c3f1e0e7f1625aea5fd1f9d44149a5b3c36c8f734345f1914480ff3f8c44ea3c9ba1473f59f9b1d5935a98ce37cff53a3fee94df6877ff23" },
                { "he", "e88f4e519355eccf0c935d78ec7156f08824525a4b56913afab4198d05943add4a5ef8a08140bafb4d87fb4e977df28098053f76c6835cd8ed1f23e9732a06d6" },
                { "hr", "60b8c09e2d2dcc745a2a4cb4cfe7d6a32ae53f66090cbfa3516ed27c900989f58594e287104a9d6cb3a5e38cf886ecc6bcad2bb8f43c757f79ca5c944ef145bc" },
                { "hsb", "476e7a097149dc4adfbbc83312cf137dc99d2ce4db746fbff725a53b5e76bafc9663a7e789255a8fb0f595c1921f2d6d0c7a0deb1cb8aff0ff396421a05794e7" },
                { "hu", "4449101c0097be542adc23163de4651d2aad5d8f6667fc2613f66181eb4ded9ad867ede7c3aea3523084f02305a8f8a809afe20850e8dcdcd0dad89af522b617" },
                { "hy-AM", "e7cb30cdec9a413d3690ad6cbdf00d007d25caeb1974038bfd9e5b65ca776a5fcdcdd6be3f9400dc2962a233650ab349296b9c6b78a4ea0fffa3b4fe8256f771" },
                { "id", "8145251a2b7699b5f33b81a0615dd93711c2aa766558f9d0508249e82adb5541fbbce27d5759bedccefe24b92c456e87b99d9d5b08da03eab58c761ba472eeef" },
                { "is", "285345f32e0dc8a4d94505e97397759a9689ebfe9f04c9e44dc12bf322d076373e40400c74e3962162cf3fe868bc0a5ca00708769f512df27e413409ccb7cd14" },
                { "it", "f1736aa78f4293a0d8c6ecf9d8136bca4ff4cfee2ee239024b7ae10eb7a07db37be24294124bbd69ec5e9a78b972747875ea3753ed555a41833651b306494162" },
                { "ja", "d36959364ca6fcdad8a020401e1f894b42dbb32627c51d3c7885fe2fd0068059fb4239401fe74db14611274e23b050b31272880c95091890e0da0acd34ec1a2e" },
                { "ka", "2f3160dae88dce2ccb16ffc5361dc1c996d0898d39e105d333ffb06468fc79b866c4fb5d9c4a307470e2b0e06d63b9c4ccca2c8b23978a5d7b5c0440368b50b2" },
                { "kab", "3087d8487e640fe2f679db035801b5060cd1f92f3f04a439c47e96da6d851e7bd183ee5062c1ab85b02f41f7e7b1098cb680a57987a65ef80beaedf2f5418ef9" },
                { "kk", "57f9ae4b7d9f44217e56a147634c005ccfd7f26b30215322ec9ab91a6c863201e753d3edddf5a7e5c6913dd1c1bd04093302b846945339473695faadfa889f47" },
                { "ko", "9236b0c74700635db1ac718071937e3a42be16ae4831b2b95934497ba5901c17ee7f69be3c1f0329f95381e99369ca8931f41e9e881e4a878f7045368fcb90be" },
                { "lt", "6b5810ce29d5407b5ba136b2f6c6412d78f43736931546e83f054b2de9a407f268e5e435c8ae20326406eb0ad218b93fea9a658af54cbe663d44819a462d3034" },
                { "lv", "5d371562c79eb8a9eb2a31fb8f024ceb27d41f4d6fc6268fa2b77015db58f9c93615ca700b59cfa088fb30ac84955d34441a38b9042360591f72d5fa83de3146" },
                { "ms", "404d85705e848d7feb0dc26288901e799163cb8ea8254a04f53d541a737e85a890291b53b562cbb8ef4670fbb8ab09aa8da82d821cf3481af5caccaf5a992303" },
                { "nb-NO", "b1d44822c753cd5bdb96559b0fd78ff454ccf12b5404530f359360a333520dafb8cdd92fcba9edbceeb1b1d6c69e72db72bcf4c407525d0cff28f34a55a8b13d" },
                { "nl", "a2e44108548a5ca80f7a89719ad61a5ac97c6ee8f472afb97a95459017fa0c4cee860d411a35565114ca4089897b1f67579d1d678f01f6b187573490990f621a" },
                { "nn-NO", "97d904d5febf1737e4f8c01eca1d8a77593671685b960e14885b2684b8e7ca56f66cde5fef1081b2a73ec2d1f65c87c516eb1386c7644c5c1af705defe38ec69" },
                { "pa-IN", "4dc07b341d00b6a36f43425d24fe5621d9087b7f81c5cab4b9e3601b980a1299405f9efab879ef2d5b1a1aa77a00b7a6895e141c53867dc6d2b75c0eb6feb349" },
                { "pl", "4327b06a0eb3cf00a20f8646cd72e1004c6ee065a087d3bd871c443dfcd33aec951eb0e321d6a8ba05b672a375e2283b2c78ede7ed079afa84ac72c628378922" },
                { "pt-BR", "59002dca08833bc7d51b590b3dfb8bb691ec65cdecbc8ffab51907d47366b18849028e60f3bf907c640af492267cded443d3f60dd6dd4068af86392384a1d052" },
                { "pt-PT", "fcf0e9c1ca4d8c9fcbbf7d64f6e5b8295a2864101726ca1db4ccdb4d4c9aab0a5fcd636ee811ce81323ca49dab9f40b54c0d616d3b6f6f19ab9cb7d4f67dd836" },
                { "rm", "6808334c18460615db7a45815e4bb335d1b38c08babccbf9632bc85601a550a2094ae7bf0d2b94b58b65f5148204217172190940c978a439c6280f1147fb47eb" },
                { "ro", "7f57eb9699313a28d1e6a20373efb20ac850f78d9023c87f60bd606bc5902de8d39eded3c1be751937a81470e511967e6dfcb4d619eea62a9eb3aa18cef76e0d" },
                { "ru", "fb093df0a56de544dda9075ad442241a024c318fb3594f80be738501c5b978566be5cf0012656d64b5d0679ce5ac09d66833df542a3f84f2bf1df822407c942d" },
                { "sk", "ff2da46460269f796535cbd1506655723236dc28f75df2a84d58c9338dc9f793c6a177fd090444a18ccfa3331130d93ee8f8c2e762f4c225b9c6fbe6399f408c" },
                { "sl", "f845754d71784036d6fd040ece97d3b95b9c52653a0659b899d5a363bbfd64060dc406cb6713b1e22618e919ce5d6b2b3bb9538096d4a9e74252cb24f769b642" },
                { "sq", "58e4a76b68dcbcc2cccdfdc44a6e5fc72862ee815017400d3bd800c4d54454cf4f6b2324c5053fb0448333cc3aa7369446bce3e32c4395264f0607a122e329fe" },
                { "sr", "f8bd030e10cbaaf08359e83eb3c4bdf4bcbe1744dc9fbd978d38c37496680969083083b78a6086f1a772b290e8878c6015640f30f79edc8a8441fb3a4f3ec046" },
                { "sv-SE", "cc2db96ebc21df637f091ef29c877866df52036873ae6b20892c9ba2c5caf1a35b91a7eccb639953bf64dfcaa53014a3a95319d00b1b48c3c169b4302ed2b75a" },
                { "th", "216547d26e0d24f02515c8149f1f1521d71300b0fe241b8053889a79be8571378afb671dcdde01d5004de07ee3ed549bf7d7b530d78c8c6e76a0921f2ce24e0a" },
                { "tr", "729d8b9ea97bb4a181e0476c92fd90e75849fee4bc1ba8a51eea59ba45d281a7fa348cfe056f7de76bba8f17ec8890509040f20d192dd74ac182f510724b4e02" },
                { "uk", "d9094d3306b13cac214e391320ad6e112e2f3bb2b66b46ab1d3ed8e2b4b2bf119022f08d64bea31e560187cf00722bad451aac9ebd37c541c19c614ff23d71b6" },
                { "uz", "57edca4ed205ca31e7a7dcd0cce26df35882e95611a07d9b95c40519a9731c28af9a72495539eeb4617cc9332c5050df31c58a727d537990ad45f9fe7b88d871" },
                { "vi", "32cf602dc021f6dc9c941255f3963cf6fe353e832d665218118760d42e7bb439546458c18a0134d5d870de779a432a83cb5b290dca4a0d40469c644b2b062f5a" },
                { "zh-CN", "49a6d46a7ce9c0ddc73af5af4b1ba2160644387c43633b034f8e9d651e898172746183a751585c08c8a7b6022b1be505aa56910574092bfe919b9cd82dcd2d0e" },
                { "zh-TW", "a3e71e0011cb7b29c25973aab59b6216ac98c1859cc95f41727907607a13a183a90886cf91bd8baaadff7320a47a7bba6eac62cd1df8f7d82ca3fc73af224aea" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.0.3/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "18f1f4a02a87ac3a88e64dbb2d33bb27b068a0e52572a5ee5705dd881478c1f7cc8131bf68a081e8c548d71a152b019dd48117d609ea847ac5a0d6ae25ab575a" },
                { "ar", "6b7bb4441969ac60974efc18342bb32992fa12e6517b0a9250d75674b43b1bf8002818c9601cf3fc3fd2b0f6774a1ec6820f7c35e488762572eca32b5fd77c23" },
                { "ast", "2db0f6fcecd373fd1c1224a1ab0ff2590d5d13e59a7eee7c2490d2409262e8a41826e36061450cb6417808ad8b2ed7169cd6521af65c6269181b09d90d30281b" },
                { "be", "6f02835e6355b180a163991ec15419f78f8ebd7bcfa9cb3371115d22d9dafd6f8d23dd59f3825d88d3a5d52c4077fdf072155872384585d4b0943ee53590dae0" },
                { "bg", "8a30891511e55e4e88437b69074d0339ce9dcaeb0a100fdb7ef32bc91c797e0d8025245173c646a5b840e0cedc53cd24787890057d57b3b4d3895d85ba8e26d6" },
                { "br", "d417007759220a0365f241f4e88c3ddabdbcc77e72a4039b17b83955b97949a2f28024914f2534cf5c6c9f661a89d4b94caf9cd18a6dddbc84fe3326e6a96e8b" },
                { "ca", "5251042626f2b0b3c77aee4c6ab3e4d4432fe17d786a265f72f9031d372e1f2627284267986a4aeb13dc059148bf65b8d4691e13e15f3b5442a1776048bac0d8" },
                { "cak", "8808d8ca7e24f390f2b6ccdce1202c93fcf0e8c58804a91cf66aaaf3ee2d65e65b3e405caa7a48584b1e4d8e577dfaf99503a25595dc10226931a450186cc80d" },
                { "cs", "daa71e4e34446cf6b966c010dc574bb9d4f46880f463c1cfe67473ef6f86b9a3746b6108641ef8bc0bc68a0fa53e6d44c146e9d198126b30d3bd5dbcc4f24b0d" },
                { "cy", "228ba711974667067482bb00dc961ff367d98cdec67e5dc24095c3ad8eb58d96942e764fb1f9edc8e971cfc20992b848b82fcc34951fe41f5104d08403e6f137" },
                { "da", "c36c61454beb9a1ec9294bda226335bf079a51d92227d88cc3889aad0141a6f8a21ac3876c3f6230bed8ae50399f89b77fedf6415879d51a7abe54a9953f054a" },
                { "de", "532cec15ac1762298a122d1eaa94db068a7130dceb986ef4b5595e55cc6a0f2c94a4f74cbc522e3359bfbbaab3c2ddeada23cf8b725e5b7db34c218d9693d50e" },
                { "dsb", "50fb969291f215b3001ec7cda3d23c0e83a9eb9c5dfcee023778a41b5a1eabdf21c4f7445e59496d974e87f0e420170ee0b4cc41a31429ccb60caeeb33742d18" },
                { "el", "e656731831254f69c5868eab73e72bb5edd326cab353587e3ae3ba9f515cc6f5122ed8bf64be68777b608f9d0bc35ad660d95b4c7e44e221a08c3ff6dde1cb80" },
                { "en-CA", "cbe8c9601ef857ab293bf23d57c01a2e29d46c5a56b70c3d8660bede36fa37a50d1f7f837f31ab6aad4c96faaa6c3cf16f63f0e5346ad88bb3943d9dc3b29d12" },
                { "en-GB", "7c3d1e7b800b5561d955a248b1b6b06cdac8223176750621afbf86270142514013b1f6c5825f98b7aa00e1b4ed59262eedb5ce0be187b0fafaf001cd99010d3f" },
                { "en-US", "987fb3a8b753c4826d84363113c6b81b0f6048fef3521709634608d36a217627472687abad68b0b7b592b7d75d290f0d3bc0ff7735c17bbf73e492f623d54d94" },
                { "es-AR", "d410ecdcce6e712b05aafcda09d52a4e779cf7aacdee952edae12b9d7dec3f4e6d1c8c82c9b589dd2aed742eb2ced83f2bda00250fcc580716244fd913280f48" },
                { "es-ES", "aa8770577ae24c2b41f08a0442c44675bcf00ee34dac6a870a312555d978c4a8fe4b14fae4db1e400259f20fada1cbd7e864ed7230eb0cc559ee4e9e96b47add" },
                { "es-MX", "95a44047098b6dc1f65088223f2fc9dda7cf79e8119e4d65219586a9e85b6416a1aa1a8fdbc12390a4726b7e7ff73be834c1b6a34f1dca20f9f6c09f5461cbe3" },
                { "et", "d0971a774d844de2139f1c72ebb2247797b49d0e4a36d277045baceafa69fe8c65aea83d91b956947be30c428f66a46b25014e26f630c25d1ebd8d5c83f40eb5" },
                { "eu", "51f8829e4781d8f33ae6d9606d81a8f3a72a4b6d14ac53fdb06f46422cfff57149e4874e10417ab24341abf684e7260e2129b96efb35c58f20e2263d58d2f2a4" },
                { "fi", "b8be98a603de89442da99c737804129b47f586522da334115464467756189a651daa6baa611ee152dd4c24bf18a4163325669ad2db83a28ff1a6d848da08e485" },
                { "fr", "86dfadac02f58fb369e3d06ac3c63f92db1261ce0fcfbf6d206c4c2329d511b8481d8eab791fb1548501530f9487982c7f22adce06e94fcfb0492cf204d607c7" },
                { "fy-NL", "04b51bbbfb26976e144e2785835ab9cd3b1d5fc68a40262cfb3eb2457e8a934f76b471a1bc5517a66ece31ecef57b26a7cdf47746657e69e465561844455337f" },
                { "ga-IE", "32c2c8d9ecc5341c424bfe0d1b30831227c1ea89d259ed5775f570a4cfdcc17c8fb347ab12f0e00d3788f8f6dec5d4042f085075ff839ae5276fea09eef82411" },
                { "gd", "902077131ca85073719a608e3e24c2bf7dce9d76327156161758017ce7de77b0bbfa58064ce62604a965e50a41f7280bd0673125a1163ede51cd0204d1f1b451" },
                { "gl", "89fbf0504f0bb53e5dc012e2e2b724b8dfc21cdf5ae85ea88a6cfcabefc43b57fba0125f6b48caf5b0963488274350478a8e379d590acb1091a39e27768ff8a9" },
                { "he", "60c31c79a67d53296f05b62b6e4724c843f9c4e43cfa8da8024b3e54fe180b9e1079f175456bd4af4b884748d82fd79d58f7bdc34274241b832ad7a43ea581a8" },
                { "hr", "e7621601cfea889197bc5ebafb17641ad8375498dab2ddc010090538a39fd26ebf9ef6423b88fb6afb39d21aaa124a5054196c5d82233fe52a3f47b6dd0abfe0" },
                { "hsb", "e55a9506210da42b315bfd48d505cf4375ec5e3c1fbd438bf0864934ec14892bca0e04f412433e633d399093092b09a57959286a8f455229ab8e9f37190154e2" },
                { "hu", "593f6a136da9cf62bcdec74bbd1da3ae06d092f317418d60c8ea54226ba3ebd8c2b95b8f7902c8e5c1f5ea1899c929b58fb4208b40fa5277a0e6ecbe2e8d8bb5" },
                { "hy-AM", "14336e131ebd5c0a622050ad99813ff1f907c8a0b46adb06e5d567910390b5ae2fb4d2f4f2bb052921e09168c97a4dd0ea767db197dbe7805f791bc845aee06d" },
                { "id", "5718c02155822c2e02ae5c503d53b07fe098854f4df1a10f628aab59a98e857c24ddd26245af57f5d6c01e9ef6f1675ede918592006187dfbce416d3a3f61276" },
                { "is", "a2d729c69789d53fd82370608a6b257b1d25aa83260468b7f6eabd4f331974e7e64931fcbec6fa36c21ef714cf293a17c58419385d05cc105456ef058008bcde" },
                { "it", "d7b3b7f943ea6bfc097678419911eb6e85e4b3a5c553b769309958d10b89eaf4adf0f6073890e8dec9e5f8280c26d6c87176d49d1fd08ef2277ba222206ff02b" },
                { "ja", "3b9556e8ec5a8a848fe33592441ca835b05cb7886e62f560792954ace454583dc9c75f2ec2e4de0f2028cb03b1becd04775037767c15e370d80e09a7f77205c4" },
                { "ka", "7e2be2f41390e4e1aa940e909827e3819d055fd05ff1f5267ae036e07b9a790c7feb557310ac9f1fd5b93bf3497de34092e40a9af04e15d855991f8368d1410d" },
                { "kab", "d37affb95400e4752d82ea1fe1b1609f3cecce91cf73e90de367012f42e3b8243bf606b520ba1520b5d3b4c662e722adbf9fd433346f19afa74051acb9e8982e" },
                { "kk", "d73814bfff153ea65f5af14cca020ecadc29d010c8ba93287d3402bd0ed70dccedbafd7c767b643077282b61215157094d5f025ad2e886b7e7cfec33d9e738d5" },
                { "ko", "055670f54630e9687be0c2513bae1862e7fc749648a438cd50eef850b4580574022976a843588ee7e5201b255d3b451567bedaa225b2969c58d9c9dd2181326d" },
                { "lt", "95d04a45b23eb8f22cf0240ee6db4ec710d883e566cb31ce8c7cb1d8017b0c8cda4cf50dc86da797e8d1d030e476e89dcb35e91208ef48efaf73e10d63a58bcf" },
                { "lv", "fab95ea8514ca64dd9b970b41d3a4fc8a98c5a969db25935fb231cb1a7e9f0b0551081032148405fc904dfd5e3865c5d40ba9cf121bc8700030a7c8f830bdc56" },
                { "ms", "f0d8f416ae954d9109ce9844e347ce36d3fcb70bf4800f8b7ac4bb69546b7c4d143af36ef1e1e63a2894b2f18734700209b1fca0b34009f466498bd5ecba263f" },
                { "nb-NO", "8a9df2916368a2f63d49bee6774513b0bb851eddee23a2754e90d16bcb9e7f4776abf1bf0b1d8778c135572edcc9e44ba169ba46b8f4ab67dcbd8b367b75b927" },
                { "nl", "4a54f394c697266d535454650300cc00f31791a8143c57d183cb8017a0ee95cbe89b27291b9ae01d8da734a4d55de1abfb676399ef2f8f80c5f16df6df7e4ae9" },
                { "nn-NO", "9318de4fc69eef33d8f217fc6c1c8a317974c441c961ddb832f4cdba5d69a672844fc0083e086b068bf0dddfd55eef7b6548eb92cead9946eec7669008baaede" },
                { "pa-IN", "eba9b91001ee83ac3090ba55889366067d40a89ffd0f3518e3144f7b528527ea2e1d0d4a2183847e60804348d2238709dc97bab8377e08883977623d71f38cc5" },
                { "pl", "6e0ff531d213798d2a43d21fde4a3c4a3d31169c1be5dae3792453e21c028cbe9d4d07cec7be6016618dede715a44a98b7d96fcab9ce33c59a8de18e9058bb90" },
                { "pt-BR", "55364fe332d36d1fb0bdc550c4f2d611d8dc3aa435d8ce69e9c9b85d205b6c36205bcb8d0a5586bd1c486a49c10d4aef842e59ba596c06f8ca3d9c81b577e25d" },
                { "pt-PT", "6da9fc75d8ffd5a35cf2591abae5f302c9d74a2f492d6cf7eb37cd2d8ff3961ebfde2ca7dcc2ef0e1fed130f6bf55172b18c78486d08c2ad4fa0a4210269031d" },
                { "rm", "c5272347cd8053b3f287adc7fe09b0009595c3ab219e1dd5e322cd2fceec8ea5c508bdb117831973b4ac3da66989c4d12502b7ec35406e1f95b6540dab6d306d" },
                { "ro", "0446eef4956e63465cb27215439f88de371c932375ba7037d2e646182e66bffd4dc5f3df0575da524629df4e40ba04bdd0b4d900cb2f69c92a2c816be6308bf8" },
                { "ru", "03d742587e9b00d4e373df2dc06029122f6b6f80be6d2aacce5eaf14a76ff8affba374c0b3c52a7f685d05fe0680abb23ae80636fd1f5940d3a5ae3ab4549636" },
                { "sk", "feb56ca5f9ec0889bfac57f22373d9f78b3041d343542a5dd011129a401d9b88fd10d1ceaf692bbc69370e298ce0a77bd70cf5c0c7635719c5f77cc502a36834" },
                { "sl", "8f5f91dbf0e22f6ae43cb8501f0e21aaf3e54c6ab9582776a93189a6046a77bb7081ea9ca337867036d835973da43538175c5c32ac824bf999648ce6502e061d" },
                { "sq", "fd58d1e2cb8807aa31f42a138523b2cb0df9a2ccc17be4c8d07063d964b0cfddcde7b51355f89568df03adb10e450feb6f0a152980ee67f7888c2e1759c4bcc7" },
                { "sr", "646aa8d64305f65dee2cf1139ce7957dbca518ed517f5ef786fed93a3fdee5c6dc73a5adf9a90b9c0e7e251544c9125ae6007f0498bfcd0cb629506fdf464929" },
                { "sv-SE", "b2bb3f8ba0b259c977dae3869970a38394a7a345388d068b6c9cdb9097ad9ecd75f349e85554cf44824c2a9df694dd48e6211082cfb63f7f504471a0066af17b" },
                { "th", "c2de15bb4eb906919445ddda957ee51c4239fc9258180f393d914b3beb51ef136abfbd7d37076dfb6c658fa97ecf69c79a027f4e6afa1dfedeb60d3b69756b19" },
                { "tr", "aa9d498cd0e8e32a7b3f862e7f147c39fd366cec502c1c44d4e7c9dc27c773aea887bf2555648d83d41aa1d84cab1bad039840aa095809201689be57ba471eb8" },
                { "uk", "79011f0abb8214b823806b3b8418327d5823dc37900af98a3e1a774b79fa099da77345d2698aefae82995198ac6b9933c7ce419e359deff56fcec54c09ea8ab3" },
                { "uz", "db1653937d9b93632e7e9f0039abead2ae5290e9bc67e83e081c753f2aaf4c514df7e6bb5fdfb8cb6a971cb2edda1623a287c57e1d62398ffd29230bab10e5f7" },
                { "vi", "05f7cd86be3caf6d5a004f0ad911d3bea45966e4fcba9329e2e6403d8c51217882dfe1204d38d9fa077acd3b3717ddaf5bcca150ab3d72b62e770217d9a02733" },
                { "zh-CN", "c340f6f13976d5c52f578002d92bfe0c33fb79936651343050eb5f3204c0115121ccc0ceb9ad33feaab48ef25328f5fa5fbdca8f43b4e0392a61c7688d64ec95" },
                { "zh-TW", "26d905f2a66ce5f3d49a605092a08c18a3c672287eec683aec62687ba785b5a3a08a3a2e07dac30f2d96370b7f49af8f2d3ce6149c78e245ddb860698b018f2e" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.0.3";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new HttpClient())
            {
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
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
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
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
