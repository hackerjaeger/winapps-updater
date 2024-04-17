﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "126.0b2";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "979ad51237d4dcbdc76a2e2c92b3e4dd6c517065199d7388619106e211ba42ad650330aa015a30b43226b861bb1892595d38c45543bf7ab6bf2f333f8a3d1de4" },
                { "af", "6dad8831930e21418f82067faba5ab452ac62d0e9a1473e85749349a2241a9513e08b237cffa4724e06a458c2db048939c5ffb601ad1b0fbee245d94dd228907" },
                { "an", "c59a399803fae481cef6bcf133f9e9a8728ffcf8598c4f8f6aa2b6a578896eb6fbbe2587c16a95a63a010b1171195df3c6b7ffbeb6c99d65461544e37e7f0d36" },
                { "ar", "83c14f0146991d0458b9ba1a7128e0b90bdc5af45ddc75c0b6fb1e4afc86a8acdfc4df2fc4e27c02ffca21254899d6873eea6bd228d085988625086434a46b5c" },
                { "ast", "fe2b93e005de92b681802244fb8e46893426de7a592947d1ba12eeb69d0687afcb1ab6c6ca8940ef9757c59be1670ba49f09630449e5a54eecd8c971b8d387cc" },
                { "az", "4af6f90d782a32b6be187abea7e2fd4fed039c38ee563f559a93ac44a6e3b88f11ef6f876d2435c20b9f2c58ebfe78e3b243f97f60162875eb959ced318cadfc" },
                { "be", "bcaf22aba1dd4fe32ddd6cd096ccb3f68f2e48b821d2e4ee9d490a64e4c25596f63d654426ae3a0223da5973f947c2c64ed391796cad8daeb31f0bd51b13173c" },
                { "bg", "4b33b435aa4159470eed24ce1991d32d9913e53caf4313909f17709bc5b26e70f4fc76bd16606830f2547b97e8895c665168d11d2122fb9674187aa92c48521b" },
                { "bn", "410f52470e334baaa449171bf16df2ac7922a7142d9f0f2dea5408be0a334f37d50314bdc2c1c6672776268ff785bf83819aa3570cb490114bddd3e9f748a900" },
                { "br", "baa42b0b0cdc4f0b548270dbc4d2ad2f643d1ef4462c1ba739fdf28e4466734c48147523343f3d6d097afeab6a2fb94362f16d78612a2adef2b732502aa17d15" },
                { "bs", "9f5e4c61538e168c00c6188106a11a00e2f32786ff6ca4e999a375a58ae27dbece5693ceeab24568d09b17830da413967e897cb31f61af1ec9ae33dd2ddbb99a" },
                { "ca", "1d7107c42c91721ea7228c4c1eef15b2ed2506fb8e1e96592778a343a86fa1749ca484c9d474cd6706f8de24fd2eafc4d643420c90a94bb655b903c27873e121" },
                { "cak", "c4ef0fe0cd3b4891b1cc2ff18aa126ab5e53ae737845bab46b9ff465dd5c5a95d4ed4954f61ed0b63a77dbc81a7bdeec60e7fab74997190c4af132adeb1a8b16" },
                { "cs", "ec7b4c23d63912486b015d51219ee67fa19d5097f488a2f812885247e8f593fa45d55914e2e7001f373ec63d6bce404c00856520e9a44e9279164d26055c46c2" },
                { "cy", "b4bb231963d921c1766eb78acf6be89b0b05659b266d3e28d733c0329e9e2ad05a870ae0fd37e1f3f488d42287880c73de346c2556714baa9543872fb03a0ee4" },
                { "da", "867648461ae11dd969a20746246d2f20476f33dc36a78e59613a5f1f8fe171f15fcea6d57301c8f9b49a9f1b00cf0ff5e8a5008025e460bd15561c617cc1d333" },
                { "de", "f6afa81344f2e5e63e205eb29f9628faf1e54c8de67b7cf05033b2a7e44f4d6771da56b9aa9b7c543cf30a01da59c704b55a754bbb8952b68106a8066e97119f" },
                { "dsb", "04251f7b7c8d30ccdf8c5f87b8807d884d6b5c1b287f0354a9f330794947a59e0d69cdda8175a676de3599423ad0889b37daf1e72534c7fce0012f8944bc88c3" },
                { "el", "86f02c3de98c431334299a934d8ec263f52acf380b3cb581318f51b3a19bb3b09e54758eb270bb26d3b838379bd8aa2e0db8638521d2e6ab7394b60fdab72dc0" },
                { "en-CA", "47d3474bf6a37e337933e27276cb2981fd88dff15a1ff4c286aa90757d926cdaf081f5babecccfb1412c3811e20c944c29d631f07758cc434322e20f0b6a7dbf" },
                { "en-GB", "4f72af02854d4cca97b3a0590903e4c95017e8cd8bd1e99e0f8c3463887c283c4ee19bd3d97006984364eeca4899a1417b5acad4b09a2eaba2bf0d882124dd93" },
                { "en-US", "c33ecd17c03e0087e77c7473d2dc2a0a19b526e5a1f46f68f7ba814c5710381426ca0f2e72444b6e7806e8aaddbda5436037303792bd883e4973d0cfde905728" },
                { "eo", "decb4cd1ebb3472952d03e432a437ff84e53a26bbec0d413ccf375b50de3c3a19b991480d678b5a063c82e45e51a4b4d37fd7e33c690dd8a08f4c379f198c1ca" },
                { "es-AR", "5924f381cd169dc48daf86bbadf2bad5ddba2887be3edebb7adcef25da094f58256034d22c8ca9c8e91cfb43664f7ca90a0f8a530fc5d0c64411d59fc3b689e1" },
                { "es-CL", "5868b69ba89fbb78671c983cc58d08af523f875911cf2b2d38701a9cfeb4c709d5a1eb350ca5c061cbb28202f5b18fe56491989606167300e47dbc0a485208fe" },
                { "es-ES", "19cce28e6d9a8f351b492c1c0bfa8164498e592bd943383c3006a45b28c8426d1c8aa753f96a103c6f6b42bf7334573d36275ae6241a9960a54c46ab218b55f4" },
                { "es-MX", "982c3aabbc1421c60a499967e93921a8ea9befccf703738993cdc88e1e48d8d817ec2d47eeee20cd9870273840be15cbe4c506334accabb6ed6298c8ed2826ed" },
                { "et", "871f981781a5f6783fa99eb3005394fccbc5b3bfe1af11d65df58a4e0d05cd06cc263a433909924ce7824647402738587da289f4ba99b6a0039865fdc3e5f219" },
                { "eu", "7958873afdba77d5f961acb9a37c9f801495fa053ae5b10cf82c01e5c6ba4c3172bf6ae9a4cef0af680e6aa4806f8edfd7d915fe7ac2b1add397174cbd9628e2" },
                { "fa", "7415451a15910879c68f5993085ddc07bbea8ae795ed1d35c9514954cf62f2653dd41af45e303ff0ec44c56fb345026d965f0e43b03758642566bcdf665481b1" },
                { "ff", "3453d343de3c52ae6b2fd86bb080946af4f378b3602a9036a53af70972c2e169e012cf3acb4c3740bf027fada068e9be7f252121623028ce2b5ad9b12a514807" },
                { "fi", "9889063e9691a4fe46e22a02d711cb8ec98a7dc817a81b19cbd9b3413ebf9309d9c593cd8d8d1da157730eaeebe60d356e14e0aa00390dbb96c7f8725faf27a9" },
                { "fr", "e66b161e7aa82391f6af5f24a1427f259ba483ad1d07ef23f974504f63269b509021f4ea350e99ab86d4fc7f08dbf3ec32ebfd267ffcdc91b2e81ea8ad574402" },
                { "fur", "50f5baf533a3a0812f3d9bbb71756851f686329a42b23a6bb9b5d6567a99e4b81cc9fe11ef071ffef2ad1bd7c14ae12f69f242b885d9723369d09073843a98a0" },
                { "fy-NL", "61ad482329e7e59477900ce5f2c3351dc7311cc4ae8941e0e669a9aca45cb2641d1c4d5a73ddd028dc5311628880bc44f5e2f3bdd3a75806be627869f9a4aead" },
                { "ga-IE", "90a7ad3fefa92e8b4a271077f5ece0cfea1d1c1a1ae73e416f56607d5bdc43bb2b59bfb97fca2bd29bc01bd6874123534869e0587cb4dcef9177930025fd9106" },
                { "gd", "87ff22076018f3eedad71fe32500a6adde16205cd5dc1bddbb7f50c9ab730ba97e1ca5eb1ed6e085aa699cb7d303d3def3d0d67ffe3e3c4cfdc851075d59faef" },
                { "gl", "c2270213732768e3396f1eb7c783320a4c55b2e4ee1af8827a0171c63d8cb56bbe35d8d9399a0bb592f2772071082a2fac5e18bfc970348fb67597cc971e67c5" },
                { "gn", "8ab48ddb3295395434507d5b95429fcbc889cbd784b12789dd8f287114f041202fc543a8fe73dfccb8322d8ab3ff8b79d0da97e3dee06edb677ee8737c6cb86a" },
                { "gu-IN", "5720ed785586d2e680092636a601759d655e100de25965d9b927da33d0d02668c485906b69619d27e60738938b7cc53161475563f479bce6b8f021ea1a3a1906" },
                { "he", "bc1d5b438da5735ddb7c90a0a190f1dc74fd39bc8befd7444ccbb69cd9e97c26372680e8adfaae5a758eacb242fe54a27ead3036c8e03983d4cc9c85f923b4f9" },
                { "hi-IN", "22b97a50f2316fa62ae5c3c8936377a786c64e43995cfa2ee81a8a37e81a7c660291d679879f9eacdce683d247fc413d3d35e9b6c796e7bc551b95d061f0582b" },
                { "hr", "9c07d5bbd0f9c68132cce5383825818111bbe0326a5772568811455719b2adbb4e764f73422f68152efef131bb7dc945c8f9e5e927571acf18a004202bc401d6" },
                { "hsb", "3a8275422a954213fd0699e19bb9e99c5d739279c35fc77db6499d0f4d5f76ee5fd80b595229baabe67be2af0a8e7e1d73cc35f10b8260e84b3a035d29367564" },
                { "hu", "dc0536992fa0423763388d34aa34d1c06dc7c06781059c4fbea6185d5e4e8d25122ed33ce8d1c84fc5905f90d6dc03deacd3ca02dbd10d56f26f538ecb58712d" },
                { "hy-AM", "ef8b4e7d6d35560f6a75c4b3d3bb09c9a5845a9a624b1db0f62717381efa45cc67f9a681c4fd77bcc928f418d336890a09a289d0de560635c4ae863d718d455b" },
                { "ia", "16a816bdef0f2d3b704b277bc2490ca41cc2d5cee9b7f67f0aacae113fb347ef71f1a76295d45d7eb93c19679e640762415ed12d567aa6078f3a233ae063ee6c" },
                { "id", "9c6bf9f64a7dd8f119500d80ee8f76f7568758e11cfc497cae228baeaa5efaae7c1d4abf0e626b6fc80c828d1c6d8ffb7be244468f5c0bbcafd8f563b68486c1" },
                { "is", "ab2fd9e609046371c33750b5055566dc9f6e22df7b5787d2dcb3c9f1b3929542c772b84ea4593f7640b09fab6d6baf40731c9d02d4be3a340751b2b3a954063a" },
                { "it", "a8de41ad7bbc6ef1f0b2c3d8389e8563ef19f33706185f7cb6ea20da4cb01d0f4d641e15ccbce38feff9c927ec5bd9e6404234543b13e56833a06e684080fc9e" },
                { "ja", "847a1601b706391f162857fb727cf821b8189715233bd5bea0b824287a4e7d4b37a816865b848e4795a912bead653488b4220eb8ba91888f0129f43c7794de53" },
                { "ka", "99408a3f0ff2e01960f70ccd54deae887d64e1286e47ed7c62518611b91a80313932336d48439c9c5aeff1f0fdee482629aa260b9088ff073f1f1361a130f683" },
                { "kab", "cd35bafa57e8761f85cc4490bbe72a0a5d43bf774f5d873161e2747f084349a00b9bfbcea186ea4cf89f8d12af4545b5892ebadb5fdcecbd8d25d022e4d79bf1" },
                { "kk", "6341af88fc4b883dd6a2eb3a7c23fe1761b5c7a56f7140fd72109566607ddb2301a7643c9ce79784eb8d7a9d99b1ebd4e532383550cf2918cd33a1570aec6d7b" },
                { "km", "1460b1c6125dc43ec7e158077e4958e7784c18809096a71c2561b58ec65f74855e9fb7d5feac4f8b9fdb803bf883c0808499490935a8cd10aaf5f1844aaeb19d" },
                { "kn", "04d9be894b2ed4e7456b66f9b8e57e125552d5d353a1c992b3746a59f1616567dd95760530a67f9693ef02a4dca8f771e05804dddf68f5e64ca6f5dce1064208" },
                { "ko", "38b8bd4244ab9f5ee2b28241b7d2cd218bdb531146a38010545feab3ca905f918f99165214a0dd68f6922faea0fe6b3193055af4b5ae4893ba65983364a002d7" },
                { "lij", "13dc8070c34e643d027258928de1160d7083d6a8a62245b27c5ef4ef5091a9ac0f9b7e56e2821f2ac1d17e285099a636f4505e1fe92bfc05ad486a56c87e6285" },
                { "lt", "00dd4789c450963bde0acd4e2f4bea01cf277d5429edf10026ff6c351b96c5bd5cce601275193bceeb6eb95bfabbb1a625657de2b344063eaa95308852f6e1c8" },
                { "lv", "4388a2482f8947f0c8da1f26c35813cbec27280e31a077c12fd26938db9c3667973a2548edfa3492f994276316bf7b3f6eb47a6920d6442e215f556a61e1ea6d" },
                { "mk", "e1114374732c8b48e6f6a50d2c84fb74e5949c6f788cd9f27b823f35ed91486d8346e2d32b34a4706f05231bae5ed3fd0eab59fa39b17f50114b444b050e87e3" },
                { "mr", "014d58d5bd445f6b4eba9c2704924c5b2db5cc594ccfbb09e62197411ed14cebf228de2b3a12c5a3eed5c73fbc033c2a7e1b61c34cd6348f7cda0e18fb031b55" },
                { "ms", "e5166cef14923647ec577302723723d8501014d10dfeec89f9b14763b4e6469334d90ae5c7a7ef926443ec652e376eebd717f96717d13c8032b4eb77f0defc0f" },
                { "my", "b6f55fc857d5dc9444e39d621a0b422ce649faa514638d10808baeafaa0f2fe420395feaa209792c87139c79862cc303d024c8bfc40c4a60ae80fc84633140af" },
                { "nb-NO", "f0f72dd77008942b048982ad4f86c8a721f26c9deee4c6698c35593387e1269e075262b78a63ae56914f8a28711145af347b7ec240be4b4cb7be16d9d06ce37e" },
                { "ne-NP", "badbdf398e8750c053c951fb3eb7977bb255a6111c0fb9ac9d51a80eaeec467345329d01d3799e5052f57a221e1e48cabe851373570d48fee615f949df48e8c6" },
                { "nl", "4ea3c9fc831fc40d508fc5b36fc84ea3ae570428ebd80c2ddc06d1babeff4961703d984d6c8e2f1d55dc6b78434d897386ee6a2add7cd989b335c3b65576aa15" },
                { "nn-NO", "cd7867f299608b5129c28870748a8584d026c41686ca3aa5617ba24562e7cb91025b5893c8b9a78a5eb5d71cc311e51023ef000ec8387a3cc2442b5c652848a0" },
                { "oc", "f376445e569e55184a3f34e8fa4368917509a079a9fc5b1deace22b81371851c07033d2a5f8e5233f8362e20ae8fcbcd50dadc8f4f7cc1070f81c16e2d56cc7d" },
                { "pa-IN", "469227bbf98079f8eb88bc4bf0d778da9156a37451c9362e602e28b06ac7138aa032351d56889fde8bbdb4f1b131927d1ad5fa7502d856f2d764abbfac4e1c03" },
                { "pl", "bb13f7567ce8fcb6ceb0a33e7406223ac5cbf859636835dd42f7a924b53f8c73976a631fadf149768bea62c1eb8e70d3b0dd14c9039993ce6c4515783f42692d" },
                { "pt-BR", "4d01ec3373db20c2ea5fa9aaa4f87d91842e70fee031cbdfcc2665a7b8922363d30556b14c4a06f31574f063d6fcddf583c8db858b6ace70287799962e78fc20" },
                { "pt-PT", "0ee51afa1901c82eb65273554acef7fb84358fd8149fbb29c2003711698cb0513206b23367fc671405982b1f537329537b4fd5b792a64160d0608cf6f2edf717" },
                { "rm", "4eb76b9ef70c6b7ac2c2dbaeea15e81d3518d6e51f102c323f5ceb22c87ec75fc3b8e0235216b62b086937d36c8baea4eceb868eeb3fd5ef529ad1c57e30f9a4" },
                { "ro", "8a64992c59277b39835682a7500758c311bb3dc7499c40546689da96d77534043a87e6fde7c4d4ebb398fd3097ec9282c5677739598e251cde40ce410f988814" },
                { "ru", "bff3e2b7a53fda7ac2b8f2f0084383947f414b21603be3a8dfa50deb81b52fde0850ddef461d6ac618054b265c144fede3c2f56f0b9900ba3483f5946ac3c8c9" },
                { "sat", "d53c9e7dca42e6697f34f833cdfd7c90f76e129787545cbfd2d40f722838f5f4935ace3fd7afacadc306968943a7bd299fb1e082f9b99dfc02f54a45f40b7563" },
                { "sc", "be028f703db6da533986eb1b4aa5040ecaf69ee56f9c87d8de460d550df0b19a8c8dc5a4200a0c2fa01007340feb15289d7f4dd8901262fc699d9b53abb1bd49" },
                { "sco", "0947d5cf42f94949b94e7dd70be32971cc0f26113f18702d64183caccce9ffd9685a84bf2def2a86aa694ea0c40abc9311135101d7b9dd53b17acc292bf43727" },
                { "si", "508688ce08acbdd438414a6b55534322c9e3f8f983109dc6040d188114018aba23c9fad38cc3e35171497229ec2e9f33be8574f2f595cda494cff1eed4c27dc5" },
                { "sk", "7b4d43a4b162af7bb80d5147dbb902278e7c728e410f4782c05cca81cd4d52f07b4cc5f2738f5a3cbd75e4d79a49d5e11076dd7c550c96d114b25778f6d6e5eb" },
                { "sl", "5fcecd466ce15c00751c34335d16db5002c146c7ecda40bbf4e22d0d784be69bea212dbf93d41d3798be6a7e285de6ff49bf0996229f2724189bdcd9d95a0693" },
                { "son", "064ce6fc172d1d1603d77dc150e9533a94c39b486aff2f8616f4f0ecf15567587deaeb216346469d2f47c07329996ae2e6dba21eb749f74669f098edd10e383b" },
                { "sq", "44583865decea902727198904edf815f3c1615a484547ea59132524060bc3d89d81a59d1150f129b859411a60e92dcf441ca92d39d08f86636e15b60132eb4ff" },
                { "sr", "8c72ce98f1973cc044a3e2e92453f6ad1100ed425f73e9073d6a70b58c0a841cf6919964c263b73447855e2885159615f830e158cbffc6d864a3b791b47391d1" },
                { "sv-SE", "0f1ee4bfe161899db4c39c7e9d911dd257ce83228e0659f206aaa9934c0663c3b4b0001050fffb64b582ff5ca208b02851d0c6c740042c2b18726bd72ac9128f" },
                { "szl", "ebb29a7667771efb1e100ce3d61c64d27a2fdd5781d6363f841fbef1527cf4264ae734f7ee815bffebf6d69a8d6c9c93350132140d39f3c7fc26a58c0e98cb84" },
                { "ta", "c5cfcd8a7fe0ddf5b3428fe31319eda70402cf3b6456c28ae1511bac3947a2aaf309838152a9ab6c87c1ee0187768f6f67ac2ce43f803e6f706900660a7605b5" },
                { "te", "edadfa5f356b677ed7c667c37ce19506b1ab6a3ad0805bb53d3ef23abbc7d1b6e5a31f1b48ab18221ee44b844d31928a618bd4a922d1792c103df31eda836bc2" },
                { "tg", "f62a9603f1e2e08f37adcb2228f3ff9de54a84e3ee5aef815113e4201be716dd7aeb54b08de1172e23121915e441d9692f95d543f7fd0b0ff71af1057dbb9fd1" },
                { "th", "d2083ff01408b3c7a56ddbabd87d21d63fcf8913513ea23bfe8649af1b98ce5e1242e36d44fbb59fb69a0d00828104e8bcff1b828113f81538c68d86d1793383" },
                { "tl", "896fb021da1b4d96ba5d391522c51de393ab4e7852317878a9380cda9669c33788e73a151798242182dd8d14d125c2905ba473782cf56ec95186c1df1f0895a1" },
                { "tr", "b8c766dc2b5b7fcdeb949cb17eb67a297041336b9eb75969be50bda8b3efc62fdaf4e275a4e802d89f783c2e402d362541b58c8956392d3a41d166a17eaaff63" },
                { "trs", "f1e03e0ac383f109a4d953967287c9be8381d88d07bb8e0cd49b3a7597b268e329956e4eaf815b4be5c5402253cf72d519be1cb83e498dfa929e94f4bab88ffe" },
                { "uk", "bcdf82b70952fb77b4a6bdbf3f8f6eaaa73103241cbad680f7f4a491a7f12605b582d066bc9aac52b49843c99be9737e5d4bc00d6ca30fce17c9c9fb2c8309df" },
                { "ur", "c1b635f1b71404f7078721bcbde67e34b143dca5e724aea3b84e9023d473c03a20e7c84f0862e51b04c8e45509e8f20efccbf83808785004da5c1a786f201bba" },
                { "uz", "9b6ec7a6287a20d641fb6e57433d1c624dd7e49e67698590089e980dca94a7d7320c604d54e8590cf458e0ed923e6e091dfe50f4d647a72597745a512b2ec973" },
                { "vi", "525d4b10884f0f79040c5bdbc99be4ae1b2378170ce87bdcfe644d0bac7ffb4662fe43edbd6f90ecdd5b6a466de7814efad782928991533164d228365b4c0d7a" },
                { "xh", "d96341caf3e039427db1409dfa0429f2d1f005e22a9a34110bea940964d357dff704605f437bcd65334a19251f8089cfc36764bca1ecaa8883a8326f0c3f8040" },
                { "zh-CN", "9ca159ee37bcaab08ef6ce2579e96cc92fe5d0bc5ac4be073bd32e2149280cff54bb91e054d2e4bf80409f59b4f7bec4c56e997f2c82b4326ff26f1b74ea3f41" },
                { "zh-TW", "a5c10cd52a8edf82e429d00fc34584f845392871044416e516b94d92289953f444428a1355128a8738b7634cb564d71e313c7bb7011b6fd8f5c14bf5a1fa78e2" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b2/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "fedc95c58d12a7b5b156a6d53ed7184857c75ab207ac44e812cee1a105b29c68ad4e015839bb63d4e180745edd0fc5438a80b50e638987fc1f859ff6d8dc56a0" },
                { "af", "e1f539fab0e8366b4e0d013d522f795496eed202bb82633676d8fd94531e2362105dd4ab2669fc105dfa5e017844d0b5999ec1683149b90e32836470d6bd2ce9" },
                { "an", "c91f771b087f2fe9696607b4a5cf230bb0d787c5a70a60260b34a4522d7bd205990c84571875b4b2c58fe589acd389957472a7612fb9ba5751116b5cc6d9f8ba" },
                { "ar", "b670a1f9d855bdee29b47f6e2419420d623daeed82f67c1a737a7ae090ecdc9bb86bbe20367e828f627d51a43f0014d359d62a4ad4bde8ff58b5dccf62c63e48" },
                { "ast", "949edf9bbbfb7b73bc04df4a7241392ed1c9ec75113e111c0bf5ba2332903280f0701edea7f88b648870b3c95440f960436405c568e47b818e5e7b3469f1ab96" },
                { "az", "d2c4743c95cba6d8f95c16922cddfadfd0dfee0783cc5d6f4e1d3287ffd8a96f315acd98bebfb135567785ce88654e6454bb8625592dc291f33829f32fea6afd" },
                { "be", "43680e98f0a9acd4e949584b752252ce0d542519c3bdb8a234b7b3d6715735452a5d8a5d399364d3cc5ef15a263c328ed6c7d768d7ea436f3419fc798e524eac" },
                { "bg", "dfb8dd7a959705342677ae6678cb08f6ea05dd287dc81183b3861ea2a02154a5452a2345b7232e23785284b582e23403227b9d9449a1e0fecba4e50da7725dd1" },
                { "bn", "abba314cdedd026905d3fe5282b0d276b8c98b1935749a2b1162dd43e4104dbd66f432e7630ca650fc05d3e9407799cdb940307f34f36523f174f08496ff556a" },
                { "br", "2186100a4d339a7aaaeae35809e32055bf63199ffda7d8de14650ec6a12b2f7e783b8a080824bd452424d2b63625e10d3aac614714e341a3b0f5c47262408608" },
                { "bs", "26b179604c7de9fd6caedee5a9a23ecbd673d1b1a12ecfe8abd67b6a9c4a4ffe777dc0adabaca17284f4902fe19229e674f3a6ee1fb9c5b0055d88adf220c6c7" },
                { "ca", "405d06da3d1e37344502d3535882aa4c8c69f2898bfb0faff3abdbd58dde71b6c6e8f961161b0a72fba4b647d128c2b287be7df91179aa38bb3388f3d8702199" },
                { "cak", "cd3be5ae61762e727f4e8b78821eebf1b10e703f17f7bd55db67d7201b0728df26efa4608ecabef833822e90907b1bd6e9cd78ce10a1b28db764594f047bef93" },
                { "cs", "55237e3e60758b0da93ec069d5e8f75136fdd41ad95b330c0070b919205f5457c6377aec8f82d411877b11d51e1146f146f7f27809b2258b2122303c841c2c86" },
                { "cy", "e6a1e8d3d3e2d6e3462318cf119754f2c1ed2f95703869976941c13e80bcfaec032ccc05784d1732f9acb137733524ff02bee4fecbdbecefbfb5b1df511439dd" },
                { "da", "600d9e539118b51794928b56f2e45d1d4585619231e9118077f7fb90b5ac4420a7b80a19da852fd97ae9f16592a727b54a51b2ba201ee2b7d8b16c90d35af6b5" },
                { "de", "1b913f3d9a0158da1e85864f3cbc40c240aad4483a3b536c442d6906903542de8c4584df157ad98a5298e51e065bfa2544eeffbaaa519a3a22c63792395f7c6f" },
                { "dsb", "18fb24a46d36a73e1299b0ea0cb88aa3eb79c86da139c48adb8c1501fbd0dfb9823c52edd19d66436bd921046390542d6906034037098f54dabf5b5c48a22cbc" },
                { "el", "82dc37f593cceea54fc57641e69b3086573bc527b28ac58124fb1a7e25eb2e729af6cc35cf692914d23502ee05ce5d4bc3b6a8de6a29f5dfb94dfd5313bb38ff" },
                { "en-CA", "451b8ee1b4ed7ac13c913a688f38497ee2d4b9555bdcf007456ae3d6cef96b4048b3ebae3728ba513a0307a2ff870f08805091505f4b0522a353fa54649f56e1" },
                { "en-GB", "44bb7ffeb350365c4b8b56d019ccda9b9366533a9a3a38e895c368fe7ab7f79951486c57e770ec4171d3ea9f7b540cbfa219557e35f0b3319a7cd7912f8dfed1" },
                { "en-US", "2987cb6ddab9405d36fd585e0fbaf460585c3192e1f0af423884b30f56cdde97acfd83631cbbe14976ba6e8a511cf94b20a6348219448c570623bd5f2cb72c40" },
                { "eo", "fc42379a86d93e867199e44f59a4356bd3be1a452856df6a5c0ac7840ea0753f69512914c9e2c4d803bf10c87ceacd1af1fd2423744a3a565ab841d892d8c594" },
                { "es-AR", "0bcc3bc7198bb8897ab9f8d63dcf12dd0e7e2ba198fa78509f492a667f73fc6f278f89d0b7dfd5c6a4af4cbbfd3e72118e731e2b2dd383b458ef92484c7984a3" },
                { "es-CL", "c198aee1f53f35907979aa05fa81a9b13ed2456aba94a4fa61e93c603082c02785f8228c3a0876eed592177cb8686fa27f4e8c4e4fcffab283627c55a6298872" },
                { "es-ES", "ebeee000f741e0202320942eae8da8a581835f980f76fcd6bc0e5ab814958c034e4886cb2534f416083d4bf03524b17adf78cb7691794fac0562f6186373bfef" },
                { "es-MX", "deb969691116d1e530b0dbdb426bb563f178538169659c0526ffd447a386eed2e42ace7ba9a47e0b43a228cb8410b3a6dd0bd2a1de8d17bf5fc23b2e7c3e457c" },
                { "et", "c21b7b3fd4550ba8c04c262fd4f37e47c1927bb2775a811b107a8939d41f428b72054a2bf000674b53eae917c935a7d953d0a253ff4f702fd6c4cf8e15b3af23" },
                { "eu", "06c6bfbd81bca78adce20ca0f101329bafcbabe3bb8ae50280bc2689d91b409a5a6b9986c5082fe9762daeaaffead565131884c162c6a4a0989c58b2cb0f457a" },
                { "fa", "ea5619b579aa3285ed5d08265247b16833056e8d4bb33bfda319609bc1b67f0aeef159c78f551612d66eb578d5c64c795452eeaa28d060efe8b8ac2a1e37f6d7" },
                { "ff", "581cb31064a7e1d72bf84432426cf8784c02572f48ae4a1eabad9072179b1efef1410f82a01de4f721153e87ec18da336dc57f97afe6278d195473e9feb069d8" },
                { "fi", "403654c0266929374ac08d27563c19e00b75cd4218c3dbba9373babc19b887e308a1343549ad47fd876d29da37978a08b62b03771e299de59a07546e4c6c9c4b" },
                { "fr", "aa82514b9b09672a3017a8bc90903295d8c4e397cd137a10ee5d1bdda57c04c98d60861688ecd7c84a5e7c69aabba57ec41fc5e8db9046a115e45414f139f420" },
                { "fur", "c2b856044a5317e51cdebd41a2a087e0509444a45208309ca4d6bd506d35ee43c2cf562d78828839529d3528670cd946869159dc52bd07ced6b06ee378c62ed1" },
                { "fy-NL", "196cee0b35e608169069d6bb176d941c5a89a2e1f51bee5827cc93a9481562d60000467bb8e1c2dc168e052cfa4aad59847a4bc6f13f04a02a2223c5cb6ff706" },
                { "ga-IE", "ae789a7a790d5a78d46b0ef6b280cf42d585b9fa10d2e3a786ddee76b54928429e139d45a2e2175a0ee0ae2c078e31636e8d07bf21a5e9d742d1a59b8e6282c6" },
                { "gd", "b6624b7071b7833d4b6d15e248945614868ca2952807499118552838fee77be9c1909978da232450a4b72205ad6edddc7346416d2519cda1a2dfaa17a302a348" },
                { "gl", "0eafdf1f9fe72864d8227e2dd60038dc273f18d9cb55a08b75630687dc2d8e6d883cc356f1aad1565c63b0d802cdcfd85f20493d52e853ee2d12d6e6ff3288b1" },
                { "gn", "d77ecba52e23ba464e6bcb7381cdf6efb1b5334cd405f23b019899b618585e93a5ead59493aa5fa54e9c4c73c0aa8843828748c53aff04e0acb83df89f03d0e7" },
                { "gu-IN", "12b5f9c9e76b7f3a6ef6f1cc23bf1d0f486f44ebd7da722f618c2cec54b38cd6b505302d0d11ceb636a5c279f5e890f4164d3128575e561c1ac917518ccee841" },
                { "he", "76cca3a1be8b505a5e75a745f5fddc5ed5e1a3a4b39c2f88cc106de28c685144104f4b53876db10d6ccd8e59be6a9028e42560ace31899546f70453682008148" },
                { "hi-IN", "1367fa0a47dbab1d1dff8bdd5afab95fee5042dde9a267871a08685776dc3c9c33793fad47f655ecae95f2c5278bffd6a96100346a92db4b7db90de710601c13" },
                { "hr", "756b4d10c62051e0ea59b650334590847f7c88fec19dec6e255f78d9fa56c4100a4ccf315269a52a19a4788b2e43086f9b9ade46262b2496075b0b6a73468da5" },
                { "hsb", "12d904736a39b9190d1f6c4769f146168d25834878d388f8844c3cf6d59ad8dac558a95f7270ce9d6bfd55f634056354684c65a64d257d4dea097028b87660d8" },
                { "hu", "22aee5cf49d44ee9a6f09c1110b43404c7c6a891020606258155327e448b4904365e35d144514c2c5a2eea7516bdd5fd3e9825b73fc363245fbf8a8eee760eaa" },
                { "hy-AM", "a88cddd127a6646e6c89cdff7b50b207b2d9d9b9f6fed9280a3bcbe9472dc54fb3b7badb9558b5d22139f2f5a3ee658b9abfc6522b70aed0770e5881a07bc800" },
                { "ia", "a5382bca7322c5bfa8e43c86d2832ca65d8360eaadf2df3758ae87c4914c10721d3ace4795da5c320738fea612749e7cf64471283eefafa7dd3cd9a3d991c358" },
                { "id", "49ba6b99e11a3b6ca74faa0f14e66d4af855ac132126808d210a54ca2c0038864e8d40f0a94c4d15477726d3e3be03db7031d9121b5c0cda6748c2908d4a4006" },
                { "is", "973afa56a3589266867e636676f31ca1b9fb84e19af8eeb95c7f3da902a860d1e4b5eb078703f63285d631e79aeba9ac5313c2d9b658f9478506c3dc4c3cd7ae" },
                { "it", "54571c4a06957365805a4da505ab7382c71f2093c0790b5ee9b638a9192967754fa4f32995eb51f59695f86ff9bae6ceb0d2aaa0a66c8d7bae021083792f93bc" },
                { "ja", "1b450e1898b89f6ee4e25bb91d4774b462ae7b3862f7132ef8ba65c9652b47596a191a1ea23d3238d928edccf4d93f33264d965b82d7560d4c227d1db617fb27" },
                { "ka", "20d98dd26813602df8db7aba01ee0d4b747deefc645757b496fc07574202f0f47139a448305c1b822734191c3581afff01e28bbed9d989b896a91dd58693dd9b" },
                { "kab", "59f94223686d504f948acdf89ace154ff49e795aff7688ffdd91784a5b66c3b2a2c1012e971d4ff711b095aee71c618993011f7080f94798fef49d1aeea26a9f" },
                { "kk", "32d6e761812fb711cead06bb72eee8a54f5f3eff9fc0b8210f461483125d5080874b93b76098246028870129a6f1fbfaa93ef2e608fb985fc9feccfb15f46bdb" },
                { "km", "ef610ceea68fed5fd12e1d1e898df42a7e149ebf0ab5f3b58325ab71d89953bd195ea5a56a2321a4b7e0c41b9751b27c2bf119bbcbcb90af028154349c1b700f" },
                { "kn", "1b872710f82b4f65e36f2a9de79df90f37f857d1f9e890aa76e318e07aad6492337adc7aad5a807d86ed70a67feb4db778b70286406663df936330abefece38f" },
                { "ko", "4bc32206b7b1f691786230f715cf7dd40d2ce2419b5fa804aa90e27c03d07aa2099d954f5bbe28a2202e6af182b4da4cdb47e3ce6dab4edcc244682f64d45052" },
                { "lij", "4e0bc23d3624a0ff41d99236d3478ce3bae74c27898f3d1da9bacc9c2d4806af23a243897016039c8fc09e097275f68a7d4cab2a950d722c8f5110837811edc0" },
                { "lt", "279718e11513a7c4d91f58eb424a94df4f0c2ff9bfe658c2df41653704c324e7a7c890a27b1d2734020de0f34dbb7dd8818ce6b90173f02e5f6d1a104ec2fcb2" },
                { "lv", "e763757b5a5d98c42bbdda97152479182d7832205265e9d1f7b82d0b67dd8f8e77090caa0b8b7ca5d37ef74aee1531eef233e885626c80f7409b3fffdc37c7f7" },
                { "mk", "e35847da8d30333735058de5ed09768ed875a6b8c58b16f2a7b3551565e9d481a2bdf50f8c365249bdf0060cbe6987eeeb1b66cc34ece5e84732bd3de6ca1ec3" },
                { "mr", "47221f84f062c45aae0917fc12cb623c175eecaf4ee334758f9668c9fbe5c9d5a70771a7ea222b9065bd7b854ba76c2adb6b1404d65f2eee588d5d5bce12f3f9" },
                { "ms", "0f207df5bd7edf97af37f731243de2312a9839a17e0979f1524507885f6f8083d08146356445da87c0e0cae4cf1c744f83241988b1faa1e6cb5e307264db0d7b" },
                { "my", "360c466251cd26eb8967fa9b04332f662e80e13c4ade0d67187592ac5d2e525d0a16fce65bcbdf35c5f556be54b924395530af95510a82321a0b670e7aaaf333" },
                { "nb-NO", "463a862ebabe791eb81e1fb404015843d9413d94dcea95cbf28eff3cd7e61e4dcab63f4c61f5dae28bef4803021f877f0499d318b770c9f7fdbfe35f140eda52" },
                { "ne-NP", "6be7c152077acd93f8cd79ea59784d99f4ca70c8838f8cd65ecb5fb870bfa58e8d8b3113c1c52e60df0dc347090add8e791bb9121bb717b228c9f3ad196c9aee" },
                { "nl", "c5f7ce37523c529462a95e81ae48bb83523ecb69df439d9d739b20786738a400bbf2067fb0b377cbde19329438c27c5a4f05e9fca05aa1b2cb3aba31f727c349" },
                { "nn-NO", "eaa83be6f2d25e2da0b0f1cffdbfff29a538a15913d2a6b839cca6aaead78acae01dd296942e9dc02105d54a67d569b38282a19c1923cd604798d37eb75290d5" },
                { "oc", "a90fa0db1fbaf59baedb47fbfbfae3eaba44a8cf2fbf1d4009b34c7aa62b078eb2514ba2cd0e25963d2c282c86132953e82a25c03f238b7bb1a3aa65af1b9cdb" },
                { "pa-IN", "baaf9d5eec8b73ae092cf6a51b5640c35b99e41c525261be79a7e28fb85f52a821da279b52168cc3fd71215ed2255d34821eb2e177c1b59949e32f32ad70a251" },
                { "pl", "8cec348ffba0c470137ef7afc3bc6a6c0c140d02cdfcc774bb5e5cce9235dfa85366447e6d96452396284213188b341d6978674bd86637f0a540779bb77dc0e5" },
                { "pt-BR", "376fdfd792494b8df6acc7d7bbe6b5caf3edf857b1de2bb47b33a85c26d8e1a3c0ee0ea71a0bba6fadceadd1d4d104b462766557e877e402184b4ff50a48d7a0" },
                { "pt-PT", "a85c8b93d6a9e4449b67e98269a69d9e311f0ee6df6ca7deaf6f8b9cbbb768bb797e78fa66d4e06bb97cc132fa463fde5f4f1ebef84111d7df8179dedc997a5f" },
                { "rm", "2e155618b17ddbc3cfe1ef9292f4dbac6647eaeeb154d31c915f990d22bbc09442262f3799d13ac7c3f34870e415eeff995a457a27edda775a80d08234894ff2" },
                { "ro", "63a0d13ed400a5c52598b612569a7b3c022ded4198ab04f813c506fd082ff89d3f2f0eb4282c0fbf902448a4dcbd88f9c05903b8baa1e5242a645350f4e5f54d" },
                { "ru", "bd7de1d80199e0762d9fe7ea8ee072d047189e55a456e03ed1ee55d8ce3401d53fc75cf1e438b522333f1ba901d1216eadaad29184031c823c8ee3f42b1c6019" },
                { "sat", "884cf4f16aebd133aea67d0602e1a8d45c8317c328c7f3db2dc9b4cb03c4707614e730095e2a91bde1c725278488ee8f34b4a8ed4a30333fc95dd400089e66b1" },
                { "sc", "81269e706b64ba1d408dd5dff6e348187a62196e47a3d4e8d8a47d2f21acafad557b4eed04e090e468412413dfc635ee9684aba461f85741c508ca5bc8f9c466" },
                { "sco", "e59266e1532906220898d5601041c8e8b7b5e546be0ddde1494941f6681d4123f30d5dd8003aadf1a8d73dc748bf345602fb8ccf4d8703597edf9acc8fb589b0" },
                { "si", "162e3bf09aebf487be76ee0bd4f7f3459d0b1aec2c835a3d3f4591bcb7825cc785d4b847febba60b0435f2281847cdfa5d8504be08521628a37e68c1dde11df5" },
                { "sk", "51a55bae2f301f290a3308afa1dca7b05c5767713f23d14c4ebf3eb73a676b1d75594fd8d53738f50cbe48db15e4bfee24f50dc5e9c5bab18d635002466c6ac8" },
                { "sl", "16cff67f102588e43109469883c21c2e54290b7eada563ce1a70b1110de7b739fb752d8c32fbfca3d17566ddabf8abed3b42e6d37d4fc07175d55d748a0ff23b" },
                { "son", "da2f7c4b557821f08efc61e29c0cef791291cd1f423c65475e65eccfb83bb5115f4a705ac94a312ab6c865ad8f4610ee0f1c4a635c5472956942bcc2753cc685" },
                { "sq", "d2ef7faeffab60c99d5a2c2a66847511a29e6706485e2a2729ba9591e0bd53d90a2c4e70bb3bce778e7315f6607de710f4710d3168cdd1c518baf53ec2fcb30b" },
                { "sr", "cd3a0457f92d695b010282ce97374517c87037b5cf6c0e35a10792103481fba1f806d7c557a45965cd4fee29d6360fc954e5181c3c017a2b9f05c86d118b91ee" },
                { "sv-SE", "c54f32db6290dd74aa934733bf59e3b82b3761909380ca3cee4096642ee189b69e45aa191ef8e1f265812bb451f3a862d9a9c3dbaed41e8e797ce21079666f55" },
                { "szl", "5d2d7a457f729b64d3a58e6864ead0ab541af7bd3db7b3648e6de4dfa4b162e000d84f9d21af23eae2534d057427909f74118c804b8a97a4813bd05ace937d36" },
                { "ta", "102072c122beca18c131549a8ed9603d2f3aaddf547474aaf8c3badd068ad7cad1e7b74b50e578eb2383c4c3c4189a9fb115aaceda5fb1dbce11af3a20a17dae" },
                { "te", "693970f39a758fa69f41cfcc9929b102cae12c4870f0419e08fbc96fa4996307efbed3972e7ef32fdba1f2121b6504cc574007164be4804f57056bf1cc4a961c" },
                { "tg", "ddda9170e81a327051877dddca41cb60cc397daeebd2b9a97676d82b5750dda8cabebb0b4cb714c550166ede6137e4bc00df9dd5c42b34639422aac22eb9df80" },
                { "th", "4c59693edeab61e0d6685f11355a360b08fac503e7ee415728cc53b90a0aae5f6cd391f814ff95ae9d170ab3cb8ec9381a13937c632d019cea82da282b80a24e" },
                { "tl", "aac6ada4fee6fb0f9a1360d30476cddfd5beccc9c23b0ea3c981fc9678fac67fd6ed6a419c6b842d306d7c52043e76375f454c8eabaf5637f90395f40a0ef5a1" },
                { "tr", "1e678518c3a5726279289d6e445e5e06a48766e38b8321e370c05cab9c7ae5357e470bb8aeef3969779024069deacfecee7e77a04e5c77fdcdb7873187955e6b" },
                { "trs", "09c9b84e8c99af1c696ee3773796723f4171c65c7de4d609f6bc3e91a5e19018a8374d76715879b8630361d8ac72b6bf0029380587b71e3882feafa29632e178" },
                { "uk", "c722e98ae03390fc6c0b99a3cdefc0bb784dfc386a2b49c5a00f335de33d2990aabe0d622beb9d0d85146c3b0f5af7653c4b05160c35a1e72f2cb21e278a36ba" },
                { "ur", "61ef037f38d3e611990b231355bd9726d686c651eb75d023d746a8ccf86eb4dfb0c130bc98db26ca3670e0130731c4aec4eb34d403c5d138edd81e3251a0265d" },
                { "uz", "fa36409d5f4e2478d5efaec2761a8c621e4193c7a9712bd9c4a4a238894d00b16f3f7efe9fbe218171a4adb152e160f80f362fb6a380dfaf866cc6e2995972f5" },
                { "vi", "231575dd153b2e9df678dd5b099efdfb3ad325d52944ab08121a137cb7ba968c4d7bab5d24d395478403757ddc6128468685bc7a7dc2992493028cf5723c48f8" },
                { "xh", "95115c8582587cc4a11fd3cf26c90831113188a05bbfb508f2ac4cc1a752359a74ec618d01dfb88a07cdb1e95792707c448285e64acde01cb8a5ae3954467d69" },
                { "zh-CN", "fd1a7a03fda74f95fd12c5a5783bbcc79a4b0d42bab00a4ab5e8dfc309a16de8f1c2860b939fef0d102a311b883577eb02d89a876919b4c5a1c7b069560edcac" },
                { "zh-TW", "82cabc6125ca467c595c311bf1b3289edcb1c3d6e2078dcb033dcba2472c45f17f9f425192dcaaa725815868ef0da4c95efd8034dcd2132a928f36cb41f150a0" }
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
