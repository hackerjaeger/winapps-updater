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
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.5.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "2916711327f2e0c8ad8b246c6d57d37c878d2effeb6c9735b01fb89f2666943bac02ba03867bce73b0657d76ca6a07b449c46e19c4e24c8c242ed2321c2ab200" },
                { "ar", "7b46c651d6ce90f29a2e7b1d0a4a3534ce2d33fac37ff6d1d2df47b7c0a157060d4f2465726c8fd35bdab46b8ddf9389119eeaf3cf07db1fdaa9c5e5805a9856" },
                { "ast", "cf283c7df08552f842dc352763d9e45caea5afcefe8f3e8209026bd450d2e3db7a2ffe2087941e47667a461436e1567ee4ce8a4b309c8bae2a71d37451b789c6" },
                { "be", "62c90da43f03795acd054275da9f7d39f1e781618496048454c898d4769d20f27144afba741e7a8bd9e47df327f18a4dc1c83e6862b6e4017a3daccb28bf9c79" },
                { "bg", "bf9bdea4323728b1420b9bab759472d125cb86814cfa398d8394f54f0b1ef56e7b3b98aaa58b50f5e1e14f9618bd3c5b11be6edc89efef00a65bae9fae89f22d" },
                { "br", "9985b53aaf04af6bbfa9218f9f7d26df90b8c7bd94e00e1f3984c094ddc3ba79a07a90ba0c4110d9a3db3b08eb2cac97789bd8fc007629e0ea02914a283ae7a1" },
                { "ca", "b40f8883acf450e42e78bba088b566482f02463546f856e5dcdc499a2ac68bc65630c339e5d33fe444da31767f9d878aa216d0332029949a179d6e195d607089" },
                { "cak", "c322b15a14b163decd989b56b332ea9df58a22ffa40212c14045971f6d40353a8ff1ded2ecc83d16321aeae04b81d87bcbb533a6a4f96c8217377dd960a89012" },
                { "cs", "342561d6bcbbe302d1d0d7bc6a81ac18af3f476059fa1176080876db868aa6eeb435c783da68188bd7b5cb18a3b2166f5a3d24bc69d907701557d78c0c0b136c" },
                { "cy", "82de7cf6b80098a390f28555de1c45455251bcf6aef7bb95f71f55704e9f39badc88553bcf00812b8dcf2022803269f4ef85cdc09a23cd182eab8e9d2478eb14" },
                { "da", "801ce7408b2875d1276ac558e6f13c5e78ad6ebb099069006a0876ad2d8cb60586059b9742eb3ac123cde7ec62d756bf493992f7a9d6e4b7077e36486d8a50b1" },
                { "de", "925ec863c555642a043eeb913fc1d1f45ce5a2b5c2dc8dd14845762c3f37e7ec5108b4497b67a0de91ee18e249614485d4d238a05b729b6efe8e1adca9bdfb7c" },
                { "dsb", "0f1b2c3d2ba0096a35b924dd398db4f58c571e0950e5cd6d523440a3138c5ba33627499826c76ca3590d77daaa56177d5de3d38f69fcd057c7694a8d221cf5ce" },
                { "el", "8203c113c70e3d5c7ae56fc78228f9da3936eae43f82cf2b3276a6bc703be1ff1ecf5a697718acd04e91b64e5aa3522c18bdc6d4bfce8ee7de4af473bb23837f" },
                { "en-CA", "0a196c0d88e09d5cdbc88643424b2bbdd21e971cbcbab8ae46142835375736631b2526c0dabdfa4982f88b4fd85ff7577dec5c427a5f0050479e3b62055afa43" },
                { "en-GB", "e3d86b67995142a7f346cd33c27bee25c6875639b0df47d66bb5af84852edac1759e19739b23ddcfd5da971830da3a604f7b7759ef93b8ecfc7e2d86ca61c1eb" },
                { "en-US", "eaf92bd9e9cc929668944c9ad0924be27dd01afcbd0deb714fde63576c830fda707a875b848c0a5f089aa4095707ef5aca87a18e0c4ee0824866a3ca0bd5b494" },
                { "es-AR", "18b845bed6e0e81ece6527a66f9d3f9b4001731bedd80e4aa8f9365e895f6d4a430caeb597ec49070dc1d41e3abee78ef332aa0831b23024812cadad37a05f1d" },
                { "es-ES", "6b7b37643eaeefffbb856ae4efd6b6b701fe4ea3d8637d6c2d3e774171cd6faf6d635eec31a5ce760bcbb9ed93194f3393c975c3caa5f7f6928c852b3f5d7842" },
                { "es-MX", "b05375703d649f7e962999acc0bb2bce5799156d7c86a0f813c96c6c3f19211d7c054755d0d2f8079d3e30265f6582577e9259c3276cb93bd545c2b397df5667" },
                { "et", "b0afb6fd998faa561d3af831eb53d1f879dd3d9f70a2bfa482477c0edd0a3ff3bc8699b91d50214fd2863055f89f2f71580a858c692b699006695e63c3dc48af" },
                { "eu", "fc2f897fee16f81c9ac17a8b7e406d62ca6ff8a02361e459c7720e24a0a5c7c017afc3b597c6e03b768a6df9a5136a1c2ec2c6a240270b4c22515124be264ece" },
                { "fi", "3de58c16b89bf98c6acf6105b36022ea7abfea91ee1d7ee69b1d2cb5561cd643ae5bb36f778b0dab49a4d5884c1d362bf7e416601c3af45acfa6ae15a58f39b5" },
                { "fr", "3fd527cd9d2ebb6825376c930248d8bb6b11313b3f593e4a227e127b40032d041db3db79aafc8f5152095dc87cbede44dcfeb00fd6c23093a7f18b74219cd7a7" },
                { "fy-NL", "5b3a5a6514c21314ad13a9faf41c3e15eec9172a455fec23f0225262a8eeeac7f9886c711466da0a0cc6595c3f399c42f5703b9469b7eb801f95a9215b0f7b88" },
                { "ga-IE", "8a8d234b4764623b862d8f0d25d3e11e1d20f60b2bc0fd1ad2caf038b8d28412574aa6d303baf6c58f0f84b97388213ae7e2decd3dedc254cd4b883d9f543cb8" },
                { "gd", "78edb338e0ab05f5264fe75fd58aec5a6a51f6e136dbd0135014502d4eceda0bdb1df70a04773c716c73e7ba11c3b77cd91c0a2fb84a9b151233b83aad0a7440" },
                { "gl", "d9a91adf1a8353ae4aabbef22b1520b3b2a4e0cade09e96236c5be01ce2b6f89892a65d01a276e1de8bec52d67d4449ea53fcc37b26db918a3155176f5f67fe1" },
                { "he", "510f1e85a92eb908639f6f85055be263d02cf722c3bc4169af90c4c0fc586d7f353072c5c2685b84d8a22542b0e874a2e942c054d0076470cb098023c07a9a5b" },
                { "hr", "bd08b0577b96bc24b2c1bf49079bd50377ff9cdd6788dfb2b65e24de5834c77e9e8395fecd896c2029d1db5864fb15aa9f650ceceacbd6058e247a0edf16ac0d" },
                { "hsb", "b6ee0e3ea3930d05c9ce8c2c1bb5660efa51bd75b0cc5c87e5badeceaf26531d0f3d6bb6283798e10f2862aa44139d5adca75619cac5085e6c06ec3d32da7e3d" },
                { "hu", "ffde0acac5f69eeebb6849fdbf1d8585b471004c8dcb3e0db681f248c44ce0eb8358a3eb35a6e968fa2ea5e9ad338d6eb3b3e8af58c14c840442eb3a60c6e157" },
                { "hy-AM", "d3c811a3ab0dc8fcf70c91ac74ea0988e03d27e16da7f3c1b01f4f83622d0d22503b0f1e29bc0fcbc36228acad299d3ec0cab8ab1fcb452e4e3adc10b22b91b0" },
                { "id", "9ade4bbaa144a8f803c8234367b194407dbb375a2e27013ca1bae998166e2588c18b54868c6fefff5e8241e5e4e783f36fdda6ef7f4bcbf3b0beff771b56d337" },
                { "is", "39d3998d6c82ea88656627255a1542efece54c87e8115ac555c88df58aa93bafcec26f66424b447c8ea4e2825e2eed9519b311c9f73e6482bdaac4250bdb4706" },
                { "it", "69af532133d3fa06671c76c36389ceec774ed7ebd16ed7b1a6bb7553dde3668495edd3eca03022b0ee2ff9235ff98fe0f2946a2cd8368bfe81322967172f4750" },
                { "ja", "c639aa43ef260e57cb5758f040171178ec22556d40fbdd98e632a8e7109d47d9209eaed0bf2cd1802b19ea0d02232d7d5928ac75bbb3456ff135c3f2e8e100bf" },
                { "ka", "d8a80ad31545579c84338ca4728e80f6833e681ee75684ee1b8ce7de8ead2d23bf0c5f706453a8eb81a6960d47317de4186b61fbe95c00741e4cec5415d182c8" },
                { "kab", "426d9a89bab52d3cc085969c48ea078cccc48b80a4a71640392f0474d0cd25f3e58b039aac50cbdea76408043b24f8a1bd422bc0b3745eaf9512d55a466d1c81" },
                { "kk", "b59b9a165782bc86fc17c84eea6aa1472fb0d14f1da3d6bb210a07c51de620dfc22853056d45d636f7b7ec0e09d2c55053ac48fe692e8ea707cdb0b66c937790" },
                { "ko", "c5ecae75aa7c64cf442620ca9b76df35745674e17b76cca7ef78e2d2833d551f48333730992be1082137ef77c2848b0f87d9529ec18a05b5e7ed6ed3b0b80159" },
                { "lt", "5860c3b389ba616a537f8f3b4f058ea5f29f4999274366f2986c6d6e940b007f876dc9e04fb2cd7391e45a41c16275890624750004b1404651914a346f1d5a15" },
                { "lv", "bc482abe973d2df3ae8adfffc8b606dafdf7f901ce12353dd1320715ea81b23b4f142fbe1ec4f752d97d02dc4a6c1ec80704c0e4efdbf6aa5ae5a51bc32077d9" },
                { "ms", "e86d7a44e503bf1a40fe5904dfc307b9dc8b164f07b0be23005421f19877d4a39bf215d2a7d8bf70eede13e980fc340635983be2a90469572d513d899f72a3db" },
                { "nb-NO", "4f6a3eedf1008570e91ee1764404b8702ddaee69f2d07bf8bf0de4e0d8f18d0ae7f8783a1f9234570a6a6fc6a95aa38edbabd7537477f5e2223e78feeb46af11" },
                { "nl", "720dc598c65afcace33b9a57225cf42da75282d7be759845249181a685f65026b3f8f8537df8cc19840189af939f1b81f5fa3f18cd0eb2c7577851a60f557af9" },
                { "nn-NO", "c40f4962bb9c02f68540e1bb4df64851fa692a6313d75afd53eb0d8ac9acd3f0085da694ee3d6d0bdd04a09dff0d6084f8750f872f3d596124a7217e73d0a23a" },
                { "pa-IN", "1d5f5c156eeb02474961bde375dd9b41e8862157a0270e16ac83aa0aee24d6f9c80a07181117ca82c5fdab99a23369f0d08d3ce93913148e7f2aceb03e582e68" },
                { "pl", "f9942b2c5ffc1a03c59d8ca5ab046617fcc0849c3b5415a1119bf5bca716a2b6acd5055a3cfb1a153de3984db2fdda72561488612eee55f92571abeda33c92bd" },
                { "pt-BR", "3e4a4476c45134821b938a8f4f24ea99d55b1a4920ff47aa918ff015457e88d11c80ccaff67c989b14b57b65f922b9c44689e0d953958fb5ea60933a7227d343" },
                { "pt-PT", "8a614caf1d80a0bf1a49a6ed2d7b22ec8c5f27f14c1ca207c059f82f1c534a4070d342b379a51a7de7df6be46ba2621fdd506e97c1e7ded56cbceefd3a391873" },
                { "rm", "f52a0b65a71c978d0312396636fd911858fa6f9241f79682e317d010a51acffb4d539be0d728c0d8b52d7787e322a5f54d6e33e9cb0e3eea50befad85c2b3f88" },
                { "ro", "54250d8d63b64a2df5b7d6904b1104e8f7133dc7a3ba8cea84ef0b82891e850c0227f80c939bf673dab474e24a8224a9e57e7b7221cd2a4e50836d672320da0f" },
                { "ru", "38f38c738b1aa80f84731393665c5a17a806198542b94686385833d20b42e6f0978fc1b6818ef602090dffdac7515a0f0658e8111bb86703cc13789c28566f12" },
                { "sk", "4a0d3f358e3266b49f242cdaf6227e6fcbbd91541ef666cf8db8adc544d7a3fbf93f2157fd611d04eb127b260f248d72eb0fc138026efc56b1b9e143d34997a3" },
                { "sl", "14aae6738a36c696908c43b1e3a1acda90806fade9027276ecdd80c28185f8002b69fa5490a817fc103c63d40b0e2321a8f77de2331b781da503f8f14cd9aaf8" },
                { "sq", "195e6999cbaa11c08e2933cf77b779cc698447aa3297f0ca18cf05d26ee0e807f4417beee8ae5f3f4c70544c08d05c92b3055e7986e4f57492bb810860b5af16" },
                { "sr", "f58aed33b836089474a27ee0ad169d75fc27e11630a60e330ac202d47224adf646614ce58aa2bed615243472bc40c832ded6b8d4dc73552f424e76568f4e1dfd" },
                { "sv-SE", "addd12fe37d90e4d808f98ae85db7e5ce20e680f8e6020afc78b52d2d123996d113765c75b8febe77ddad3ff4239034d381a08fe08c0c0f68f5b5fa7a7e320c6" },
                { "th", "f2ee701d50712cf819c0232fac19d562c6c2596aaa44361cabbe490a25789811d0f9010cf29cb7f4b2638cd882cfcb93fd15c1d59cf11fe46072e3e35f3e72e3" },
                { "tr", "5af262ceead77e2e9cf520d3f6b9135d69a6486d20b19a8f1e4f8c42a2885fb9004008d37178608fc126f7a7e6a26e1be07d50b85e8ed2ab4d88bf83345e97ae" },
                { "uk", "408514e0d350f8518c59f3fc42e27efe53dfa763fbcfe59717b60401629258d752f7ea0684014591effbf71388c98256cb5d736cbda68fc21dffebe713856056" },
                { "uz", "a7d0843bc901641e924022eb4f8af83bc97c4b96b7315338c5f02355851b7825b4b9937a6e7c4703cb2cae2fdf7574d5d08cbb19e34dd4ca996f9ca8db846b4d" },
                { "vi", "6aa3a5d3214efca872b23f1ee8bc3f2511d6929da217973785a2aa9d69f30d6685c779a99122432b84c372c54d606c7af16e5c7511089fd8068a8020ab0631f8" },
                { "zh-CN", "ababbf3391a8b4cca25e0ea1fee196ea36608f35af07d01d054b9f37447072ff5b3b466a9b68467ccf5ec37b8aa1ab4332f5e5d42e17aa6bf47f7a4f20e25b85" },
                { "zh-TW", "38e922d2438afbff3de3ab762aaa2782ad74703481d5272a12bb9178e7e9b843742da7f8b0daf10fe36e72e50f6bae2ae994b56f342964f3d33648764d0b0fbb" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.5.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "cde4d33b34d5f487c77b3cbb786eaa6cc60cde1b3a044b94d3e6fc6419d0df6e07233ba7500f225187b676f7462d76962017f94d2f2a48e8d45ea44323dcc4c9" },
                { "ar", "a1581371b96c9d46e0271afa6c98f1fc8b4de6ef316807d4fe9564a0efe9e6e1d1ed77b5e96097e15f5f2b26fb7a36f4efea28aea0b2c567772255f7db08867d" },
                { "ast", "1cbb7a7e04d1a0d1e47a534589207909ba5168c76920e8c181d8cf26591516f090bda6f4ab765ea4c4f3b877026c4a6312f5faeead02f044619c411727b84c97" },
                { "be", "fc4b218e3adaf397e854b12a7e15ea09d39dacff2ffc914ff00f9e1c33d6ebbf2218b162ba1ddea7fbe75143922a1e5b6419e212a730e41119e07377e07a034f" },
                { "bg", "3e8d00cbbd4417682006a3fc7b842e8d27930fbb72e20a3b7ea235a844f3a19078777b7bf7e34e19333463f3644ca4edb9fafd6f3d335c4f67cb7a8009f4b06b" },
                { "br", "afcb69548324b9c740c286d6674a751ff7c703049d02ad3ffc568abc1bb1614049ad75e9f48b928a337305d9e11764d4d2b6012896f9c9a044ac28203dc2ab5d" },
                { "ca", "17094afd71154e3c2fef421afe52e3d780604119edaec77c885f0fb86613fed2bdfdb8b38553acf14ae15042ceedded0012a20eea3c835b7fa8fe4ddaec41ae1" },
                { "cak", "4670d4760c0e48e30c743a4ed1e6a25973ae224b9cdac4e63b250900eabf3efd18031eefbebc97770364910d803e1751130f43064064c004e4544c11a26a29a5" },
                { "cs", "ae019740bf3ffda08a3af00a4666a08e28024e0bfdbfc22ad90f983008854bbb1f952094774464b63d581f1f10f71a8b06720e6cfbe85f0304ce3e226db64e72" },
                { "cy", "7600a2e4bf21a02b92feffdcb9085e8cfd2337558397c50d8efb3967cd4004e5c8771d57ee569bd411bbae4504a754b438ba7ae3febdd0f4b6aef124a4596df8" },
                { "da", "642e3de539d46e368929dcd6f29a56894bab3cf2766947d1176ed6b468e4bc282d7a693050102104b8c5c1f7f7aeb19f3821095bce556b296bda52c951467f40" },
                { "de", "23df9042093493bcf4bdb72374f90b2fea7d4d422bb0ba2dd4237c3e854bb62f47e6947e657b7c2b820990cb4a0ec3a209c3d4ff7db1999092a0a6bb01dbed76" },
                { "dsb", "539628a9d777987b31097bf4ed38debbec786157682228556e75b3c291b449b9b5549deafebf969c7fe52f7b7cece1dbb6f9e45458d075cb4be49dbd82f7c51e" },
                { "el", "6862a9bc56f172e768c38481b61a0bbc365ed1af02dba90ce5d1cfa7c58c7cf4eec6c6b03b933213f571083410d5918aae17685d12500cd82063ab43a7071055" },
                { "en-CA", "a3f9e46fe076c53531e977a9cd9f6ec5d7aa6da0334c7fe335f83549fd8319aefce86086095a9a1cf617a779f0f6a9f0c22cb7e294949ed852fe7f5e92c40c90" },
                { "en-GB", "9711b846069114faa1fcbc8c566641a6ceab93d6bfa14cbb3a165fa6f18cb7ea6b1a9ff151c254ea0514a854ca8c61ed84a6d6e3e31ec3acd9fcf6dceef0b8cf" },
                { "en-US", "acb676edc42028f89f3d8c55ca9c20e34beba59b8eeffaadc8f6fa3c3239d6eb8f64a2105c8f0f91095bd387e44d62e5c2f77f5dc8813f3da59ab65e4665ff0b" },
                { "es-AR", "f5412c5c00a1de277d819acf8a170e72111306f2c9c4f850ee339db0b13d24f0cbb0d813661bccbb571a90e26f7522bb288c996e52facee4041cfa311b15a226" },
                { "es-ES", "252e15f8ea31a985503756373af52270c72370dc44d849e748b296b2351b2e31afef46aef031a743dd23f72cb13e1dfdb326c725ad89896305cb0c5254f52869" },
                { "es-MX", "392a30cd0d6aea5e68b6de7b30f8dc792577e0d730fa0a7af0ebbc3075241414558055f32b8a4d04e9ba88f0dafbb37041e8b96dd54744e202ef0d2ebe4b4ef4" },
                { "et", "7df4e2095a78b16792029ccaa5d7b549d9e0612f58ba524ae984924afc8101eb8a71f3452882c9b320cb758ad4fe77637b9c23b82204e2849bef3e25f9b31b22" },
                { "eu", "e2fa21da4f4ec24001f3873354b39b4b4a3368807d6946b18d0f0b8360e7692da9ec87d7712a4883159617205f6d5b3811b9f468e5cdd7f8030320d428556591" },
                { "fi", "96d892985632f50bbb4315cee8c0704315bbd3eef4c721450cf319ca6e77988de5a062b562e594629468860dcac08b2c78a9e84c65a83770b307da0288f53814" },
                { "fr", "bf47c9c75838538056320f359574dd933928d46fed0570b54df096dab7084f3a599bdbfb89ee29551602f1179dc869986adf99062d0a8cf70cb771c97457e348" },
                { "fy-NL", "9dfe608c2d1173573eb8b3831d1d2200bb2edf2fd6f0ba6972359d8ce2f11d44ccf27c9505dd67f2a12520ccee5246c4ff715ccca822bda2af28629a15381e53" },
                { "ga-IE", "fdcf24147e5da5edc371dffb721fab9f429da285fbe3840103eb3701408217b2ae3a11893d898e9759e06aa3a52df5986c02f3950abbe8aac2008246cd07b094" },
                { "gd", "c3795d53fa10146021fe7fed9d8f2b5322ee6f475d48449b725b4e9f32ee9f735afa38a5310ca41c6e6bd9b123ab0a2255415fcbd794ebd5fc0ba81c79b27680" },
                { "gl", "6fa61beb82bdd73992dae21ea88f2ab2dc467af9792ed48d0bf2e4e45947f9715dba7902abc2eb18e31614e4457b19f9026e5c28826dd13bfd146868610baeeb" },
                { "he", "eb326a14f420d2bb8310fc435bdfad4d5899528820022b2de22f525c01818c4fcd69dba8f56171a4124c21c3050e8660b29288963dd60520884b61e3bab7cd83" },
                { "hr", "07367837841a305d95f3d013afc15a808448aff7615e673e26cf2a8ca4fbc8eed90f22617127ddeb59d74ac1df4c231ced8c25ea87bfa818be4c362f4e25e9a0" },
                { "hsb", "92b088db4289c634640ad7053cc4cda0e86fcdcd0cfff76adbf4eed873559f298aede61773589261ce95bc4f830e110d60b8f44364be99abdf9bcef1233fdbce" },
                { "hu", "ecb06d533190db466f5c7a6197689d44b9519c10b602cde88cee654f5f37f549b65130d4a678eb365ad60e8b2a661c93acddfaf921c4523e55e7688d8afb3a4b" },
                { "hy-AM", "a88d70a53ca18f546caec886ef8b73d4d9eda98311bb0a2da43591a791d36bb21df63051574a1eafd60103fd2bd2bdefea866516393fd9431af2064374d010bd" },
                { "id", "0ca0012f260fc2a3ccc09577a4a921b9149ad43e61a75a0f836eca232752955dd49d3741438f83b4905ee5db363a8b8e0f1c8c49c4cf7af27d69a2ac0729c85d" },
                { "is", "71e7f3e8cdb62b52ef748eb7a3324158a1685908dc63e2a4029e3f50692e6a741caded8edd802ff046b80418e2a4c2c6e178eb2f9ebf37337c15e86d92292176" },
                { "it", "a020e653646e1f3a73ee27f9fad456df9798a9881cacbf4b24db326c72304881bd00d09df47ebd50c8b666798822c78fe28afb3a9df18eb5431d75d042505eb9" },
                { "ja", "a802e54e31aacf17455a6e9dabb6aa1f1471ecb38277659d4c8da2fd1e285e3576ff6a686c40b5131e3afe8bceb1a8018eb57143cea38d4203cd88373c1ee9f3" },
                { "ka", "5c4ed251ec23d606d19e62b27842d93c18f4f667f4d949173d622c609867ae6b0bbd592e12e328d24e837f7a3070d02bbf2a7c713957b0ddf1cff5874504bd32" },
                { "kab", "ea20aac1d0f37481e31b493d9012747c14a335e86ec185442aec780e41bae233ac41b4dc07da1cbece64d00beb1f8298fe0584760c9574c83972d210c0e52af1" },
                { "kk", "19f105160ca3861a9bb5151c623abdcc1a7e3fcac5b4d8ca3ea61a3d206061bf908ced7f0adbba1c58a336e146a2af39ffc59130cf2ed791decda5100566a95d" },
                { "ko", "07e0433c32d391cda493a08d634d4fdfe1879d0a3614fa95403552109e898fbcd42ac4f217d41f075d4ccd2efe9c0cb4b77ab1745c61ef1cb7d8f5a65edef2b1" },
                { "lt", "ba52fceae5358a1fd0ad40896df2ba9111b65205c89d69ec6c23c88fa6a3b742c459721c911265608887565940552fb124c6ec4f0efee26015248ca9724bccae" },
                { "lv", "24ee66d39d71c0c35e832c0345ab95d6329077454a82e48c8e508623a4b2926f606223e201716ea88189fcc2b613787eb17d150885f2e977f76474a5f3400b05" },
                { "ms", "25b5e49a66ccb4a5568f8825853a82eee015d17b9b24eb9e438c55c3fbb4d9fbe108279039ee272c8f277053991265b757f9faff228901e5f89c1935830275ce" },
                { "nb-NO", "7b22491f5b27806848afcbda84a38a84defcd9a825c8fd257069d0647ade6e6b295d0ffc290a90f134d1cea6281f52bca304c78b7726f83a8daaa2bfc18263ce" },
                { "nl", "8c86dfde8ecde37dc74c7b7cc8a3e0547a8b67b110dff090174c59816efd63cfe23e69a302811346cd9acb782a3bcb9d90f8d72a5f9878df274fcdeb98ccb1c7" },
                { "nn-NO", "e207f763b6a1a43847c1a2d212edd1dbe5d8d744e511da722663d1ca8b2b61e9862d378716413b670257d8fee0198b5936324de8c15775ded53957ab626ba5d1" },
                { "pa-IN", "8b9f6bdd22ee37772a39df805a40baca75235efab0d0a1b10a6d908a0cb11254b8120e8b4d5c369e3ec71b9f9aaa917e6dd0bc375a225531dc16f8a1d8945e64" },
                { "pl", "03840555401baff3be485e9eb64d6dceefdbcb0e6d685e03d27f4254290a8dc350abf595947cbb6926f60394c9056c9a2e8c8db1f1692a7024e61a6697e904cc" },
                { "pt-BR", "55201a8cea7c3c1935098c694ef7c0fbf8105775dfb8f12b1642a9688a289de85e131f3c55953d34a6fb022f5f98db219475a9175879def5216fbe1788ebc50d" },
                { "pt-PT", "44fc71702a288a798a0c86fd187c0afe16eb57e6f1aa07e484dbc5da50cce7ab15cf6c465d3ed1d86e45d40f68839053c354e266beb136c600ff89aec4b3b9f4" },
                { "rm", "16b35dbbc0d31cc1a1a6309749e6702ebee9a539b3749e395f2251578ed140222f3db7ad323baeb9082d8a4dc5a95e2be001432264a722b55b1ce6705678a1fa" },
                { "ro", "6aa2aebefa830b3b5e858f3e324fb2ffe5171ddbb828e08eabf92a91a7d732c0b9e15af0a4173849a4e3c37a686a78c390a2191910bc5ad317fd585469bce636" },
                { "ru", "e9872dfe56f3e8f8bd516f9de25ecf8d56cd07e9fce80a045f67e32adcc66c4231afc90b381c7b2bdb98cb8b78f049684b0be3f2e4da5dff8df199344720dca3" },
                { "sk", "15d6a6d2f1c91d650a55f02ff26bc8cda77dc7154e828bb62c6dc7c270b4e1d34f21013ddf841d45641df13bc21bfd35edaa01db8534b1fa1bba8a9382f2f2e9" },
                { "sl", "4178f976af5f5020d6967c811b1a372bf984399fe5fcd83a5dd7fd341a044424abe771b807e613177119e5d20ec8d011bafaa3f2dc6f879ef45ee79450ca1ecf" },
                { "sq", "8663e0e63c66cceb9481faee6d099309350206830f233c105944248c1aab623168ba4c61bbd4d2d460b23fd48b7e157bce51fbdba6456840b14f0572da770db7" },
                { "sr", "5d893f7a92e85d8ccfbab87a6bee88af1497ae37069efcfacfd4bf9154af7b1241b3cb6799b71c377549fb51848bfdc7d4e2698379390210ea614777101f22f3" },
                { "sv-SE", "ca1145c365c466ac0afb46d5f0d8b87f3152bba8e7740e596dd93753c1520c615677644fb712594d9a554ed4f8540b32e0e6de2efcb022a36092a32b89b209c3" },
                { "th", "8886aee17401598fe1cb81ba929ffc83b8a4fce0230e307ef5a50808b52587e5f10e82904c142e9b0829c454442f98661a949bcef98b2b87899b4fadaf164d0e" },
                { "tr", "d5dd79a58cc1c06b4260ad16438647eb52e4ef82fa6f1e28e73f03d62152c873387d60506146b059de0a4a2d6f848defb39321c4827a5ff87a95cff00c2e1dd3" },
                { "uk", "8fee7d4e91d0a40d5138f311f1c55d102db88c77e463d25f27d7b24d43fadd47d7b31e989d55f7f8f8261d1e60b7f4f49ea3b2e660034c0f8830dee3249596d0" },
                { "uz", "4c92556cc9044670d5a8d7cbd7d39c2a8fe046b139a00bd669d826df3454ef554bcd50beb992b01fbbe4d49e26ff20d2ef61fd2ee280e4339f8ba54e5cef7230" },
                { "vi", "ccae9788ba11a417f823e81886a17f4c07c5d54a0c906901b3b665a5970c4828050aa698b2650728a1deff53099e44a48767938d0628e640927911e631524938" },
                { "zh-CN", "2163ef3338fccb601276dbcbe21cd60429accc27020c32b66d03c9cbefc4c735d6c8a25944c7707346dfc8dda9cb5c1ea7640f597f85b91858d3f8e58c2a6738" },
                { "zh-TW", "29a969dd68fcd6622ed729851ce451a92708d3529554229a3e84ae1b32250f7b62c0eaf83c093dcb66c6dec11e0c29eedd7460ad6def813391ee0f6415f7f42c" }
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
            const string version = "115.5.2";
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
                task = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
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
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
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
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
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
