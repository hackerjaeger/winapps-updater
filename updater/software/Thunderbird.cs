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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.2.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "9973dee388dfc6c6000148aa92d5f51fba84f84e2392e0f156fba6aba1e7955950c544ad3e7a55e1965f791b4056b095798f806f4d485905e6149661ee51d505" },
                { "ar", "190940e0fcdf19dc9cbbd49ba21a9d85131649772025952f8ebe38a9bc26a3e9459e8a7478443274045a72ebd1cf72cd6e51d53a6bee1b9e9934bca04f05aadd" },
                { "ast", "0bd75b44b173c19668774f1c4bc0c6d7962f21400f61aaaa7d850a1e4bb5369aa3b462f02067c5a69d0ea4d9c0343c6585cb84a0ac0b9deea56dc8bfcf7d409d" },
                { "be", "a33856a9456048483ec93adc698d44220455f7b9bff2429baec3ce8a7dff94857d0c43a8076e63783e7b6a9d590958ec8abb59d27d90797b6465b80b8cd8778d" },
                { "bg", "af5ebca01f8c9f552ddd248b1f52b210ef1d6a7da43ba3ed5cef1d7057f85d55ee1833096c778e8a9bfb30ad78b3d595f6f85e822e1376a02fa49fc3b31029fb" },
                { "br", "29dd5df8bfda2b49a75913f5f7894f9d7968a4c5271746b921ea7f690375793113bc0e8ef9dd9a9c2a4faae7fa42a6971b1e40b6fd419b93dbe2d40af1a49b40" },
                { "ca", "78b680920623fcbf34bd02391a0f879aafff3f3d97da0fbef3e192657f222e32509ae008afbe84c07e7f8dc07ff71dc3aaddb1b00f9062f9ed662f53c22c2d3e" },
                { "cak", "14970f8b7ffb08b8f77f510719659d9bfb99a202465b2c3f4c347e37d51cbaa27446405359929c3c8bb3dcd50ee40f2e7293f721a5a2aa94e4b6940d45d74e6e" },
                { "cs", "b9e8bfa943e381867bdabb22699f7a7d3ae6c018a0134df26829b9311a22c3a9496b4c46d53f6f6b0928aa4edb74f0e682769ebaacc2faae304b09ece27d4683" },
                { "cy", "1423970063783f16fd72c3d81541f03a06a0dc1a1b7c8fad913ce4d568347c0ca016043e643065259a6c123066ef9a3402cb08e3d6e2326d30af0513fbeb8556" },
                { "da", "42c7378cdf7258a1497c4ac64c7c20a58e7ee8ad2b8aee53be9d81bdceecedc9f6790bedc607af4af578b8ba3a01cd03fb5a8a2a9070a7f0c08576e7e2b30681" },
                { "de", "512ca05cdafd072a4067c4353c00bc42c965c669b587e0d088ba0709fcc3d4b1478e5a0540396c713210b3142e8f874d83d1137d14649bb16696027f196b41a9" },
                { "dsb", "4e5e074e7d54e187b15e196bf50b87b80cf1662535027212ca09e5799fa8dd38e84cb5ad91da654e7420a98fd3b879ca994950f42282ca4162c11c1bf9aef23e" },
                { "el", "56dc36cc5349a2e5567169c25711b4e2e0de9e940e96b5451a02a7a64e5ff65629054a6d27c593f84f24742f27604aca1a8b634cf7f351e785851858f1cb0076" },
                { "en-CA", "1d85abe585ecb77d1fb049a83f26c7ae9e636d6b76708992a674fee4af2709fb76c05e6ac5eb5e267a9a35a491dbd3b8ef4fe1e4f1d34cd992121e800edc9477" },
                { "en-GB", "4341fad94498f4e2bf34163569ea422612215b2773ae73f641f264bc296065ce288ac4e214186d404005e696b30660942b09e3ab433c7ba5cfb45a1ae4005a5c" },
                { "en-US", "6144ed5e4362d0600f376fee4edaccc70b0cc149b7a135ca79e2fa0053914a55cb69a548d177443bf07d0e3cc484564050721507345e402441f0ae1bfaecdce6" },
                { "es-AR", "766f4059afb213429ae5bbdfba4b4ca2517461719b7c9e4dd28f08f1791ca8dcd6d4f293305a14f8e9bd780ce919a53172baca6c00d1b0c8afb217625816ccc4" },
                { "es-ES", "992c45e69ea5bbbed0739d54bef8190bce2f5bc6b8cb5ab0aecb687f58a731ef1420b464ab67e3e86acc2d06fb215cde969b7fe8938d7dd7071f6ff8687ec265" },
                { "es-MX", "c9c4444a6b1cc6936254a8f62850d9c54870e3b5cefaa5872c17f37ab82a703af8af89056e19a33611553a1b581a0547623be8fdb176234f53192d3ff9108346" },
                { "et", "3e50a633e20c5f7c5d965b069c53eec1a9f0e995ad5c3933d69cb90de77d8b07377d3991b43bf1b6ac22a55e56194eb8039df4fe2bbe16d03ca7ba71da0e5b68" },
                { "eu", "8d5313c2d1d8544ea6f73ea69734bb639b8eb4504f6f1aaceaf18fa10dfc36cb658bd111d7813f29a33e8af282dd6226aa6d8649b36a0e19828db2c8d0fbe70b" },
                { "fi", "d42cfb064caf162a7390aee2bc58b5f6216d997debecfbe04c1ae46f86717d3afe71d80c7d5b58d681779d3a72c6bc5ab043a3c93ba73446047de4a20ff942db" },
                { "fr", "38421ffcef827860f6b81b188e8e161226451fc390951647bd0a942b3e05a641d6b059114340b9d37c335a99a5d320c86cc16ababd2b24f8f4db21240e39f22e" },
                { "fy-NL", "9248a01a78dfb55f8685ba44a1aa6c20bf49b7c2628861ced2c6fc73225905ce40a4488cc3a687bfac5d90abac37a7cee492a76819077f5c18cb10cb1d798da7" },
                { "ga-IE", "0cbacf1e1dd64ebfd0de51e33eb0aeff70b5c42f6aca325f8dbd3ea514db991efb162b80dbe0f14bd87be0716cf33576247decb7500552efa3dc54bca28310d3" },
                { "gd", "5bb97f3424a99fc9e8b2424334021a7c14a4d0bdaab0e5ecabf2c9af2ec4baca4758f2186b24b7aa3097266281363df0b462014e8d46dc3d56bfc2e362d51728" },
                { "gl", "fd6c067383c36444e6421db3e3484adc79037da50ddcbd1bc4a88b46d6c175558d949285d05d6bdafff9d40fcc3b0f9c0993875399d27be5c9762d660a584aca" },
                { "he", "32e28cc266b625873fef1ff1ef56e4869c26aae2f058f2ee07988d3daca95d6663a6d002120e66f3fb9a25c0b315ae1994f34800e063ca42a9d78273c3c71c0f" },
                { "hr", "a67d472db5980e8baace9343b211bf101fb9461e4f68b10db75cf8c0e48f418ff2f6a7e153c96dc3c59b1adf7f284fec3dda3125dd3f400523e3e014b597f85b" },
                { "hsb", "5ce8355b8f087fe5f32ebfdf31ab6dbdcd3176a09c3fe672a3f25ff3c60c1c2d9ad5f10cde36af931505b4d7142814cf89dd611a0ccebd3cdb3357e89b826c17" },
                { "hu", "a07a46d744d13c6f8038095b4572f629271d4a69a25fb246524496db2eb2ce74b47057cfd449ce679e94641445a4738e58d53986e9ae052f337ecc87b161a61d" },
                { "hy-AM", "8e2f87070b1f094fa15226335b267368d7c9e244cc0ab33433b6f1f8bac647a82a96c4a3132a4d4d61f864ea18ae62f22cd7afb9948c557927bc8c952d6bcbfb" },
                { "id", "8f76c55d29fae27fafe90753c1d89842188ff365e047ba37bf165775321995b633703fc4ba2e0c4d794241d85387bf8cdd2891c549809580d16cec1e430baa0f" },
                { "is", "0d65bd8cc2f1ec28a29cf53ac7ea14b23847e322897725bc56d7143a2b999e418e4972c58ed9b50885f4883c683a536fde915eaf9e4400af12421ef15983d84f" },
                { "it", "5ea732d8edb54732ed9bbd0d8b3a6bcd75a0ab43ccad6a1243560f2bf06192a086bf1ca77c2c3a489b8bb845b347749f03d7aed57bbf29c88127b297a749b2bd" },
                { "ja", "936a2aab864c2d694f322bb17a78e65abb94f64aa8ee8d25cca39ad2678c3763e58fef56a14cd6398665577c2c0d7e281a8e04178dc7c444541fba020c81ec8e" },
                { "ka", "9ecc54d600214fdc713e8ca566f2969bdc5b2820412d0304b8161a2ee015ebb2207e13d4364d39ee162e2ee617e412c59d12b73fac08d8fffcfc0962b4039415" },
                { "kab", "428bf8ce2301344520b32d5910423924b3505415213d9aa63135e1b91a4147f30572e28be16406477877b65bef1fd0f4eb667f638eebfa8e8773e84a098c9dbc" },
                { "kk", "eed6efb19de4d6c20f2a99200ddfe874c7b91010e3abd39e5e27d485312748f5fb2714987dc47aff05ed3afb0484d8ab43c05e97fbf9dca9e7fd1188fd009f41" },
                { "ko", "2b7afe38f99f3e64684db0fadf6cc6c52e400c644767a33e74ce127cbc5d4be76f8301b2ae84ff041e79f4a7a78e5d904976ad244cc5ab0228019702623550f9" },
                { "lt", "0db6937df19a4b5fc21031a347139e46cf31856dcc82566187a36fbecb0e9b718372166a55c1d3fb094d659f09b7645629223cb2bfff4bb4481f26799efe674f" },
                { "lv", "a1eab95b6b47939921906f95f95a51a05620c6b2a67cf780cdf6195d8433c9d2187d4a0f4cde4dcabaf3f189f822b73647f9766f981c0c505b491d55f35e1a60" },
                { "ms", "c40aee3cb8bcb82bf647242529f261f60fa0a0ba3784577785db672514669f5179026786f89262c470a9a22264e7365590aac05c57da4d6e3aa662705a67c9bc" },
                { "nb-NO", "81c41ba9a269bf5f7182db9d6833884cf9dc9fea9991ae3d63b812c61e68de3bc05a0e367d14803487e201364a8adc68197acaf416a2f5669a27bb85cc2e74f6" },
                { "nl", "c9d9e01be88d8b168315113178133fc1e12d99302131ce977143649e20c1332da301865686909308c53c608bda7952287242b40a9b7b1c700d7e913f0e7f2c80" },
                { "nn-NO", "734f24ad1e0987adf304bb678d1e02e9d937639c93f07e165afdaed9090f06dff466e23c175eebdfff1ddbaaa5b429f4aed8dda1acaa70578f4ba3ff3d31e96b" },
                { "pa-IN", "41a9941499597f95386be20f6082c29b296cd1f91259d9b396e62af8085029bfcad87d0aa749152182e32e54d09a33a0f218ade291fed077fff74ca1c87a2410" },
                { "pl", "34159669afdcb8c192627c374551715b906d953c1e50e0a9f07421e617a1ef67f626a0d24833d22b0919c7079b25c06b9cc28eee173ac6ce7dc72219644bba55" },
                { "pt-BR", "1a4a505f2ed1af8bd24466401421cc22d1895417c95202ea229d5da40f626798d614a4a900dd4a5cf056511259794e80e5aff5576ac7add8fb8b565645a88f70" },
                { "pt-PT", "af4d1d8115e64230ddcf913247ecc0ddce155a3c37f050786de1e9d624550f552f496f26678fc0cc4b034cfa69242bb3499a7d26c009453f5e9b73d897817504" },
                { "rm", "0c29402dac6a3b2141be0c35ed56172ff54ea281ac61e0fd2a174faa8bc644d5ed1c3de721936448a5be6c21f15eeb30418700ece0805da5daca0795754e8f78" },
                { "ro", "7933dc32bf3eda787130b804372a55b165347c76ccf883da6f46ef8da6818d4034fad92a20930fcce164f0e6766a2df60af52b4b789103713a4f9a1924288a6f" },
                { "ru", "9f70a90882acb5e7721de3eb044ed5f31fd8a1e17a69908ef58ecbd407aab0a7ff60038bfea857c83bceabcf7303a7c18b0ef6e11e894fe8145f956a606ab05a" },
                { "sk", "c18e0f14d6b373e87cd214c04d14960e313f3061a53ba69b6d2c6a14800b1872b3de2547ffce4451729f31b98a2eb6a2672f1b3fca3270b4a6586c151fcd5374" },
                { "sl", "b17da83eb6fcb585edcc0630f322db1f8088114deae8fd5cb2909eb83852d82d9d46d6e4d0a973416db93d135fbba1011900d50641348dd3bf2aa4a44f6346c0" },
                { "sq", "5232db13babd81a7f862b1d7b4d9c50f8fd0d622eba06f59b98f82e5a99827db688e4313163646442c89caafa1ea24483154075fbd5c30e4e323870eb2d12834" },
                { "sr", "f3d3b952b008c764f6152f89e5517fc624d8160f254bc3072e231b47b99fcf0815c567f4643f6dbf2703058978fcd3ffba7ce2a2ae556928b85a8f16f8eca540" },
                { "sv-SE", "95ef007ce42dd5ca560c38cdf7e3104129d527edba001c28f9c94c313c1398cb37956af84020210707e8b8c9a7ca5a609eeeef959fdb399357d78739be338299" },
                { "th", "1564322ed6ded0b7aa2c3acc1d81aaeb8f3cdd1319ab29679ecfdfb777a678630eaa6878f3dec3cf3f23b748afe4dd1badbc6a945d1cc01b96ee6979246b6906" },
                { "tr", "ba56d2921b3b07cebef04fa4a5676a086cb02a2ea53eccf5257008662bcaf05c02f9cc14a0dfc679c203e1f2ba8bd2d63cae7057ed79216000bc24bb23c1e3d9" },
                { "uk", "cb813a1662e7ba6ed3269a7c7135ed2cb27a8b711216a01fd376e80de060dfbd090175c153a4419a6d78719e3cdf4066f5e31e83144ee6b9eadf30803dec1fd0" },
                { "uz", "5e86652859d573969585a9b9a8f79201d7f3fc0172e42090771b05b8597a3bd9d6727713bbe27f56eb81f8c101e1885327f0c90c32e9acce5b518fdadd2dbfb6" },
                { "vi", "50105a475e0bb3aff89c0186a9518a18374c2f0a4f0bcf9d266aa124f0fa0b1616c23bbc457aa0d9a4330ab378acf5a318e2188efa210d3c30382c41a180b958" },
                { "zh-CN", "358a4ce421c49223de487ebf58b5da48182bbe4c7afbf7eb3f0694a96110169dbfd2ccb1b3ee98a28e318b0403d0ea8a5513c1d8f3da4fb4929f496055423189" },
                { "zh-TW", "2e474414bdeb7d78e9ff67b87e47b9b16e2ed052e66297542154300a360d1d59728ba8b811112bf51dd293b3eb9a7f25108fdec37c179faef2888da05da18be4" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.2.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "1f35b329ec2f52cd82879d92e7da70445272e5688c1e3dda41b24ff7286c92b3eb482de8ae3a698106af9b875d6052b1f3952fb28968f5d6ce7a14cdbd254bca" },
                { "ar", "1f5b2bc464f588ddb0a6c861e55fcad77322dc24cce33a95d014235d4e770a5b2cc6c65454044e75bb844d0572cacf43acea7e75938f2d89b47530faa7332880" },
                { "ast", "9b36b037b680625bda544abe81c86c429a04fd99e9071bad9ce4e0f7bc2ffbaa4c5d58c4a5fb688f9e8823d1f3b635914307f1881b65f28efe96f4d2ce610288" },
                { "be", "b41cc0ac940ec55135d11bf461233c0ce08fb582d5ba5888c604df0b33494a6a7fe69b1968f81b1c3f0d3ff737ce52e22432b4618c77a3d5b301d54a1ebce462" },
                { "bg", "a5d4fda6f605ce0516e92cf223501885d0407d1dbe57e7415a1a275de6c42d64810f42ae4c2ca2d06c39c47dafc4ce99dead97bdd1b0b414a5a24e704da00433" },
                { "br", "4409a1cf26e1d731ce3bbc4d677f43b4498b537a437e697e281cfdbff320265b053ef1de1bcea8bfab5219413ba7ea547010a3e1798561b928e9e86d7abb31c0" },
                { "ca", "efcb149cba88746884a3f9339cc5ba8d8eb9e43396e6e4b5b443399cfb2b8552cf1983cbaa772f1085f6f730a86b6e3b0abfea272fdd0c94d153289e6d174829" },
                { "cak", "034b9ebe2a29b43355e9a37a6ac2158afd38f889387bfda1e97060a9fcc60ef573788c04ca595fb1a6c6fe965aa51d6d591fa49a1e3b019f591dcc55af9ba2d3" },
                { "cs", "4dd8c402a2fd67992dc5491848ab97a7dc1b0fa1c87e50b801aed3ca6a2c3b78751207e4baef2a400f80816cafd292ccfbea45f2d4f86101c99556c02b51bc98" },
                { "cy", "3e1c63c008a1666610b9f8bf64ab987c6c84577cc41aa3d00729b58a045b804b6efbdad8387faa91d2b261bfff6a0f026411dcf0ef813a20f6930bd0beafe737" },
                { "da", "b8e1b8ef240499364e47e5a77c20577eede0fafdc37720eaa32803ca40eadeb52220df1cb1d014991bda96dedcc335d013da93bdcea106202753e7c70ac36324" },
                { "de", "99fcdc0ebeeb9dd93d6847b46555e923e16ae62c395a740e55750c164ff345706cf9423257cfcd89bcbe7af3ed5fb94d1e8e544d3debc531d9b1d6ce414f3be8" },
                { "dsb", "d8d642fe290add023d28acde7919436905e10f87afb4b58442b71119f032be19d677aad053a27b5347ca7be05678358b3af525f3f7d95e73359dc1ad7256d54e" },
                { "el", "c2a0d6989dcc9836331304e8674fd46e9fec38bda5b103ff53cfd510ef1072606b151a58520d5d3437cb2235803ced56b17abd72615cbe1f2a0b86a24c5fbf54" },
                { "en-CA", "8db6057deca0ead6ce179f8fcde8c3984b9b5313d2cb1262bd2b8366187349f0215453e514afe7d80987851be55e7ecea027d9eaf1897ba1b2163bc978eeed4f" },
                { "en-GB", "129e4e8f21b96e8debc6e36fa55a985f5c36cfc8d00f7c69e71a98a197acd9f70e893a32b213d766b13131519601727fb06355794e00a6bc9554dc4b9897a384" },
                { "en-US", "48b5a0682c014bc1e5f53e0393a67e5664374277641a75fd6ae804b1b8b7f7d4ded16a868fe6f51dbf93cd8ad9da10f842983a34ce9b8db1229dc2d7b21080a7" },
                { "es-AR", "2b71ffba23fc71b8e80c7ec17ec4c3596ea0c7bec5de87f7112241c36eb1266e1f489793726cc7f939545788dc05fc1488ba134e3121d183389c4420ae358938" },
                { "es-ES", "ec17cf672051622610d0b12318f9866960e7cde8afd195c249c466cf12e37425307f63cbb5aad07570e6813f5963372b84790b5524274f1988b8d523d0a55368" },
                { "es-MX", "fa507144334890e36e55af7f79d7551b5396bd2e3b7bb2d7245326b39f2ee48ee680c624e7b97b1a84116c0c631112e105b5da5f0cbba249b45de875b42ea0ce" },
                { "et", "dd9f14642258fba135781f4c5d53edb69abcac259756b5fbf877b27c2c3c36a12f9e19e09ee0b0bd1f371d4ac8680be9abb363cb3991cde20f428a553ce6299e" },
                { "eu", "fe2545b0c54cb65a64a9bac09929d971c2e0c9e154c12043099061da89e8ce04746c3c924f02cecc0f4fbd8e37cc503a1a8a9f87bae7e69e6b74e6620a354977" },
                { "fi", "0396406e094c92a6274e2c65dcda58b58db18bac3dd6dd22f3d0608fde43f911f2516a392bf12590684dd03b81b87d994f2ff280c4b8cb6e396ad96d0fa43a7e" },
                { "fr", "56928f0a95481a454da650ddbda2c8e893449850decf707e5424d5b16fe914b092b364c3dd5d9863f4f5bb2a8ebcef29830067b5c47dcca54db18f8be69a512d" },
                { "fy-NL", "b8bef33906622d575abb3f9fcf19380f02d0122c0ff6fe28bb216ba75550def09461f7979b5fdd0c5239c13e81e9b1a2a919bb2005067e58473729810d838bdb" },
                { "ga-IE", "0970b2148a2df4c1e975623d494116eb5036f23d33a87d3cfa419770a5a94a914d235e9b167d979e49c39a4b5ba13de58f8a4dc98a67d99fddd47e4ea72867cd" },
                { "gd", "f14edaceb89f0021a6f46ae9a87f43472b6b5ca9427cf28e6992094cce58f144c2201191398fe6d9efd04e1f3f5a94a0a42c473fccc2aa3d45774392b34d7cb5" },
                { "gl", "301c47562455958263bc9edfa21a1c99d2d5848fc113b97494528ab2fbbd1066dc5cecf394900b8462836f0bb88890eba8688a036e4e64109465a3392f04a73f" },
                { "he", "f41b2f5d6014798023b8ce5d4eba357693c80513e3f015828345ba1c61025bc66197b983ef0254b9d556c5c58407f20a54e288f1fd0f154546c8b0b5203443ef" },
                { "hr", "6c3158d22fbbfdd5aaffda67111117f5f2187b285acaeacf5a39fb2b6620fd8abc2abc41434039691b833d2e57446ec3ae26115f38a22b8ed934d20238828fb8" },
                { "hsb", "cd5343c5344109dc731a6c9f0dfc2561f07c769efa321d2aef6369e0baf1d52617691b3a9ec1d86e7591183eec2623a52f81170af2ec16d21ceaea4cf9759f80" },
                { "hu", "73a92373ef055a96e3966649862b80cd177ca2eddf6e69b3088407ed0a7fd2604eebf18babf8798d4b3a5b4b412af48ae6860ed1530857d9710d651ec8b647a9" },
                { "hy-AM", "19b4ce141f717ba6ec2835187c53f5441b91666d8a8cefc7720ba300ee47dfedfa943bd12baca1434cefcb702d33799f4d927f313baaf33e17c81ab5e55fa5f8" },
                { "id", "142c5a527c8c378ee243c793e8d290613d5ab85582bf61a261bfcb7b2fe539c28e90eafa629302f757abe73e2086e09c4fdb284977a6d7cb5b74341173a69e8b" },
                { "is", "3c1f40fbf147d73d55340f26ec9f018adf38c575aad3872da349bdb2f26aa0588ba3b10022496011bde69a2025f0104f75221530ddf32dc3c78fb891814d3394" },
                { "it", "fbde3a4429d0558c3117ed86df62695fdf21b25b2012abc4edb1852db3517402728f22e2444c1f657c4bf2bc7b780e68b35cdf1a1d99e44e1b77181cad676f0e" },
                { "ja", "aa4f9063384f199a5aa5aadd81f6ea72258564dabb2bf7d3a9559381cdc9fed19a39a240ce7992ea6b128c1f9913e4e1e3e9b9f5a1668c62a8106b6c9b5f498c" },
                { "ka", "d7f0efff58790f1685f6b278566ca0654333da998d4c8b6532b19f0290c58ed1ec39e704893ab0999e5fa0eafad5b18d60d0b9fc74d57054941b58cb38a515f7" },
                { "kab", "f30a43d5e88491838b0788510d0e11f0281799c15810038bb37e948d43a2c800c0f4ceb9ecca1db090a649b234e920b5219ee4eeabc21b4d1fede5182c4c697f" },
                { "kk", "bc2195d22256f89594c32100ae4d0cd0b34536048464f2b01861133f99025b2668a525f3b249392d842f26aeed7fcb18893e4008bbc91d8b1737825bb2a9e352" },
                { "ko", "36d4798c486001e49ec9e3737c75a7e5b1765dee1c3cbdd3dcd8688bff7356c7a772582b5028afbdc3ba2c44420dff878f2dd026873ba264c91578875e0b17d8" },
                { "lt", "0e689db94085e64245e7ca73aa9aba8341e0782238751b37f748dac3d63eb2fe89df7f3e869fbff5bb48b65ffd45086df717d1dd48fb0772f72b6dd48fb63032" },
                { "lv", "331fd456dcadc0524ae0b5ea04444715f34f12c4f5f4473149c88b7fee23ee0315c0681bfab011122c4cf0c73c9daff82b2a73912a1c2128fa9821dbe1d3e9dc" },
                { "ms", "03caae8773e9870eeff48478b0cb1c54f993f596a31d5fb82e59bd93e584cda5be3f25e8bcf076613da5c7d96dbc91a3016d7307148a0aa5aa31eda5183edaaf" },
                { "nb-NO", "a26bd7285f2ad54953f6c96780cc2478acfa920045fa6fbabbcafc0d229c35922c370eadea2de0ff76ab736dbb1665cf2023ac7582e71babfd900167f7cc1c74" },
                { "nl", "e2a08b007330f5c5f9186559ff5a6018a585ad5168a8fec0930ee2633b2f1e5499709f8c52c8add73306f63fc225364d30e7987efaabdfbf04dcc06ca0464c76" },
                { "nn-NO", "d296785fab0c14b8c053481627f3c70d1dda251bb619718014d5626b3d6f8e108d687fa537e62b7d63ca6f4c437ca7118cd0394d7b948fbc980bc85d0cae9e8c" },
                { "pa-IN", "cffa50ba509bbf67680778b66098e83d987d1612797dd09da2c91cc13ba18d1871081f3c802b4cbe86edee7b6693c6368c201862408308ab7973929391e6fe44" },
                { "pl", "6f37a1520cb308db57ef83b8e31c4bf9812e4220c513914ff1c6177f562f88434d17d1712b3e287c0e2d6aae11c40839e1ed0199a1807dd40f5d5fd2e8c73a24" },
                { "pt-BR", "ea8d6a717f5a04de712a83cab3db7cd3a99b8275948f543c99a6f48ba64904193266dda1cc99f055dd6e7ab2cf2bb986cd48494592c7858f6c43ea5c40d5bfdd" },
                { "pt-PT", "74394eb9c43b56034561fc14e531b1a08f5b7eb9caed002e50304e9f0a8bba146235bf46c826c8c9a617d45011229f16d3e6764b50dc2ff43f01ed4d5f1a6968" },
                { "rm", "d793d8509c0996c9b4132d150cac9e0cdd161d1e778eb9aa41d0743499dc45287ccf3ae81862e8a5e1e781a83c42920633364c57c8bb08c39ff90fbcd5d75fbb" },
                { "ro", "efc54566b27ec0a48ae6ee6e37d88b2c0f60eebcd908759faf023e7a13bc4336c245e00b0e25ce2f7034103482155bdb99c49f96eb379ab3fe0a0bfbe583aad2" },
                { "ru", "391ff1735670a85ae8bd77c26edae286a7e5d420a7dc10a5dad6c190f7bf438dfb5ddaa0d09e833f9e8e84bf5d9bd7f3dc048592d01b08e40275cef3ba3f88d8" },
                { "sk", "3641343807d979e5c7ceaccea1451deee27a61c80306372db2ce43a31611b6e6ecf80ddf186bbc49e4604634066344b246a2bedf619e51bb14da4a92fa328240" },
                { "sl", "e035dac7023d62e38ffd953f62102cf6e684716a6475cd697be5f6467fc02e947afdf4e0b71c90f2bfe9920b2e6e8abc3ce2a8ebf7d196e1cbe290273fc9d304" },
                { "sq", "aa9d182fc23f209d2a2f8876347491db6ba164dec12c4ca39931b573301f4f4fd61894bb7651408c47ff9e42efce77205cdfb582cfb020a39b718ffd5d23896e" },
                { "sr", "5120bbc976f940664e9b16d3e915d73c31be3fe28e97eeb1b90594851febfc89fece243a5df73c86d005f2f9607d0907a5c3be9847ece817df79d2166084cc61" },
                { "sv-SE", "eaa7fa569fc6a7a629c97f3e146a9dc4c2b18cdf72e451f4d1532e9758e54f4a7c71c8929b3db5114c1cdc0dd05781a5c56ec54691159b9d3e58db5ad4f5cdad" },
                { "th", "1aabb8ca78fdbf8507681bd43c0c8d5e95d7d98c4162ff76b8ec474da2d55c678790f510fbcf9921c31d704907e26ef98f65ff8887dd987b51f5175d889bd4d4" },
                { "tr", "8e477dc8965e2f41e7357770167d9e38e80f099b9791848fc9ef0cffcf26003babc67e70c4156eb0f4235c2f43df0d2d8425265a1e3523a4d23aebe916e4fe01" },
                { "uk", "d7040c7453311f74482b576e51e9a30b061d47aa8042526a00b1972a56d203d0bc51f27ade6a9bd745217e6de8d059dbcaa96359c3a9a44d2469fd35dc79ff63" },
                { "uz", "bab7a573d4729faee47833d843b9dd043bde73aa6b54c61349b64bdda1429e4ac58458d81090a266ba236c137c849239bf39f00ecb748064906c9feccde1b40f" },
                { "vi", "802ca48d3d6c30b2fd4db670e058357e8627a27ed54fd11ff064145c35bd677a3f80d377abee9202bedc16d8ce80ee0197f504f5425202f702b1d00ef3435595" },
                { "zh-CN", "5add8e5fd461c1f1d8806c224319f60baf6355d0f0db665aa98d680a8a8ad4fe5e9b6bd5b773b1751a7ad51be7b81852135feb408a1c589cd1685bb014547b10" },
                { "zh-TW", "1b5d6f0321c1fe0eb464294ddf71ff2f2a692ae013dd6922b89d0288bab02586b31c61e5a4f63cb6931a58039e1a5a8141add1a5c4df4d44a17e48e82798bdd5" }
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
            const string version = "115.2.1";
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
