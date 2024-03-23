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
            // https://ftp.mozilla.org/pub/firefox/releases/124.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "c5bc0a6a36faa9cde196ac8249ee422d3177be104de4c55c7b52cbe2c4cba582d0dc5f65c50387ea8a80cec2c832e254a35a339e87019f2b63a38ac940eb414d" },
                { "af", "66ed162d143fab024cf744923c29115f381f3cc26e0d145fb7582d901ac8bf26b33b91e69b2e617d1f3c34b9c2ffbd58a4100243c72a55cb0fe39105f5350021" },
                { "an", "94812a53dd2f704e840b3ef725a812bdd4b7855ba36579451280a113d2d9975a894877546bdd8042b9e80fff3028a2e7fd48b0a23e40534f3cbfb785a9728104" },
                { "ar", "c46b044876860e16da0693719bd82dbf6ede8eff5dd23b9203ce6e306e878645c65c6d0b2859b2149f4da3425a7113e9ab50d992e60c99b2f0ace9cdd97cf15a" },
                { "ast", "3057667ce6aae0c053bae2b81ee91805f453b5e4b6cd6ae896457d57a2e56feced9e10e39069d6734e34fa7b2bc279127ebb4db74bb295ce9ff69812784b817a" },
                { "az", "aa7532906a0604b4f88c4c8197022691ef725654d19d8b1e073790bc539f95b5709521370a59912d6a6c7621e6b20d140dff5fdb6978c65cffe759ab36a18951" },
                { "be", "78030561e11777c3941d9269bb1c08b7407e738c3b32c19622705a3b3ef33de79a22403b4166c0ad129c5a75f69c62fd5587c399a94d40e79b0c67f5e40fe1cb" },
                { "bg", "62a80b8fa75cf7bc92d887249fc446b4bf9baf12cddf20510e19fa3bb384d15da322a338521b3fde5748edd7b1857e926e3271440cf0b834df3124c975a511eb" },
                { "bn", "30cc3545c681662ae02db052f92da73ea83d45d209e5ad75af3a9ba193b7fa931ab45aefd3969b0e5e12063df3b54365cc74cdea1cd59b6826e5dc5662f9a3dd" },
                { "br", "ef22068fced42fcc02bc71b7bc15016a670f5a95d502e079cc455ae34ec222c6e57bfcb51706fda272bc9eef1d2f949c39c1c9caa4d902d104b66934df545785" },
                { "bs", "04a993107cfc1dc04150dd6d34f74fdd42cb8660f1cdc48083c6f59adaf597635c330efa3a8db185f58168708465e1467a28d6ce55581ebef1f681194ff5abf4" },
                { "ca", "847184e558a22f4d55a8d588475162c82347ea8a2010a160a59ea57faef0d2ccd5985bb61317ccd7ebda976070cb9fe74f6ceb3423029eb7624c512ae117a675" },
                { "cak", "4a02bbbef828cab462e63f394bd2e6743423246ae2ebe478b85ecb31cba42bd109edfb15b59fd60d8681c0730e389ed6c87858f2cae6342512a4556116defc21" },
                { "cs", "c6825a4be69748b9d7e38f19c7bf87e56a103698b220c7226da94a149453e46f96fba49e7e52093cae05362eeafcce9e32c63ef15647e71013be4576ce4569b4" },
                { "cy", "c48ba9d9c201e68ee3c9f77ee4dd81db7b589b473f95eee92783e858e2593a80165902fa32eb95bbbdd4fa35f73ab9a41bc57e72c8a24889ad76643ae1077d98" },
                { "da", "8bcab0257d756f1608f05098e9de97f5261037a8e58dce5dfb2d3532afd56a9a88bf1f07a74d0584daf0ee7f8fd17631c728af733f67d91df4014921607a1dbd" },
                { "de", "c39db090f70dca48c8838286fd8ae09c80d5c090e1d7313ab1b0c0027eabdd5e6d225dbccf175215b1f28fc36c3891b4377e7e1a488c0bdc5ae756aa429e17e2" },
                { "dsb", "7162c9a059b339c481e09350e92c4337a6a6d44ca66fb0147cebc70cdee98dfdb266ab6ff45574531e1a30f0a3f632affc9df303a63f9f6fd1de33d178d8b3cd" },
                { "el", "2239c2b760187dfc2b06ceb59f509b2bdbf104c26fe62d604aadff4c1f071e8bf89ef4ca8c2bf4d215133a2cd2e5f0d2c276f981c63da51006e51e23e8f322e5" },
                { "en-CA", "437603baab43e2e9a0666a723e0da6bf77975058fe99f613967aad8cee9d1dbeb76f4fbc8d93552389574b3d0dd7aa5b8e719677f2b5408c5136e1b0832f6908" },
                { "en-GB", "75c6ffab050ca8eeba4538405853a6dbcb25122bff480569e7840cb92e75d778218cdc47fe31d6a51375a2c1ca14e144a6c28af82a474f4802e11559665f1b09" },
                { "en-US", "eb8e837f62b9f82b57db47922a5a088539a84636e12118bf1c345886a03493feb7ad92cfbf57f2442de41040e246b4c445d074e804f0b918bd6abed057c80d59" },
                { "eo", "97718b98b99f832f297a6a2ed8030805ae75fd1d64a40ccadead3818345f72bd4092ee879911d997a036eaba79bdb018c38b7a9c4d4c7c24b6a7eb1a7be7d534" },
                { "es-AR", "62fa28bf071f0b54c70d6011a723f9d5506f64060ae7727626754282263490a93907b0dba106408c2d10d94fbb9809131d1f667f08e8218576a5ff9ea803cce4" },
                { "es-CL", "ec0716d60634d834be9e3c2098413cc7f708368109e3b826cf7e790bf42810b5418b2df6d14bca0a740161cea34eb4463e6cab81e4f62b65003290f239293d3d" },
                { "es-ES", "2eab7d05edb5378852ca5dec95a4c39036f7b19b0852d26544c24f6819dddf67e89fafdfde1407ea54cfe0455377486544852f14f79ad3181cfc871d74447343" },
                { "es-MX", "0e77d3f041c05b042653b799c56df1062afd1383714c022a8e4175f797a75939898163c58b755174e6547602f5dde38290768ef39cd1a5f9084bbbb6986ffc0c" },
                { "et", "20590f33ccd10da01bd7ed9f1e623d569985ae4a905e27a895af6c49344472281c4704146de4e0478ce58d210b82a3bc9e41d52d527de708883b8ed4e201fd4b" },
                { "eu", "45c1e0a66c01fe58be5baac9c816fdc8bd29fe951e49c50cfe0d548cb29d86d52eefdbc6faef56526dd21ab3fa1c60747ebe04a053dfc3af7e550beda6dcb281" },
                { "fa", "52454c728af26fcb7b3c8382db5d6212810ad1e097d641a74f881bb1f07dcc40234c9e7fcbbba823255d732375d47b47fefa2e3e1a1e10ebfebe4a546584a7ff" },
                { "ff", "3f311acc53ec939748f91ab8188bfbc9e20bc77c03114b8447080130c9b6ea612bb3c8971b49386fe59554acf6c05566a566d82b098dff12558939d71325579a" },
                { "fi", "1c81f923b7ea3822d2a228029b30781ae5625d68e51c4b76e55b173e04c692f8dae67226fe755962cf260af96a5d768c9fd6fc522ec04ffa59770b00e080a69a" },
                { "fr", "0fec9f8e1e0f541908a08e6e02d54b03bf8489e90ac511d93c7423d8fbf546cc011c965bf2a5069f769891166bdacd8930f03c64d8a038e9aa19feb0a3792293" },
                { "fur", "e73e83c3f8e1a741740a38aa974a883b1ffdc28e03ee7b261e488e788e79d7ac4d1a647be8ef5243a27be7ccc3a36f842a87d36fc216f88a7e31d1df000da3b8" },
                { "fy-NL", "6b52acbdd56b27677b2782b5848b24a04ad837fb26bedb435826bddc8edb33c65b3ab14a7115506d1c6b154b532a40238fae21cca523f5601c59210779c8a00a" },
                { "ga-IE", "09951e787d82dbf00a2cbdc0ecb10e147d556fd7d364999cd1c7d990156f3b8ef7a5159f4bcdaa1dd92e3e42d33898f52b8d43475621005ca90d135a0186d932" },
                { "gd", "3133a2649666e6ac7a790d350ceab44b927f35bbc3d0bae5a1f6eee3c31ce88d3a340c21515ca7d783e0cabfc400d417d5706ca4aee4214d5481538873b84236" },
                { "gl", "8587f0f9784f8aae8a7304e1d01f211b86f36d96ce7751e525ce8b0401c5ed257bd1bc715042ca5e5299d6edc8ce3a3d45aec299bf59df7f85ccca7c4a82835d" },
                { "gn", "7e2b06f6493f0ab8f3b7697cadd7a8fae30d5ba6ec284497ef33292840da54d0bdcb959c947d08e81f1f2f6b746c19acd9f995b9161777928ad824544216f929" },
                { "gu-IN", "3a34b54a46c873a8b592e647af4f5d225e2fff9cab1fe0657e7ce9f5e285a5f7564e0c8e3aa496e3b4f4a12eefa40c90d4b9fdac9f10d7284be96a3366925d0d" },
                { "he", "166e4636098c350db28d5bd9268aa51b61b596791a146bf47159d8fa7dce69cdb7eb6ce5a040b4c9d65dd1ffd918f9891e00c02eae38f3e4a1c1c13a13b0b507" },
                { "hi-IN", "90ab8d5aed614235ca6286a9af4fab81f39e4ad72c4d71c19e373f5edea4811449ae56b50e8f0b00406b5992693da09c1c6ab79898ec1a1982cfff57aaf31829" },
                { "hr", "a12487e01d24e091936d782f45fb741176c209144fe88dddbc040ea896457343e2891ac5dfec7b21fb191ee5e29d2ce4aa5d6837ecc3c7b8c0c12375c92f0f46" },
                { "hsb", "52e8fdae2ef64936e44b3aa49786702ad6555f21d9a092dbc180055dbf1dc1972cfe37af15009c5a1f0392dc7d345438b58bc2e47e3f3a65d08676fd98256260" },
                { "hu", "1eab98f4ea00ad6fe9a107090e5a18e4b08e20d43d34933ddb2112cf441804690067105884edf375b99e8f4d591c97bd5c230095b36ac21bfee9f23ee24fd472" },
                { "hy-AM", "e42d40a737c2c874d4bf3b18eaa4c1967fe71db88315f0540d26db52530dd88b2d8d04bcab02c112cf092e19b92f6889ea5d5e93d65f2e8fce4b48bf0c5b2751" },
                { "ia", "aaddda76e56a407f50f63ba827879bb264f9c74899d96d79f2affbc60c6bc631f4b2939a96694419dcdc4ad3bd3ed5409a85cc017520f0bf3715c3b462b8b719" },
                { "id", "a278a06837df5ff3250165dcfa86024737effb823aaa548fad27898e4f2b47aa51ec4448462151a7fe31b8d6305124e07353fa9677f40ebe8c459e534ae2229e" },
                { "is", "161475b7b8738cf07f4f60d0f31ec22d76bec25b518bea6ee18f51560a8ad02dc9274fbf4cf30db7d6545441e70c6cc612c7d3866cb9f656ee883bf60f207e25" },
                { "it", "d4293d251173d95d8e47d7ef7cdf8c3e889a065995cd4c82854e46fe5288f7c7f9803c1afe63769551566508ff6dcde43409e3dbe93d94702c8903c18bbe330d" },
                { "ja", "ba4871020945385f2e149780db6055c00dc58affc127e53ddc1328eebff7d586ccd6cd93aad6292944a8d004a6df63c62553c24cc68811771fe35717aab12880" },
                { "ka", "3b8a3d3a2ff4369c4cb03e618c7754556c3009f8597133bc5d4231fe7d13cf0a4ef5ba4e1ff85c13b0d1b71919ae69c0d98a9a4934c7c517b3cd35b17a7a9a0b" },
                { "kab", "26c214fe7fd7260dd0d220c72cf1fa47699daa816939ad97ba666887df010b8ce7df90ae27cde5e95fa7102c68211411b6d28ae16be97132ffdd9127489a891b" },
                { "kk", "4b925774bfbe10843e7b35f61c4a2cd98e5be5ec337cc38c720ff71f23813b520326dd2976f1123e26d12e8540740d9a8f21acaf93d3dcd3f49679f81a741443" },
                { "km", "7a4a15d259378b99549b4b4c22a368a06f03da04b6f54e1569d5961711f7902b6839da568eb4f36f036d43584808f0a465aad7d1130a679ff20b13ae72e9cd50" },
                { "kn", "b9a7b5c5b9cbf4714c8412bdd7ab368f213adca98d95af40b64cf6892cca4e97d0da8700cdacca6215c25a4a240627701d84d2fec36a014ecd99bdf9592caf8c" },
                { "ko", "c8e6049a02254331b279dceaee51dd8d7282a01c9381ce4dc7d62e173fdc85eaa16e1f0edcc65ea73f4eb88443040467347d6c3cae0a20971de89d3c3255cc0f" },
                { "lij", "6e69e83c447f61a99d77ec98030704bd354b633cc4ce745ab62b2363107e66ecd0112a8c7c33232afb327cb5334e889a9501d2253b4a8bb7159c06505c90525b" },
                { "lt", "2a8e31ae61bcbff8553f204065fcd6d5fe8f04d222c007a567f803a8b0575814bcc1d7b450cb5a40672e450a4c399ddd0bf3426f96908fc11d67a250533cd0de" },
                { "lv", "505c4138bd0755cdcd772e202791bd6d1b1868c2e4f18724395adf8d76c1b59d29e4b5f5059ea747210ec928e4719a8611881bddf74affd01da9d54d95654acc" },
                { "mk", "31b52a92ebdd90025dfae37cd9939eb9d555089e1b8a94784804b4194ea35aa2d8e9a0e580c191aeb229a9811a41766193e4b7c4fae780636ddc3a2f4e0c5466" },
                { "mr", "bc3acf9e019527cea5806a2bad8dec14a04b8e8070ae5b60c7d6ca549bf63bc2a602729b0156de70895c0ed0f40a70d04c6fa821362cd646536af3831e8b1745" },
                { "ms", "1fb8edfeaffe6679ff6112cf6500ee45d80dfc6a4d1dea0682562959ef7899de958cce3e66cbf44bbd0e7cc6aeb62ffd5f00ca4d70728512407c3fd4330c8056" },
                { "my", "1eba3af3b043df417641a65d3c05a0eeec68d360c92d4d4683b98092633af28b24b02172ddbe0e9abc927e28b018647826269c8754e296d04dac7c7370cf0647" },
                { "nb-NO", "0bbf82884f1d8ac1e6da4eaf40bbf582d457ff7cc349479ec8c00fa5d8e19aa1d0606376aa8b430538a5ce5871f08cf9805f1f0dbd95da3493430f95f39894dc" },
                { "ne-NP", "9506682ad609e4658e696ebaabbf2c7ca85c1fe5cbb3f7f4231c5c1833b439a4afd711093fa080bef0c17081b313a04b841df643841ddffc11d082ee5b7845a7" },
                { "nl", "95dc58dbe88247e1c0094240b1c001b113eef267e2575df384cf83d824071df73ac75b72a79c64d50c39a3edd18b27cab3f28c8fab324dda4e66bafd536ca193" },
                { "nn-NO", "a7efb0a2f4fd2483c22b84f97e59db07abeaa84a806c117deba4971fe8f071ccafa676788753b48a551375ffac5645486073b1b39bc7fc64b01a393b2bb8ab13" },
                { "oc", "eab6b190a5813b9257f151a8509ebdd9d50e346f09ab7ab2de63e8e7b94b2a59f57c1285d93b394ae40efc243f4d7a616bd9f5c8b67c8e165a2c73ed1dd160e8" },
                { "pa-IN", "189fbb97f5b663044ed8a76ae777342383447ec3dde9c8c2795607a8c36d7f8d94dba8a2ed35c9069a6eddb1121c270c13d1895cdce7d3002cbdd076dadf65ad" },
                { "pl", "ada0cd552a37f997b68229c9da13f89055709f0ee528f8b05b1f4054d40e6774774c2f8f6c4202132f62f8e7fb778a6c8c34e1fe011f74bea0b1ee77a34bb1c4" },
                { "pt-BR", "470f7b04fe74ce8b0bf415c195c8c87609d732acd3db86f1332c1e5e350e67a2461833e015525678fa9614cd53d56d292cfcb5832a734b925adec19fa33e38bf" },
                { "pt-PT", "181e0b773fdd7b1392cb464588286d7361e7f8fb14f2f90f5d6c186fd897b96cb3e01515457137f70fc430a2cefb5fb1bf1eba00ba0c882e6998e05d9291fb77" },
                { "rm", "1aa7246fb1b7ac7fc98f4e7f6bd76b102a6e17b7ae5c76c4c2132443729a53e5d80c77ef9d98a5cefa687570c29b6ecccf47e0a31c60afe4613c3d852c100893" },
                { "ro", "4531d8fa322286b0502f6135cf30ad28a04a555f7d860a0e6700c1018770b48f559e65bc693cf618a2206f273a9590451c77f60353d1e125f453583ddd96cd26" },
                { "ru", "20f827490e1f248c2e33910f1fcf416ec6aaaf61bab4cd52914e222ac4f98a6bc40fd97aef94fa6835e4f8eef59bbcc3bc710dd22abb0b31ff9657f502f95e15" },
                { "sat", "e2d1c247ca115b9b334c047ad53fa6397981107a68f41ccd03e650f549f3ed2c393355fc1e255f1bf50e94720714c50fe093c40db20beaa7178bce128a5efb9e" },
                { "sc", "c3016e6bed0189124620128fcb1b71a08691ee32751f7afd1c88be04abf5d88c4f86ea21c3f7b53ad328a9ce9f71d0036d11781b334f1d197e186e0e6bfb97cd" },
                { "sco", "9e60413cd845b231482572f02aeaa13e7278c574657c7e43647129c89288a3c36f2978055aaf587c6310fec27851036c93e75afa01a1ee07fce74a1bde5f6080" },
                { "si", "6dba8acd89d00d487c96b426551ce5f7b9a9d947aa5f48b1aeae6391e157c83867a3d6ea9722cf9511a234091f4d73411c6101949846f52830325cb09469d337" },
                { "sk", "0d453c490192dbe543e897507a445dd97814044d033d840e32833197ac16e915dd259cb4b7fb34dbe25b0bb3e7c2f2c63c73aa70700b4dabf4171f6fbc7d2543" },
                { "sl", "25c8ef1f054f38347ac96830465e7df2e30200dfe3506d417e29e4c7b4e762c7a6609348129012e04ea2f9bb4b41bcf31ceb96194b1173df1c13b561f4ef7d43" },
                { "son", "bfbbce350b87915e7ac220edf4038fd9496b625a83149841ee3492629d356110d5b041f8acfe5740ba287c472790178affe15c159916fde6a3b4446f1166f02a" },
                { "sq", "3f2cca36d6748c2d4b01bd26f611e6bdf80022ed2e7200148cd160bd266965bad94318ef565e9fd960ff3eab1197851b7f514b5da95d088e36dee989b296e10a" },
                { "sr", "a7b8337bef7752c44cdb840b975b26e44114457b1e50e45ebf8d2fb1e9f2088cbe6116491e20c9648d66146d2ebbb33a0d8290f4a7d98f68c3d72a717db839da" },
                { "sv-SE", "f01b1ce8fb1263cc041bfd2d9dc63a0f0c5bb3367a26f6801a85b4092d332db325f4e6a3603ec8dc47a00a3dd68e0890dbdd4624628774d81d2e3f3e51412921" },
                { "szl", "d193d2a8a300df29bb13a29fd2c2ac0071ca8d128ca210503d78812de1b42fdbcfaebac06d01b98d9fcd31a3466d6239bc77864d9557d91342feb675a46564a2" },
                { "ta", "460e5ad112d1b1f4e7eaafa4e3b7c159e375d077e0e66727bfb9dc18cb56416bcb1943a914a08f65b4349f0f0ea9120cd77f104638ff97f7ebcf8f52e20160f2" },
                { "te", "cb12684e6b74f5146fd893cbb8b2dfd2d3ad36df57db6f2f664b8a981dec9fa09bc7c6de83b408c6d0a50a7e42dee95241423bae80f08a5d27fdb9148fbbff2e" },
                { "tg", "817d681a62b0b63d417f90984e09997a3d9bbf7d6b082b2914b20e23fb0b2f05fe057ebc9b3326791b62f3439b4c934739cc3b6bb047e4d10f1ba6f7d19e79ac" },
                { "th", "ddbe7e0c2f90570e09e434ae4b6a5eee1494d91cd498e057fb22c23c1e9e358cef89c5195ae49258aa5bbb67fb5869daa0e124b0c7fe25da4511a4c91c2d0b8f" },
                { "tl", "16161b93fd80a5421bf2acc5ba3d14f6d80619e3a5bbcda34ddb4708a13a7623e4a14c5a2ddbb2402d32472a0f1087ecf6e60b3936a19b5125a3e656f133a335" },
                { "tr", "7dd4afaf2443db03e521a8fb769d317a1aa5e4ecca425be0cfadf4f3d51464dba427112e7dcb75d0f97a7d24c082780b9998150fc00d8395141b270d3aec0cbe" },
                { "trs", "6f10fd751e0f6de92aa550616f2f45851bd19a31d4b500bd99365dd85ec6d870b4406f5a24a86b71aff7f27d770a40b6870d3c4a4f8513338b9b9340ac40e5a6" },
                { "uk", "18218274d565187a1553bc9ef5c30e46bbf9b2cbd86726999cab7505118474a75a7e24f5355f2f0be4580056c6f4232929a9775739549545613fa5239ff0516b" },
                { "ur", "acb9f5b36cb1570b555f275cc71cdd1e19cc2ad353caf845a9c087f64e1abb9ba8e269ab1cb7d0022983bf6162563a16708b6d36720ea762351b03629b5feb64" },
                { "uz", "049af3b373e8c5c0d26b56ed6131f86bd1c0510c868faf772d6ee2591c79e6cc592b0e00493e495732a32a41b73408bc3e80b0626e5d99a1bbaa2c86643ac96f" },
                { "vi", "2cbc1d5a940910274df008ae5e83c09c7d27cf7e7939e9e8f2ecf5f2fecaa6a4aa176a43306f1bfb1a80652d887e2e4f6051b16c5b06b3dd437e39804080729e" },
                { "xh", "59f46161e0b05e4eae8eb338dec936dba0e31f4dbfdc1b19e11cab523a08ff5c088ca8c88e34195e20c05a9b2f1a45585d68e5c0439e7d31a592c36cff8e3391" },
                { "zh-CN", "f4c719e8972744035c9943eefe6cf288abc9732e4bbaa93822cb7026ada5d5adfdc4bc4d0e8007d3fea917b4b9f29bb8a6a761c79bcb6e91408e09f8fed98f0e" },
                { "zh-TW", "2e63d038ddb550b1963392ef35a63cca44f725681af3ec06d48564bb393c08e3e3c0259548da70594365061caa72aeb63abf5c475e33c50fff6d4e54df37990d" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/124.0.1/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "6075248fc5fadc5bb0014115292b5674241093fb4382716ac9308202dc82b9bcb89703043728aeca119fa20306f7303516ba4ed9ed7376a96393620e9b3ce78e" },
                { "af", "e307740d3b216d3481b50d4c07ec660692000ea6f881cc914706d0f77cede77601bdd2f90c5c6874cc00699aa9eb7d80c2858a199042f659d6bb36d3781de36e" },
                { "an", "738230639f8b4d21475961d153de06378da5ce199b878014d2c4f354bf9755caaedb2a969f2e7781bd832b87a364fa987593bcf44092b7e08306aba93daef313" },
                { "ar", "78ee7e8540cb999106a5771efc477a8c3c2be1e1e424485367971f8dd4c0c7a95c2af97d01cfc3d9fdbe1c5130c657408ab1933fc85e22155463122172b5b7f7" },
                { "ast", "6b0dee248a3e2c9a3c06b83395f7364c281b99abd16ceea5a6d662f8f207121a97b737dcff3eb16e96d2bdb78923389278fe07afd55aa59ae3ef98e5fc65038c" },
                { "az", "3f97e5ef859a9cf41d57bd9b955aa9e6698a7b2c7583bf99109c411c2477a43800d73b6d5d4321985052b077617a76b91949a293abd5ac6aa8b23e54d92d965f" },
                { "be", "c2e42e8b4d18b129ee18d72ce781ead9dc47ac14ff1aa29a79e30111e47c640b13fd4be90aee5fa5e6e795cf3412b36d57220b6a72c9fb10f60f5faadd2da156" },
                { "bg", "89f09f152888b7026848bfe7e611d7f01be255cf0dedc04a8d092c8b97e3ffa5bda185cfec8facaaf70ac9757594af426842b42285461e609cc8eef0e8368c2f" },
                { "bn", "0a86cebcb47fbb0510d69fda3df78e34ed7add52f080fb04587c14ddbb4de567ec73ca396ac164db5139d6ee80632d5967392e95634222bb7643fd9f1287b8af" },
                { "br", "88d6c5c009010f0156d4e66d14c856143aa24c855a385e21571cc785c381824b54e7332f16e0ce1d0bdcd83ef2f6f56e4fbe533a1a3c44558b147aa91c7232db" },
                { "bs", "c55b9e11cc76427176e036bd38eb7f9b98b1d34182cc96f258aad76c113f2f6b6f5c8ae74727c0c72e1f984ce0c980b3ed015831b1f632956cbf52635fe2d72b" },
                { "ca", "dd09e65ea02c2319a06a6045e5b813ae47bae89a0d980104c5813dd536db1c097da4946e126f266cea460157798773d260eb409ce2ca74895aa05bcef1b6e313" },
                { "cak", "776e7879dbde04a2d3844ff832b4cfe9f64a100784c8fc24a30b75aef99169c08e776f28e905c837da628ebed9046c6c0d829a734fa7b61f9c3b794a8695ea5f" },
                { "cs", "25bf2ebf3ed55ced6e979f1312402126928fff62bc6effafc6a7714501f8e856ed24fe62716f1f30a681a471a668c67f026445870407f52d67d924ce3f392daa" },
                { "cy", "0c2761112e8962bc343efeb779d0c3516340e420eda116a5e764d0fbfc686b2f35bbb59c459b1c3f53dc60e27eb561c86c6310bbe219b127f321987dc9ea6da6" },
                { "da", "e61cd75722d8256de502eecb9e3fcab58c81b8dd774b4616bbea7a0467a56efc112c98feded0a9dde795de17143a4b41836afa4241af38aa7e83c8f278474630" },
                { "de", "5ab9be3e2440dfcba9079b5e9af84750aa9e1715d2369b073414ebe9da787455698219c79fcf847d65d90d7dfae358a9a3a3ff9c6382e247106be3dec4b0fc37" },
                { "dsb", "0d7ec12e952559734515840c0fd5afb542ccbb65f952d4c82e11ebd7162d2bfa809b11f91f5b4a5286f18f8f6622c30138b82e363fd60c1377e061a5ba7418fe" },
                { "el", "5dbb2d0397c53586e3b10caf8771c7765d6e9d5ae8b9ea7f48479f0c86c57736a0a15d5f803c7a1205edce4be2c894f52681fb1231e3fddb89db829010c59ee3" },
                { "en-CA", "664210429a7ea6f916f3e1fbff81f81365cc38ef2f9ff1828732d293b24f3d715036ee7f573d0720f6e581a93d71f229202fc1612532177a32ba5b2a243ee82b" },
                { "en-GB", "9abfdb5bfb4cfafffc1aa7dea6c5b6c92b18c3567d3b9a12b24caf8e91dfa00679ce1c77e5054ba1bcad54e43b28427e62ddb03eb6eeef16cf00a78c5fb97096" },
                { "en-US", "19ddea61245e6c5e6ebb6d7a6d0900ad6e39dd11cc3519d956eba9d0ec5889399658cc36f2c03217749bb6d8da99660106dcb0dc1cb87cb503a25ba02d86c63e" },
                { "eo", "e56a9bd3efbf51691a0df8fadf405e4bc946c43cb9547061af7483f131460f4b8bab0c8ecfbc3643d9921eff7b00891d230bba90ba1e369aeb1e8d95a5519048" },
                { "es-AR", "7025c2faa72003113bb4e5122615eefb7fc27220bd741228d78ceb49a48718f480711ce50784269410e63461263fe9ca7e174b2b2099fe9770e39f80b6154fe6" },
                { "es-CL", "c8b3758b2534e130ea1176882afd3435c3689b01a339ab724fedf4bd47338a7dab45f36b5a532b241aea9bd62dd68e1868a4c38a1ad99b071d972317a5054072" },
                { "es-ES", "9928db7bdad2565e48de143420338786a01beee991b391300c7b12547ee7a246892fb52c6efb1e3eae63ce6bd1bee194aba01941421bc75b32a4029e5c24918c" },
                { "es-MX", "f876f1f4b8f174677a16d3edc0126cf4af596dc1054812fc3acf8fa274e131a5288b6769ca39359d76df3b162722f148ce1fe6546d4ad68cdc607d23a69b2a5b" },
                { "et", "bf7ad80884663a9a8df08d78a262c72471d731e8fcea839158c7fd5ad95edfa070472113546a6745c29661cda0e85103042d416513f8800530c92f236258ebfb" },
                { "eu", "25d356abfcbeffb56f6f66e55580d9dcfdae34a8bbc0d4c5a0c54153bbf30d682fdf08cd6fb0f280d25fef7696fbb93e634384b8945d197ccd5ed260dce02db2" },
                { "fa", "2a3fe555112ad8601cbc3c43d184b5ec3d70f7a218277659c7c94a3e3ef95f2e4d37474cf26800860020767c57560d65976e4b9aee42c27f9803154de1e7dbf2" },
                { "ff", "7248eb14853215854b5af0c08015f50f1941e5ca43ee68d8b3f425d6453c400b274bccee9790d42afabca70c49c2a985f15f9528b8ae63106a61167c9bbc4c53" },
                { "fi", "3570df0efdd49aee2e21b169d28f4551e57bd42d27c657935beec2e18f604959fb4da24fba47e40ab2a68e85519a8aa9be58d39d264bffb1bfd9ccc89a3d10a8" },
                { "fr", "b6c407eb23377d5280f1d40845addacc45e2b3036d430de6c3487ff17592be42c782e44e01098fde09488024e3f5d5dcec99467dbf406c5a04140f00523d3465" },
                { "fur", "2b9ffad786484c82d054bf4b7b5d56d33116a1364c4735ebd55166fc64ea0230d663860af53e89c89796d8201286215a64acb72f29ffdac7adb928e2ef05c656" },
                { "fy-NL", "8a642c3af3112b8bfb618077675c845b8fbf59db5c72566a5eb55c85aae83112fe75778e86a595fc1e72eb3c510421beb3837219d813a16cd36dfc6159ae2ee6" },
                { "ga-IE", "3f3bd43a7bedf77858ab02e6118d3df1bafff12f8629c69be752630ded18e77e5abac6e8ac7702ca3c824f395bd84418bf0c5a9cc20142e9a5698cb2132d394c" },
                { "gd", "a10b63ab9c08173ec3aee90a71a09992ada70613777c06362b784da1844556afbbce2b6c8c2025d2504eff48cdd1a2f07f3601ea296a049e89d4ad03163d2a02" },
                { "gl", "b9ecb7fb01a0189877485529057dc10dc6709bc4ac1b32443ef083f863210216fb744e2c91658726088115ca14ee6586786bfef0fedf5bcfbe4750b58595978f" },
                { "gn", "e25e91fee5f1f095a701eafd86671c1df02809b16993c2255fe859cbea25956bc06cb3a384601a18ce7ef2b9ef1a913b3e4993357c12332a9acdae9efb89d384" },
                { "gu-IN", "09adfb2dd70bd6ab4857be17776007fa5c1d7caf7c27379d30656dd8c1bd3286c1f7035b2e671de505b5b1d668667a24470985ecedf0918f98c2cd8b6cddf511" },
                { "he", "bde914520f1ffbbf2d73a98edd5924c797948b23904dd307d7c5b2d409fd403d8af7d30c1dad6a57d02b8036be6d76f7258d91d6ce90aa99e9cd79086fbe484a" },
                { "hi-IN", "ed9158b58866c286c24bbdf36a106aa21023bd0221d8996513a34e2453e82e9ad3ceec2ac1e1b6a3e4ae98f173d3bd5b8c8e9f701e87a8fb141d3182862ad168" },
                { "hr", "a0486ab41dca1d155321915be045daa94cf5400ad1682e2a656d19099d386cc0436d2258b60eb49844648783258a7d1224ce37f8f8551f31a864f98eabc5d03c" },
                { "hsb", "b051adc45cf8cf2d31995893e70581bfe713324b10f20f9b741b2aa76fa6c98323d817447ba2589c16af52194cfbcaffa2ecc428722a31f3a244feea1308ee5f" },
                { "hu", "18132d789094deaa8b3fafa3ed61fe81af1d3c23b9ed22ecda1a6a592e171775eb52fe5e4e961acc1e06b9952f8cb916abfd30869a23e06cca08e1da9d3ed724" },
                { "hy-AM", "72de6fba589493fb289373f898b5b3927539ebabe4df08dd33319cbfa70e78fe7a8fe28f1a63b8a8d538596f3e7498bde16c65ec8b7be3e3e0f6ed9c800c43f8" },
                { "ia", "b3f42c5a7ebea7784343b289598855849e5faa64b8cd10dd7d39f91b19fe5b66f09312549ff27432694eefbadcd064272dcc64cf3c2309e6f3dfe44c286085fb" },
                { "id", "0069ef6548a4a7027bf6fc51e17eb3ef57a473081821dc5818bb2ce8d9977ef7b971a212e801fd9c899ad3d4a1e0d72abe58c70b7e3110bafd1822a7e636db71" },
                { "is", "2ca99230c5e7c19f7fe8fbc1f1cb28e30c62476665fe430fb40369749eda5a510998de4ba68b1f3fa5f401c39af6334bc7cf302fe1d50099b6a890ecfeb6305a" },
                { "it", "d7771e792f164499815294595409b232aa0d935dc4c97f9585fa3766edc43bf0bbf63519cdf2b7e94faad68a7459dc32bd16c0ddf49fd050d2faa6b500ffc992" },
                { "ja", "e267ca414deff440a573b59a12bfd40cb82b8498bdc72de4f1bed3235db6f9a4c9ad93ace521324609b08f5cc7fadf82ba734861cd426cd0bcfcc44289110905" },
                { "ka", "b4c7614b330d5b84001b902e72fcac72539b2ac1da2185a2f5b027da2bdb8c1c04afc60bdeda67853cba8f7ad7fa42e5facb4615a04e6e0e37da28557206e9f1" },
                { "kab", "ffe44bfb3e72e4ffcab07adff7b15bd730d0e9ed8c9e51a5c4ab13ca550d7fd84e8563092b16406de3d5612057541ce08139dbcd2f064b398175042508d540a1" },
                { "kk", "4345c8088e126e4c25ebf60c47bce4be6cb74ead5cb413c5f85a01958b55638e4f45654c89be5839a5d0682fdb978d2cfba91990923f765fbae3d5e3533217e6" },
                { "km", "63c6db58329b9f239e22cb2e6e0e87a666bc09420ee12050557c118ca2f8f1583f2c462c56b8cfc51b3b3c9041fea4cd9cc543dc3bc1284ab722efc0a62c22f8" },
                { "kn", "7b1a03bf51d476d39b6c7c21e50be448afddcc71065cfd84a0eadb37b7380759ef273c017b7a06c81cfd15c358dcb5733026b4b873030461db59dcd7b8e8e5d1" },
                { "ko", "84141f9bfae8ad55ba77ec6588a711e490bea53e0faae9663667fb416c5d20b54f4a1fdd1359e738c742c4c79204463c189052b5ee4a37a86810e6af22ed0145" },
                { "lij", "f2e91abf6c3b6c1e545149e898f7acce79279fe19c71b0dca0cd5b20b4918f20dadaea8ec3dc62ee61c25692aaaccc239317a25e34dce80d1f16d5f4cb26b1db" },
                { "lt", "7231a50dae6b0ab111722af14644c702d1995e5f60673a7604c46a66a403bc52c5f913c9d8d2ea712be7f9d0e9dd3fdc69d2526a5b792ecef4d9bf380528ea3c" },
                { "lv", "dca5f14d60087245ef0f2c02efbaa58bb846dcdb114b0b91e4b46ac187454b34803835df0036dfaf2abe7097bac27ee3db606ff53d3f7cff6c460395da774152" },
                { "mk", "33fe070dfc10a65e6f9596dc5d1e29cb9685b4a3b4d6994b000bb9b7d3579f945489f37ff40680e34ffe69d14dd6e41348e4f09003e5bb1c3697bf7816a88832" },
                { "mr", "14ee93b5ff00400c66c0166a5d1ed7ecc6e1f08366786a3381e8f49f501dbb5ad3f5078ee74f0fe5b433d25339f65bc3ab1636116dde9874221bdb03f0ceb4a6" },
                { "ms", "b375b217f83aaf57bf979b3540ed5eaed6323eda3595e21f1d122c8a585b902a941dd35014ef9a9df42ff846022babb0c13b1ee4fe91d52509f162bf12542a83" },
                { "my", "31c93792af93c02c1069fd00ddc1ef390005f8886c3797c2275a8501532192f11233b1ed16b9615151ab5b6cd52cc5b7d8842376fc82047a845c1d20467cd7b4" },
                { "nb-NO", "b25e28f207a7df765854ea63bb234729df06e7ef6c253eef66d13e5b5ccdb73f6e308caee83cbd74e32a3ac6f268b5973c88f3b1f03fb08715dd886b90402877" },
                { "ne-NP", "a2102a0c6afeeeb87e83e5b470e1a17c79c79852c41b99c349b39acc23dc5bc330f34136499f6a6807845cf85341c418fa995fae389799035448ce779cb72b81" },
                { "nl", "86925baf1d6fbc32f1d7869731791247dd9c03388b7dc5e387c59a8d6bc2a59a903176a940d35ee3668ba429aa606c7dac3723c233629e151029db2d7d53b403" },
                { "nn-NO", "cc4989401bdb613bc3e1ca1a9c7b26aa00648939a3586e0250413d23700c536c33c8b7d3b3a7d172858724f7fc0990df447678645b93b2dd06dacb792cc6c370" },
                { "oc", "8d47b36d381edcf6041abd5a6c9d55d7b2171fbb40524ecfcda025b5b11f7df9ad73000766ae1cda36ddc569e763f2b0b8cb6ec77a01e9317658a1fee818c3e6" },
                { "pa-IN", "152861756e2badcc4ac093b9e5e484b917e6469b6b033207cc8d3a1acd41ec70d14cf94bec082c6119dd38df4961701f917790387f1131624f72cf73702d8da1" },
                { "pl", "3a8c26060c1c2c5299a7ab0d0adc469cb3b85ab947c756d0be4767f7462a64196d8f57cfe01128e0d94fc6dc8dc7d0bdceaa1f5187627f70814320e4485a2bd9" },
                { "pt-BR", "2997aeb1fa89bd00313f30b7dfd04210e4f161b462ff26946596b58cae2f40f029104847f32d51db6f0630ca22a7eff93d1bcd672bc5927e8280a096719a0e8e" },
                { "pt-PT", "2e28f8c173d9cf9e8d396118091511dfe826884686fe622b0427a95f99d7cd9a29389970940cb6887a79f58f21afa666d41c209714acd9bbee9ae8344e53f5d0" },
                { "rm", "f07034adb9b3be4a431f0e026e73555a4d634cd2c90a02b943b6b1814251c1a864565300625d5258a7da94402e92fc1811ad24e7b27700f45132f26ce7a23bef" },
                { "ro", "a3e68b696271b89179654919c219a7df51ed6a752e6d4b1abcb282c1c165a7d0020735dc71f4e7021a9b0a35f9b366699842c80f86d8e8a7407fd33d3ca0af16" },
                { "ru", "a318599070bc9a357ac9a8bf841e625b13d8f867c840acbc3028fee5a015833f2cd07d072e2a6b33ba31785879ada1f76778461dc1ad4d0984dee3cf449a6d1a" },
                { "sat", "93ad9893106f98d209a72f103bb30518e55fd67ea6d78e89637a2a85c256168ab435703b8eb33d8c0ad1a2227cb9ab10ff0481995f0942d94c26cc646520a53a" },
                { "sc", "8e9be59f8fe9c19dda9f382c584cdc2f10e7148d2541f2fe1dce3cb74dcfd0e0d4c8fbd97c49f7b035d9bccaa8b57b1ee6d42f0a673708574c2d2f1771bece98" },
                { "sco", "df95f10dc0ae621cbe36fd87c84461fa1921ead6b4998378402c7b75c64fe391964efeeefe6c80b527d2294861e9969e0e9829c3f96630ccb758c50cb7b819c5" },
                { "si", "9d9b5ded8ee774ac088b4e33209844b7cfe82fe3d845c23887cc61297da114450319288eb86119c3498ddec69f9e9a1251a4bff61c99e4dd80c6fa4b18974a44" },
                { "sk", "cc6eb60d31182e214fe6de40f63466e66a325605f8908f4a9479aefc33ac0ed1ffdf1ec04e15f9ccade9302426981247b5f53a536d60384f6694c573929bc542" },
                { "sl", "08baf5055e5c37237aa2a19b9325b773bb1c84f1ac2cca62bd1de8f59c2c8411ca1fe1839776c12fd7f6fb41f35c7e7b0707e9e3184c578b163367e4b00d66a3" },
                { "son", "10af160a53db72ecaf61db0b80a75412c41ad29424d6158748d02a06d91c9e18a8578d7fff874fec7aeb79e9d44a172ab50dd880745bece34a0e6feac053e6cd" },
                { "sq", "ba075e2366bbbe9503f64362d3d7ee591b194f38a1455aa2a0d0e47b6c36d3e5e58b25b4eecdcfb060bdc5cee262aaff6b9022afefb277f02c72df62c7383f3f" },
                { "sr", "6e67d6e1fa5f92dbd3d1863cf5d1455ffdf431fa12487e46c045b5b5044f29d7f896e5923231e7a8525bf3c4600db2423210e98d8c2d7e22283cecab870db0ca" },
                { "sv-SE", "1ee8d531a7410fdb74f805218afe2afc6421dc1dbc962bc2675e3adafca17f8774a6f9a93d5cc82d98b821acd5c3074abc41240f728b0bb72735391b9ade5ff4" },
                { "szl", "1f55d64ac7b74301f0ad6f3d1d708dd38dd8765c4bab782914b00bd4f011d92f58f6fbdf3322e7feed614ecfda4e5a5d0b20beb299a69a6516f9198dcce23eff" },
                { "ta", "81f9ea825cc787f058244b74bc3834554f1993ecbbad3ed8ac71c44414daff4922b7e8c7b3323a2b08fd2f8a19baebc6e2054b0cf0e31c532f22231775a5ce22" },
                { "te", "7fb8235d76f1458ac65a32f68bdb2913d7493376aff4908c0d69e8cfa33a0deb6cd964e5e9f98232bcbe2f63cbc4c2e431e95377d73cd6aa4f39912b886f6d24" },
                { "tg", "4e07831bf5e1c3de71c5fa6caf39006088a1d900557b1cda76188ceb34737bee47ff9e2f2db97427a590e64ba06268177183b4e119471c276e96bd881ad267bc" },
                { "th", "a868fd618192c71144ecb2d2d5b4c84c3ce0f98288eee6685ac42d5018d567ba9f6791c001d8a7bf496876e24b973a49fee4499450316b198f50588a10c09880" },
                { "tl", "b68c1942da7ea7602315e74169886aad80291047a790b7ab43afe837c5ba97b182d3c1f67c60338240ed65d48523c787c4004f3677d52b8406635800e0521df6" },
                { "tr", "1c49652d79199f1ec233aea40dfaa9b40ec39c3ba5344bd5206b7f87823c06471c448c61d0da905f634bfe7a4fb0fa72af83ddccf84e6ff695060b23792e58e8" },
                { "trs", "b02cf92c23aa25a4d618085bf9c350be2e3d851c086513709b91378aa2215d3002c07f1f1f29b7abacb4611942d5240c156437074bca85e66ac339798a228cff" },
                { "uk", "63364829d55d9fcf4af71bca72a8a0f8dd2b90f2401cbe9e986d926802d82556390327dedb14f0fa4eb345a2f2088c9f47a59a444f3476af3af24fcd9cdba291" },
                { "ur", "f62509b4664dd9a1035872fb06675c32ca9bbbf9d20fa9af95bf764cbbd3095e66dfcdb374c67cb66d13c0db6c18d20d2d8d2262819a59ddfebeffad75210e4e" },
                { "uz", "a0ba133f304554019415ddc1ac96c8953ea44ff5718e4301e070e088d638b346ddc48dc337922e87ec823ac89c95047b8eb1506c45e83cad65821e79166b6d5d" },
                { "vi", "5f20f736c987f1c959c32ca77498b55e88d68016259a07303fcbc8eda485d3deed055fed2de8ef8a9d64a66b83f6c2a797339e9fc2d5cf8d6a1d6d9c821e677f" },
                { "xh", "269135841386f671c1e6af93fcd5412b0d7554364d2d7e9114b76cb71e075758b1864db60ef424e6d92444ac981a5247a7d17b9510a5228361de109b1a4c2bb7" },
                { "zh-CN", "d55a48fb7996d92c0cc39d85321f6c0170f5ecd1265370488b0a91885158480bc9ba29b1316b9e513d2367e4b908300d78ad0f6e6c0ed3d6aad0510c25660b32" },
                { "zh-TW", "12c5dd52e3fa29bcba56a0568b7e5365e47e8c28ad8faa027ae6b29f095376faadfc595d2d3b51bd1c6e7004973409ff6dc787bbcf6eaba2bebf8741074f7c9f" }
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
            const string knownVersion = "124.0.1";
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
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
