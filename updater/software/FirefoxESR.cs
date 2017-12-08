﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017  Dirk Stolle

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
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
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
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.5.2esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "264e75788b1ac534e8f9c8933043f24f300d9a27278a525f29e06bcffdcb1097047071fdbf2d86531195f23d46ee5061b5fe61aa4effc2e06c28cb8c9c933067");
            result.Add("af", "e5eae0c9923025b1e15e8e1601f626023b29eec1615fab65b0f64239529a6a3a92294c0c63616812f9705db268545a880eec85720887973539630f27d7ff4e66");
            result.Add("an", "00b8f8fc2c57cbb6cb785ad1f20e49b8a3a50d27c3cf63b8ab9fdadeb66fe98f31bcf714899bd1981fb117fb95ae5d3dd995592085cff0f4d9ad07eeab79c677");
            result.Add("ar", "fa7b538902dd698bcca376c3b8dacfc53566cd8b59dc0295b637d14349ef7ebe94a90a9bd71aecc7f542d68afa0336b10e53b67c5d82fcfd11a334a1afa1d080");
            result.Add("as", "ffef3b5f2c3f87b304547a0c4683d65c10c974d128e05daf610ed811e4e1f00dfa46f341b63167de15947a8b0bd47a7d0ddd3bbe915c3b0360d4c9a18b408f32");
            result.Add("ast", "a0850467e520e4bc6361ebea804e1afe97bc9b6b793ecbd88aba0218bc77579da789315ee48b72341ad6afeca94141b9f5c6469812e70d832453218a233c90ad");
            result.Add("az", "c4556f465aedd295cffe5c08a879f009349a449bd5b1f2845bf9e7551984cb77254669b900e8fb6fef86151671fceb20352c8e2beb96c9062c45ba540e4f1e19");
            result.Add("bg", "a86c8120fce59f6496cf3376862cb077064b8eb9449896bceb282593c18e654f574a36786eb8d294a48e3457a5a37c5dab2673a116e5e3d874064cacfd296588");
            result.Add("bn-BD", "3865ad0b87828630c063eefe496af49db9d6f7fb561690c53727f132e448fa8cb53e90d19d48c8e35888470777629ffd2bc2b8e2ad6b7e62578ee7b79b725c70");
            result.Add("bn-IN", "dc1416b7da0ed50093add5b7837f1cf3f1db764d7c773d87224d2955685b48ce0a04dba897703e2904101935e383671afc021fcf6896df29d996bad50a26df13");
            result.Add("br", "23bb5b03a208083ffd2d0d7bd47210b35739fc8318860d12168ddc608ac48084efadc0933d6e08c6597a5b6bd3f23e0290953edfce9d7df76fad06309f564736");
            result.Add("bs", "5dabce600b428e188e5b0486e8876bb963c20382563f703f2da894e40e58ec47ff3e7ec7e0431c10a28454069870b5fbfe8b1542eb03150e505b39aada370b85");
            result.Add("ca", "1ffdefeb24d8d9957db81adb9d5312e972ee95f0f3ea261914f267a29aa051950c763d6f5770dc022b9669a1669e9a5f3b2fa81b0535dd1e6a6f2a46339bfa9f");
            result.Add("cak", "2bc2a2d64ea15e2e13a6fd2d845bd0af5552a04474e620fb4b8b389791246409d61088b994387994ca8c5035021eaaffcc47c320d8050423134b954e8467616e");
            result.Add("cs", "befc91f7539a67bd17e822af84e07fb7584367f1dfb6da7b557b02c0416b8620fac9867e35094b201ee3019dded7ad238396cf4da247e43886a2dc496f5725b3");
            result.Add("cy", "9303081dc5f69053dd4beea102ab74ed29d0a6de4cc671f885c254845be9d61ea4504fc411100a411be43aec9175ad0b5ceaca6467206ad01095b6788b2231a5");
            result.Add("da", "8b11ae16860630b6d9a20480ddbc514a06f2e1f111eb51e3c2e8d02cdc2a40eefd5fae9f834e97f1de06205ad5d1c78af971f73ae10d66ebc2e827a095984aeb");
            result.Add("de", "c37b6845e031f6e004baa723d77ca73f8ec6abeb21d63afca693fa3c89a736699e08380929d2fe9bfd4b30a50a8d3e0e1f955e08a437aba7019beac045259541");
            result.Add("dsb", "d0d0db30144ca856302837fb1a3026917740d749246ff7ce76b2c940f43e231af9c58d5fe9fb47872e43e1fb86c17557115435ac5de3855d7772527468bcc08f");
            result.Add("el", "af941f865ee71dc2547b9abdeda720ec4f9de4900f0edd31544df830f7472bfb996e661652b19795b89541ee8b6f1d13d92660839d134f9ee11f2ceb49a55554");
            result.Add("en-GB", "5a3f5472489c0d00adeed2ba5d1624c7aa908d2fcca56d1eba03d5285eef5675d6985559d3b2d0215fb8beddd1569ffbd8f843d6e7d6205e9897dd7cb0bc3c79");
            result.Add("en-US", "91409247663ad3197bca3998e5bd3afe7babb508f01749882d25d1d204e239de475acafc737f9ab45c55e9e2ca3ea4f973179f2ce9f091e953d03b6934bd85d0");
            result.Add("en-ZA", "4c75fbccc6baad29574f6a92f5027b5e1644e804200d6e0293ed79d60559305fedc866a90408699a24c5848f477cb364e9f44dfeaea05f426645353e524c9106");
            result.Add("eo", "c6294eb3301d2e1c2d8060c278b973e8faa82cfc904e937140848656c9500537863577ba4d49cc8fd59fb58ae8fb5ad01239b65cd623e107164624b4b2c33638");
            result.Add("es-AR", "7c121c95e5ab48657d9940ae44a58d864e6b4a2b5eb1297fb324d3f6e47b051608225abb33f654c06fd3e9174bce199601f465571f2abdc19600443e6130e2a6");
            result.Add("es-CL", "31ca9e295e39e58bd2c8e45692904fef5668024ddbb2049cf9894ecc0360fdf2cf90e464d5ebd65d552de3bfc06d85d0276c03b7cb45f7a04a3a955d7c795d2f");
            result.Add("es-ES", "58102cb3435b286ae1cfddd37fe2b5291a99afc8e9b58c8692170ad0afc22323777397346e4b45757b393c6df0ea87116dfcab76890d38c43ae92d54d4599381");
            result.Add("es-MX", "d73728bee247c35ad4b975d55b62bfae3a431ba6fc395eb39a30c0d1d80889399da639a451f11df121179eb5a821273fac1fd81445e1f4cc2dc302deb8d82b26");
            result.Add("et", "25fdbd7a3a622788546d10e55c271297fe7693ce38dd1722a6fc3af71bf8a5f2cea1adb405714260c671e7a4be48718f18d1cbcccc8d9a2bce6218cee56ae0aa");
            result.Add("eu", "5e3b16e6f377914092c4fe68a2b774c42c3dbba9352fdab6ff2045517b9d33abc0ba9f737e2b028b729b90b5545b1002b01899016255e29b01bc8c1e4811975b");
            result.Add("fa", "bf947cc5e9f44babb10bcf0f282821b3de56d8f1ad9f22def14d0c5a32a494f2dd033806904d4423e90e5fe4c25178da39e598fe077521d37ab536467ac37820");
            result.Add("ff", "0bf48164d1ff42feac40eb500f07453e33b2f0ad5b57c275cb247df79c25b42a2fa8a232013c70e8823da03fce6e738baee2c862fe76251f362c574d369ce9e3");
            result.Add("fi", "c32b86f9771e08c3ee07a497580b2a48d146dc2148f4d10dcba54a878dce89b87d062427fb11abb712f53dfd789557181697a62f29cbba7021102ae51cedbcc1");
            result.Add("fr", "9599a6358558baf21e205b2280fd5ef245e7d1e44e40f3afb04b5668cd7fe9530ccc5a1383f03a75a9ac57f6281dd466e01b484bfe9443ac04dd51ce7b9cd94d");
            result.Add("fy-NL", "8385506add3f4167cdd10eedd734b567abbad070432c7a5c434b9746b760a03d26d902b85c4ae72e2a0b9eef7cff05092c687f6e654a1d9554618d2e8071fd83");
            result.Add("ga-IE", "b9a70b81a85a4b5586be4c69819e2ca75c34579e0b95d7919d930ca46a7fcec6c5be41ab512df2a3035a37fbb06b37f00d5eeb55959745084b3a0eb6fe17cc21");
            result.Add("gd", "2cc4c1a7e17a832d1cadc5ab823af89c407659b476c0c740907ebf7e403e52b72d0419db9e3fb9d10587c9ec0649b25bb28a3f081cda079f668c572b6ffc040a");
            result.Add("gl", "9b79814061044f93cf23c54f27728e18a5c4b18603a8846bce9d7dd7cdf729178e3e7c822adb60b4e792b7e4fa381e5b8971efc3c05dc159cd05692572bdfc07");
            result.Add("gn", "e82f58ed1e1420b9bb9405ff925715dd361e589caf5233b9b1bd55cc7ae37b55eadf23ae91badf57725032ef705cc88595ac58de558d40e8421f1870fd2e2929");
            result.Add("gu-IN", "ed186c584bab6ee365e195056ee93d996860c9381fe3944b432e97c75594ba378c168e6e11b31ec0da0d704381195a158f707463e396809a273582ea0a2d3cb1");
            result.Add("he", "a7999909e5030ef8be0fbdef123f1f80c1b843caa3a0ce09addd84517995eb8217c3f6c9de935c8372b7a88c4e0a306767decd81ca05e9aff2f44f7a7cb31ada");
            result.Add("hi-IN", "c79e3d1d8cca484fae8e39e4fc7ed8d6fc128f977d1a1a77cbb805e95bce7d3c44f81c62c157344a17541a1126fb58f9fa50aa98f0e1a70b8b2162ec39845cd5");
            result.Add("hr", "0c7008b959fb0e10b5fbf778e70ca7b2c6fe26630f1fa2d4155c399247a84807931d9ff426771bc0ff230abe30f9e4c40b9ebb8de4e57820ba32ba7f1dcdf8e8");
            result.Add("hsb", "4c04546eedfbaafa0d98d9335e1ccd4cf60b3a333a2636629c38f0d996474199061910e6ccecccbb6a408ec70ea947e60c85583ca7dda4f6761bd9d475d448fe");
            result.Add("hu", "255fb7672f903adcedb799b97394145edf21527f1b6d104570c316fd0629848ec4acdcac46a89cd72dc15332b6f65b107df9ed20bfe686ce605bfd4f47ea3631");
            result.Add("hy-AM", "75e69c81b03fe9066e7ebed2eedece3e8531bdc7bc2cb37180f3d8ccc6a582a77d5105e09a28cd2a6ce29e0e8fe4f062c0d1a2281788e81370fd20a37279930f");
            result.Add("id", "0c42ff1d03d0a0522a6f8167cba818cae9fa741236469a8993bc0e93e45f90aa36e599c4ae393d76aec34567298054d5ec5ff2c711470205a19661438c35145e");
            result.Add("is", "d4d91c3cdad8ad6afd477c47a230639e11d80b665f6d62d994b278417cb25f8d29a2881ec4dc378e291648a87cf887e779eb90c49634cd451b9a9de80ede5119");
            result.Add("it", "29ce7434e74a74d1b881f97d6b3cf6cb0baa12aa3fd1766a342b2c37fc75ea52b98a78c4ddaba69750d1edea2eccd4e8573a798e84d06e293a4b830ee6a326bc");
            result.Add("ja", "d4101308c6486c2aa3880d6915dbacd0caba3bbe389954c47d83c5015d28d516a31fd5487ebaadadac9e8b440d237945da725c0d51ffc5c53a85234e2320c719");
            result.Add("ka", "7e840a516fdcc3ff9bca5da291a7feb5be38128c6be73eb7b801f8566e0a8bb924dc5d67f3c143f009e75c96540311e30fe521b304f15afd1fa651dd1b716c86");
            result.Add("kab", "7a7763795dbf3ae337c76116f3a7bab5a0136f9f5f650b93e9b6d3fcc655c659d92f81792435577f300dbb28fafb810448027659409519a25d4ffa295825a5cd");
            result.Add("kk", "fbaa4cc170506469d726d89e0fe12335ce81828348d00f62d098b05274c3398b024c0c618901f02147282b73ee34c6c4ab3b1cfc2982b2eabbaa9ea88d970c7f");
            result.Add("km", "1b49a7aaa4166e3bd79bb83b03145b38c8ea0e7099c8bb5ca3a74ccb24efa687716e8fd1764119186a31487a3da284e0371221d1ab1c41458ed7031824db0a71");
            result.Add("kn", "b0b157cfc66f41ba74971a359792723d0dd5d289b9d22f90cc390d1342491e453247320b50767e306ce0e20a40efae2c4de51a18c9a1c790b0e36eef05695bf7");
            result.Add("ko", "90bee9b55c03ccc903e01ee3a37c9d16ec333de94f1ded060de3191e57d2e623b613a4a90a81f6801802080cf59cc22dbcbfb0cc677f1ac259831f72725ce99d");
            result.Add("lij", "af9f696e4a6eb6603010800536623f4d0cea6d08817d8353dc414552d5afa38b2a47ee559cd598553e0e03430353ac14728a4228af99778fc44b556f1d5aa62b");
            result.Add("lt", "80f3633ce7f7c6045b597388ec1e6fcaf5405483d71ab68544e20d703d50ff7339968f1a88579569f133b8e668f349f805dcc02dce1bfa5dabf34bf8d52ae2d7");
            result.Add("lv", "58e8cd5c70675afc3ddd84d9472dda58255eb35b942b81c9e6dddcbe454c0b10f1827de014dd04143a21ddb2ee80c1c2c7feb0dc82ff72566f033b08d52621aa");
            result.Add("mai", "ae8e59be3d7882527204566c9fd8a902576d3ae12ea2e47ab7b53e8e8d4bf85a77e8c6901723fb28f78859450cf80d773a79e70bc1403ae42d3526ddf41a7e3b");
            result.Add("mk", "b29c8052d52f401bc68fc6948782c54685868c881fa8fa4e641fbbc30b896c8446471806a8d00262268ec6db072d5e1c3638813ef6073fcc2acc97931c03a6fc");
            result.Add("ml", "98c3a8f4926f724a6ade07e0a174cdd8c7a59648f826ab6946791520a18a57ee72c60417287679f7e3e509a02c0fbf1cd233b71ef1da1126f03de80715e7ea42");
            result.Add("mr", "82b31a3f0af08dbf8471dbe4ea2ac3993d3bac5f7fcc899e4ebcec2979e3992212a3565c969818e24f887b1f924fd9e9a50b5041126788bd9d1e99312b8c8333");
            result.Add("ms", "e3e9eed42bc1322be9d2dead4d2e843dd2e9873b51ea541dde4c7b1ba0c940f0ce18c84fd9b8b4ec48d0e712c57037dce91656ae3792f5a5cf56864cccdc7b67");
            result.Add("nb-NO", "e63633c9fa992ec55e648a305ec21ea0483e628810a32284b33bca0d36b0cdbcb5af662ead9fd14af7c10bacb4b07bf3fc088c380ae4925aaf15bcf0b3e2195e");
            result.Add("nl", "d5cadcede58cdf40122e557b54be32cce9fc5bcd4ed2d4bf5d27e11f4a6f560283ae7e344d3527bcc2cf3817fd0b6da5c16aa4ff4f6bf7d3ec2506fa5fd0c41b");
            result.Add("nn-NO", "f30add336ea1b0b021b94fe041623f5d58649dad43610280eeb19eeb2eb2bac0900b220e6093b2949fcec5fe87653d873cc29330ddf266f883b20c33746d1222");
            result.Add("or", "3f68d0970251b4f867dec5c05bdf3a7f378f698c16fb9abd6c2fc8e3c4acbd856d16561c61670a8f757d49e7cda369c23c0e4c71a8917fda1be113152a2b2895");
            result.Add("pa-IN", "2a036c76989e89c3976796a8dbbe9ad80ec0d8ea8baef28a1b0f6d7da373ce738db827bd12c69d9957d6027206ecaf6e4a331fa62b0fee3931bd2a5c425c6311");
            result.Add("pl", "6c1a280bf4866d5f0af597db115a9e161c4cc3867c5d69072a40c72e6a567d36f5ed3f4e4fca931d67a6fafab4fcf80ffd9e1cb011ed26f21bccbaa032699726");
            result.Add("pt-BR", "4a4300068f3e657c3a920df8c766bf1179e2cec1f729d6efd8d531a23c4a36dd6cb70e03bd8cfbf9cb5b774e440007acb2ec2ad0943b383f3fafcd8fa4828d98");
            result.Add("pt-PT", "ed38873eac0a70ada7b7853cad8580786f28f4cca70ddd04d24ddc4edc92d7e32e589d30ab5de3a5c6d86eb7837a2d4ca82f31deade8562f333e9e6dba46da4a");
            result.Add("rm", "8d72618f23aa4eb0e9e06cf4a5af4959d096d9e8afb48ba0d1bb0766c992f3a4f9fdea66ec2dfc58f4d259ba61966c83020b38f3745cad033c7b81b2b2ccc85c");
            result.Add("ro", "b3b82e6dbdb34825c70ecc4ab42e84594d5c7a9ea0874e777cd93132f303c2feb5b253a79b1fec7383865ad30ba29ed6768395fa0525ae34f8fc9b55336fb425");
            result.Add("ru", "426ac7cbcc7cca6a1fcdc3ef7ecaf60557dfb8bf655c96b24695367eb7bfb460a785785cefd1811ae978fb46f27e6ae949718bab8f85b7f645414cb572634cb0");
            result.Add("si", "1c2e01f29e74f10989b1859b6117a6b14eda16ea56cc3b2cded01fd4029d28dd9ee63e4ef8822cd89be70e0125c832488778c1b745f254427883e87c21eb16f7");
            result.Add("sk", "67cefc8688240a1d293b10eafcc5846bb4e27e9655994606b70ff574dda05273b88061d7aaebe4f4eb00b698747e17f12cd764c241eb058d6ec871c563e98718");
            result.Add("sl", "4c7bd049cb20621cee78466343e5fe7d2cb9198bd74997705e6529473b5130e84b80b082021ad276d172c686f11e518bbb2cf8a6a1dd08643459fb57a3b83d81");
            result.Add("son", "8c7db6dcb7a28e9d4b57205902e0dbc7f65090e38538a49cb2aca81373bb5bf56e252fe48714cdcc76c91a349e7cb188a378e014a1fc8d4b3847f5394d535c75");
            result.Add("sq", "2ba6ffda8f629619262eb75ffbc4c06ec6549ce222d86b7441a9015d0f16ad59846425f7d4cf19ad151eb404cde1b89c6cc9a844f90408aac858c1e0e968c4df");
            result.Add("sr", "7d67614eb9fe60dc06f0b1ad9575ae01523bfa036a96ddf6605ff82533bec97dedcbc04b4ef5e1b21710f502499831485a3fad1a658548e79c2dacd463dfeda1");
            result.Add("sv-SE", "7ee5800d60b9b33b08be4e78f9bb86f0ebb5ae2ffa57e778aa1dd959ae2bf63677d351ad8779ec41307e3bf781b3319cc979a86d1243e677595ade52e47c0153");
            result.Add("ta", "2fb548c99b461c9cb64d06b12af72a5db7be74df02a77b1e25ea7b3e13e672acade7d16394f27954833feb28910845066884a3d420edf0f9efe23baecb24c9b1");
            result.Add("te", "aabb80bb4ed18530c9d22ff420d7724aa270ad88791b93ee6ff5365bab79c2091e7bf6b2a84e8a039e79f1de44ddea2917e6703db07119fc07e420160f82fc07");
            result.Add("th", "645888b328c6cafcccaf91372c9dcf356476a5cc37b521d6d2965bd4d5244cc74c05dc2f22ff89ed6856974d28fad14bb0afc85d18543a684f3e7d319d20a804");
            result.Add("tr", "17612646c1fee1b0809279bb8479021c40d374bc168adec826d57cac71073f6c3bea99189084f49609162bd8fad66e599aedea3043941bb00aeaccf20c095770");
            result.Add("uk", "0a9269963168f828aae518a6db80014674ea0f9f7ed56e2c4f459f3e7d4e0e7abde96a4218334af92b5cb5fedc795107db9a621ce80ccb87a6f1f0f6bfcb9acd");
            result.Add("uz", "6ac39834bf129c18268380586b4fec9eb230fbe78e8111e9e3028f3c70da7a19a97605c30e3911b85ff3c1cddc0e409c3cc778740e6df76440cd16c42c22ccfb");
            result.Add("vi", "a9210fba3f83f06e3397ce29ea0c2c14a030a033fe7a0855f013cb2c92b33afe56ebdd2baed6fae243d0a5c505f71f6fb5cd3d9474ff020fe9168dcb3bfb9ea8");
            result.Add("xh", "a002ae0b2ab610a340f7400c5f2deba689e86361b652e670e08c8969ef4bebec86452b690c5d1be61233e47d7aaa5c25442d1cbae726a3a7cfbee3770e4c485b");
            result.Add("zh-CN", "8efe3871513502da6fc6745de5e1fcd69b8a14f16b8d992975e45757c584b3856b8536e2631a3a2dc9a5afa55b58c263e4f0ad3f9afac39ca8d87cfc7eb4e4e8");
            result.Add("zh-TW", "a20db461c9e7c99c2157975d69530545a4a7946daa8db9e7ade8c476b1e40f52d401c1218bf50cf9c186c6f3d4b790d1c7cb1e2c42b9e3319073c13156c23185");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.5.2esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "c2a9b3ce37781dfcc9eaa6bea1b006dcbd2f1385bf363f723377e1d2d10b6810cd505412d9f7a5192f210fcb9786617b85f6f919b1587d5f0646b10399407d80");
            result.Add("af", "eb8a89f8ff732cf657a878130425bc9e4b5121b175cc124701dd6e7f5e780fc29a7da3f9f1f22a8005b2ee2aff4bd3c0805278f83e14444736336ffd3ae121b9");
            result.Add("an", "d433dee886c856ddffc8cbb0c42a572c8637fc1eed54fdbb1b037a25a6bbc6f1d0d18c9c14b4d92467b0d5d86fcaf11f653a166ac347f6f130f4adc70a172cf8");
            result.Add("ar", "fae596dba3968038bd66b99f97266b87b55e6044931ad7b31a4b709473470f53892ef3179a86aa984e292633c2eb143b095f5b0287d8d5a1c3f85782caffd870");
            result.Add("as", "5e128f9bef4b308ad7c7dc80812d9e7d981e6bd4c1de12b3225b3c8b88549b6c04509c20575d42a7900fdc1c17fb0f18f2de6677dced7e06584cd85092ac1f65");
            result.Add("ast", "76f544f7b9dd3763f8e4abd65ab37350768c5f57fa116e075f43ae345913a381389118939c91814a7212869b7a3e08c88a5ee69d0c1416fc88f8700434e9822b");
            result.Add("az", "172b5a93f4b852b6b164b5a1cadbcbc26b7becbd5e1b148973900c1ffa67aeb25cb2f2453dedf5b6f9e6ad241c5a785dc932d8f02e4af8776e33efcef02ffd88");
            result.Add("bg", "9ef150eb8c81decbbcdda58dd5f3fd6cbecb561a2642f7e3539a76d86698e6d74467e789f36e36faa8e567dd1e4db2d788b10e9af8b3c672a4ad3d9ee38abe92");
            result.Add("bn-BD", "6662ca8145babb67de631e12fc72eb589dd38e87f93dc2863204204ea33a311f992e305baf978527326d0b8e5d8c0334caecea67b67ba428ee0e72bda24e983b");
            result.Add("bn-IN", "376af4efffd4bde64af0b21b89925c157b161b5c33ba82e46c90983e664ea817720fe7149ff5bdda0804c27830fb9e51dfecec4b50df1992071d7e78761ba88c");
            result.Add("br", "e471abcc3a30cab53b02e056f9c3fb74c867988bef19bbead918cf5d39e2f444db291dd3545dc7176322f977587fe8404093bac255305afdec4f04ee7145af30");
            result.Add("bs", "2b966c8abd88aed1740198fd08341f7f63e19a26314ec0a88742ea24748711dd9c9ee22710a5e1f8b5c3cfd36ff38f9d26136988f25014940431a0acef6482c7");
            result.Add("ca", "f0ea4bf6017416aed068a07db56b7d78a7c2cbda381223a2fb561d0df054d3d4830af1d69eb5098db6dea488d3b47ffb995d7a9b738f0c7baadb94f718da8926");
            result.Add("cak", "9bf5a10841f67a1718834ac36ebcb30a9b5517945cbb7b599b57d629df847303d6d7fec5ff2c596f329653aef4fa4a723d803495ca69ffd9a3ff1e78a9c61bfe");
            result.Add("cs", "b0cb5b408718068b8f9c134aa4feaca77b0f7cade719bda68701320143b74d5deceeabeddc115aacf9a7d872c8004e65bd70f73d256c0dda62c21456aef52213");
            result.Add("cy", "4abf8117666aa6f13d7e07de1e79db96a7cafb1df82fbed178cb07f50f57c55f37a9b6cf87aa0fcb52ab88fd35ec30e0ea77df6d0b458b84b4952fc5f61f5657");
            result.Add("da", "c2155e591d5c67b93910811a51046ff501c27fb54db843fff84c2069b21b05163fa22628c725dd6f4f5c6dd6ee392e7c47d4220a4467fa9a125cc58bdf40ab91");
            result.Add("de", "cc742949d55049c0f87a7fb86cacb9659d5942bb786151b32917efda2c871a17c7413762a45073334c07a504b00ef91bc54924bdab53a2405a4a1235afb95315");
            result.Add("dsb", "1e90193819c27100e074241c19b849211b7d451b41d217dec1432e649904d6f8aab9bdb8acbe789fc9c4f5daa00c7a3b437a901f007c2603204e9125754a22e5");
            result.Add("el", "611449858905ae77ef6e6188841bbe19b380e1e3e545bb5bb3fba26f5a1385f7ad5de3c933fad22110f8680e10a421d6039e68546bf266959adf4b9ef1115ad7");
            result.Add("en-GB", "b22b58fac837805a62f812a3cbe5f31c38f0d89b39fba3a434e8a37b2f09ef6eb394365486ef7f0bf02e7f6b51f15b2213e72b6cba785b49c53de829d82df23d");
            result.Add("en-US", "22a19b547db237d3848b11f2cb6029d76ec7ca61cb1b709e1ceef756442c82caa24f0061e6d56c46849f7464de456bb116247e3f1b29ab3b87d9dbf77f130315");
            result.Add("en-ZA", "87aa782124309bc744749e61456fe2c307373fbe7b23fa77f4e19c8f1b1e0514aca2f12e734377aee3d8c66a5e3f1cd865844423d070f7dd335e42e7567b558c");
            result.Add("eo", "b61c15d4092abd6c9d62603f2c67b81aba31c61a9737e840104a5f8a9087f19e4abcff0cd6cd5bb7426f06b0605ddc5ee6169227ac8c0ae470a92fd40c757ebc");
            result.Add("es-AR", "1483cb50c9fc502db6693718a5d5094aa3f5d34eb5740019be0d03cd476f31aafcf54f819f5787f957fa0f88328904830c6d91528f586b2727454f5af717dac4");
            result.Add("es-CL", "f54bb547017cbc43aab5eaefcc51e9cb284f4bf187a340a5937b39d0edf8e6bebf0bfdd4824bbb0371a1c99e503a41be420ef1791328f34079e2a18bdb78afa8");
            result.Add("es-ES", "2fe74a98ce4a5eea15b4a35a3decb6e7b08b8074eab7e3ff2482aba4a460f113576ac1188f72b04cd7090cb1e22b096a41a2e32270647c8d947ec243b230f536");
            result.Add("es-MX", "454409517ca4d724cf767a02b295f727bf9ec0fa46650bf9ae0c7d6214feda4a2f8e5d61b5aee46f1a0fe3399b6065c47a3c7380bdc53a3b39c40904e196afb6");
            result.Add("et", "3c8ec30179d3577c6f0ee419a90e984b6be2b6f856d9292674dcd2c7e610e68a14cd6b7c6ad0e5a084bc3bb0c41679e30282115730f9d69039d42976ee2346cf");
            result.Add("eu", "0edd6f5ccc03e52e8b22b06d9494d3ec8524a8f5d0aa524a8d75df9223b1fc67fccad6f493446163d19604701844ff10af48f04b972becc70b69279ff1668eff");
            result.Add("fa", "faf67894e594a14c4de0f8f64c9dbb2dd3c5f8b3069fc04942a551d5ac031eaebdfad23e0fbbae621f8002f4e038b1c25c3910065a4965af1b9f0d0d761d83e3");
            result.Add("ff", "08c8d8aa7b9846a6803dc799c8353d9fea018073d4eac3b69b5d836d21fc44cb5135a21ab9973156b474a6bd616b34e26194d229d9bf99016463af62ebd447fe");
            result.Add("fi", "6b482fd565e165f2ce7fe790a1b4ea588e3525f3554341881c5070e50f86cfd18981699409ca71066150a425f717b61dd53b16b435dc4d7a9e1c696640680c0d");
            result.Add("fr", "1f7742aa75bf013a608de6bf077e3a79bf48bc64881406c857ad84989486ca38c280ee7452641ad548a20e9dfc0b43e6a35d7f4f4060d5a5ecc35477ddc19664");
            result.Add("fy-NL", "fe3b2fd1536e0a05ab3c4223cf0fabe5567f621e66dcf2b04baae3c8c9ee5810e3df595336f743dafbcb997127e4266a028826decec510d34af3cf4c1166e34c");
            result.Add("ga-IE", "daf282f47919befb73791e573207e5fdda27e6a99d027fd6a2d2133b42523c9ac8277b0dd764ecb640ba4830a8b1dcc02d25b53ba321f17072a40668e7c7aa0f");
            result.Add("gd", "cc99c04b99763dd139902c633a20a8560b8c8a8afb9d0500cb07749b65fa7f415aceff3dddf73f51aa3fbc8317709b99cb63d9c5412d69270e5739a95434bdaa");
            result.Add("gl", "6de7aa36e8f163b2384458fc67b172f5f30e1dd658dbe3cdff1068b0b7d7a39397c173a4b8376b72aa016dcf6d05cb719bf0a077431f9e6d7cd23d90c6d76c1e");
            result.Add("gn", "01e8b18fb85545a3b424ad25ce6a5dcf8ed2d2633bc889f4ec65e600d200c4d4f82a97c532408785107d319d3ee7da79edb358b2ade5c1222f048dc435afcbae");
            result.Add("gu-IN", "76cddbb0580aabbf5178994175b7cbd10b64de6003b6172c1fb2594d3a9e06f5aaca580a92c9569e279dbc87de0c9cfd46e81937dc62fb8206c062e27cd0c760");
            result.Add("he", "6b29c1884dbe1f0ff8290ba8fa438c2099d125d6dc9b39fb887177766a01a34a211386d1d3f052c3bdc1a649b5e12af2f6b8c4e66a6b4e586c266754825efdf9");
            result.Add("hi-IN", "593461c2b328cd8021d04dfbfd923559f6efc4a0f4ab903868f056da84ba079903fc8bdac6b6e72af30ef792c7061094fe9ced95b9803d06dc2d54a6b011a2b6");
            result.Add("hr", "d0948d3873dade08f3065a87e1dd6e262a469025efe74ff77de9e7c8861dea48573d8f75f89679df34dde3a4cdca5cc732d2e5e42ae989947c9c0002a0a68dae");
            result.Add("hsb", "59c680e8da6ac011651c8d6c59e9c02a99291abf72e2fdc0bdaaa09bbec80b0f38173bda738701bda3d3364775cd5d53c6a7298aa65c45f743fe37f9eb1ab284");
            result.Add("hu", "529fe5a7ac2fee390f452bb67fd0c45eee7dffe3fc96e963c7187cc8f3963005899a25d9cec86589084b3f8a329d942a98943a998717c0b15e05476a6db6ef8f");
            result.Add("hy-AM", "0334e374548e8e00d3bda67f6e54f2e11ac3e6532e90f7e905d8e97d8113876adab98a44f05e5e4d4a51aa6408012d0add344de900d902762b77aee0e6135926");
            result.Add("id", "ee85c1b395e8ce53bfe95bc0bdf8b6a2a4c501b81dc45d6755d5bd83c726686329d2cc0a974eddc45b521d89ea135891fcd534fbc7e0eea28898bca77f28ee59");
            result.Add("is", "021a072b1de87b5701f2d7ce0fb4cf224424c559125b49d6c16721045dee8cf27b3da57ae5400220671ae55748130582ca7dcc075192879924f198aad68568db");
            result.Add("it", "8b1c7051d193053a681fde09186d53731330c556d4cdf819b635701aa16c4a5a1dac6936996bf2ec11309201dc2579a2269631c344363b098780b17dae8d3fbd");
            result.Add("ja", "3a588ed2d38f02260f6e97cbadb648a40e627d39d16b76b9bd7a61cf71ba5072f4d75b6991a70053b01490d805935cafc9d62b92063b2f2eb1ae5fb39dd7461c");
            result.Add("ka", "d2776e1dcae7357f79ceed86677bc41a68e856fab87d848ae1c46f333219b4183c3bb4a05c359eeefb1575137866e28b7508bcdd47769f078ef41c828cb804d3");
            result.Add("kab", "5618c039a8b2f0e734bdf3326ad4cc6de6d888376fad657386635133aa9f5d1519198a771f943d4dd395c2eb11454b2460c715b9ac9ec65a0271d333afcd10ef");
            result.Add("kk", "71eb992a7cdf02de16122aef3e22c99fbca1e3816048044ce7fe60c8e0d87589f5592d53a95b8d9fc1c67cb477be12b89e7d13f99f360605500866755fbdcc65");
            result.Add("km", "ef0e5096abb1dadd55f183b5ffe49c4617bdb165c7d10ae981d853e7e97a6e17f36e7d284b833d97014014bd374f630aee6c26502086182cb86d79da1d8c6f06");
            result.Add("kn", "fc6fd70aba3e90e5d712ecbc489898abb8455486d52d940643d02e2aca7da23d2bb4d282f17dd5e9ebbb633606a6c0a15fb99a1f79918ecf67cf20042b93b115");
            result.Add("ko", "6943bbb4753f4278210fbae95b8c5aaab1d4a4ebfd20ef82862b76f4db6c90453001bef0d3af672968e495bab232687e647c704da8262d51907dfd2e76c81a40");
            result.Add("lij", "3a7bb79556179fbb680376d972435256550cf1529eb8f32e36a50e305134c9a12df8b0aee21c6e95e473f9044510c2e4ca29be6dfb1ca73e56a532b9deba4ac9");
            result.Add("lt", "c77cdbbc9ae9898c7dff68583fcc3c93d227fe23c5bc2e2f0f17bce178376b863b84d0c8e678caca7ecdaca22bc885ee3e084b3aca2493c7ea00361958252106");
            result.Add("lv", "da22cce20c57a33e51fb1f80069848ccc3bb7a0854dac52dbc790245eeab9082bf7c3dd6d48c694930afaff252561c9e893912277d5c37c258e39ba6ce419166");
            result.Add("mai", "a8811415f71daaee6722792e8d2f54f1c7458e6d4741b9bb3a138bcd7c9bf139d3be70a553fe7d632c23cd760538440a269bb810554ebe0c953c8016f9c759c5");
            result.Add("mk", "9562f2c90f256c4d84c166673d1866b40be3da619728937d09d59fbc4fd0fa8ca7f65a9e7d930a8b1258fdad5d2d54c30692f7b942b33769e1fb304b9af111ab");
            result.Add("ml", "c63113b15b85730cede37eacdc0b97bf3261958a435800d94d60d181d8383f2ab02d7b6f7760b6d1ef0abf508e12d94f8dc460da807bcec123008644bad8e562");
            result.Add("mr", "4d000fcb15463b69405197f68040238486648b0a04c9ad7ad4bb9e71df12f6d34aa13bced4e39047789fcf7e8359e9f5f1d20ebcdd12aa26e66599433da9656f");
            result.Add("ms", "7688bf11e5f8be1d7ec42b5bc84136f8228205b2961d371676187b1c4981a5e90f763a0f3e614a4ad2a14a4b1caad8867dc1f7ab3c7cf5777839f21302d1c267");
            result.Add("nb-NO", "bbbfe02782f5a5fd076d8112be260879ae616b12b1f6476ac612224a214f6f73b6f1ba47d73c4340336a4276b5154b87caecce909ea47531e42e669bc9f2da48");
            result.Add("nl", "98c56bdb190c44e41ccaabc87f1f3bcd5b5df739150e4cd97d0902b602512f7a91ba017852c5fcb4aa2360537be6c5237e8c28cf2bcfc77ef33fe3884778f6e2");
            result.Add("nn-NO", "e5e10d63825a86ad0500d9a6536089680bf8ecb3a18a64da1a3bc2a868ffa90fbc0c116ff2597715e0e7696983aff4c2d63d5e45c39f433109e5e0e07ea728ab");
            result.Add("or", "107d3acc28b1bef1d4666b8b9b75ade299264fb61bc5e050f5f19e3f3482dd7ab87cadb30ad8e1e3ec77db6de495b2400a8245adde191f7dbcf94b45260ebfb4");
            result.Add("pa-IN", "259b43beb61bd047fa3e32d921f04c54dbf967eef3205bd3cf682b0ef591ccb4499da7400eb07315f1b97510b95d10b7f1cb69a1d32df36af9346437b6cd10aa");
            result.Add("pl", "d1c0614c0026dfee07b91f9323f8ed72317c9dcd9a40bc4c23dbf6401c0934172bf2c355f316ba03fb264fd62a16986e35e3278845a8bd82f32e43409940f321");
            result.Add("pt-BR", "7bccca0e79430be7c39d4e4a0a0085f09d53019ca07897ae8ed707042eaecb4a7f0d0457d615109db841f2d87d5b2694ff66a5042f9ee162f3a14155c3d01f15");
            result.Add("pt-PT", "57e6ffb71dc4386da8bf2e3c80be4b437d3742590f9087b7ea10b950eb4f6ca811f295af9e3e1755dd36ff7f5304b4d901a86bdc9a59d9228f3dd0fd1aee6c26");
            result.Add("rm", "4820aad4b634c26bb7fd8dc2a683d8dde6f55958b8c346279b8452cce801ff66adb54af57c756c7193d65a92f773bf5ef1a3ada8885aa385420e7a0f97093121");
            result.Add("ro", "0640c52963d0c01a60e4751116183a961ab1f4d0a8bd66dcd391d80f0df640ee4eb7cd9f5ccf8d6762ffd7af7a38787d38ef46e8c5d6847bb7727c11d6462aea");
            result.Add("ru", "5e38ccb8a35706c446846575e109eff3bc648db33b367912a63612b86bc6a3cae77212d678865702439d88e4dfa6aca4d3d40cd55186ba8430ee52e668cd85ff");
            result.Add("si", "dd401d3198981a838ea46dea2d428c55fffb8e38c4a738b737d58e251a4fafef831586c6d1fc89799fade6ca57db7d668d1c0ec8587aa313b13fe3a491418a46");
            result.Add("sk", "ea245cfe2668e9bd65ed90359eb10538a0f0d94ebb35c50f8b88b42df27efad93e80c56c536de93a16c36f2b8e580b7be9d06f32fd37e84195f5e415dab07796");
            result.Add("sl", "6f93f5c016edb4cfd7aa3b9d695f368da421ca1d15eb89a4046fa5bc4edd8761e449608ec7e1038d80b33d536f0d2845ef6ff83f63a2e900e98647b5bbc8822b");
            result.Add("son", "ce80a4a047838db0d05f1b72725ae05125e38d1d776e95ded5796f3caa6a4821105ba2a12ae5a7929ae9d3ca76100e614c75694d519981f8b2a4ddd0277b3c52");
            result.Add("sq", "41033be82492affc5ab332c136ec9e5dcaa733f69ff3bea829a0aafde0fe8a0ead03c4a05d039543594b48dd2427fafc7473a634dd4a4efbd1b62a1e49b9d0fb");
            result.Add("sr", "0d04addbf9e923390e6c064df0d23aa3e31f29981f46135f2c7b9da92e0e41600c686879e58e055a045e52f85e6e657d4b4d0873821d530c948624e1ebe7ed79");
            result.Add("sv-SE", "2d5fbe3833674c004c1be81008bdfb0eca15e23b8b43a6e53e273625a6eaf336297c08e22dd8be79fad8f1f97c62c45d7b65f521bcc6c098dab73a0d036d28b9");
            result.Add("ta", "0c32200ef5094d496b80bbd31bf6c4c62a88bc85307647fdc0d8f2759c9c12848f34a8a8176032031b8c3d1f8faaf7741d1590834ece3fff97a6ad48343d906a");
            result.Add("te", "de6912dd02088f4a1dadc556cb43d8382085d1799b8e2f5ae36270ff93f965ff506df066eae09813a19ec3f67d786b378d308c56cd1e8bbc2b5cab651e86c68d");
            result.Add("th", "5c789203de47f7ca80b9687c3020a76287afe8e977446d7cf8e4ec30c741d3a804fbe54e6d72460c504dfbcce1548def933ef9741a4c1a4188b5fa89f627b9dd");
            result.Add("tr", "2f8a8b414430d86fcc4579b8e118553e93cf63d62ecb4e9ed25cf1cfdcc9722d2fdff3b1b158074c3520ccbb76c3b18cc5e931ae79498cfafc3b11832e772654");
            result.Add("uk", "93eb14e78a928b688b9f0ae012c8972024de47349cd5937ba2eb137d095ab7ecd9197e546f55eb7a068bae7f84e8a8279e5aeb280ad60733ebd2da8afb4a9093");
            result.Add("uz", "f820a388768347c800ffc11ccf27b085206fdc909df8c7b1cbbc20877326f74c0b8aa0cd1e7740e03c720251baa20a5b3c348b706f499e1f097f1a373ef5bc54");
            result.Add("vi", "ed032f25452a9bbe5e286697a2e5a69de282b4c65f130aaaf8abb4fe2053769e7a74302bb39e2e82195051d96536aec6d3c007af14a62099cdc4378cd1d52279");
            result.Add("xh", "9faab70f9661a50951161e5be16d9e3844541880d04816cfc5bd0aed0b983fb9cede8054868af0d7aa3057424696438bd72cb9c74835a3bab3a4bf66c4050099");
            result.Add("zh-CN", "9341d9a5eb31b743dc1ca5c36e98fc1ff62b4b851e9f8c74a1c7297d9df1341a8c299b2d64982b12e0623a4a91f81758e192dcfe8f5c6b2d9f6c29dd91209e40");
            result.Add("zh-TW", "e056c04117cfe0f8f702324070c47ef8ce688f6264d29bf4e0126ba984af85428f611d74a701a50a50f96fbcf4b960635b04ba32b4a3782c618bd00d3fd38437");

            return result;
        }


        /// <summary>
        /// gets an enumerable collection of valid language codes
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// gets the currently known information about the software
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            const string knownVersion = "52.5.2";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox")
                    );
        }


        /// <summary>
        /// list of IDs to identify the software
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// tries to find the newest version number of Firefox ESR
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
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
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// tries to get the checksums of the newer version
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit an 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } //using
            //look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            //look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// whether or not the method searchForNewer() is implemented
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// looks for newer versions of the software than the currently known version
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            //If versions match, we can return the current information.
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
            //replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// language code for the Firefox ESR version
        /// </summary>
        private string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private string checksum64Bit;
    } //class
} //namespace
