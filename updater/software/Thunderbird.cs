﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.0.3/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "4fe7061ed3ae98f8c407bbce254758fe1205fa270323f48aea4c177f15ec2ad8f629ec381e46736e8e7b5999c3d26b7f60df6e4425f043eadef4282581ad08d2" },
                { "ar", "efb93676140c72a2c77e4eb525601fdaeb5e2c6f2a2993d322c098bd434a346b0f62031c376ca5931c9401b04f69f85d9bf32b52fb8e8ae14d3605c3a92a54f8" },
                { "ast", "800acc69f94e9bdc6f47b1a7ef040f678202e1a16daa8300070a185a551e3868ad54b18d0d23940fb588a4a23ef26608e0cbe325a06d803ae25084641105434c" },
                { "be", "62f2adc8b828a6012e56c4831af1ec6b774b54984ea33107a285d119dd4ef4d864a8b01add607aff142776913a1b27ffc48091a13928b663cf4b5aec527d2580" },
                { "bg", "49ceedff6bf9503a404214fe6d35ff6ff043c3febe82309158c98f736d25b835d9d0199c4b19652b8b491b777e2fb1a46d9f38354d23604d090c8c814403821c" },
                { "br", "d9b669f2728634b064ef243753756d950068f9ff5206d8859f11dfc14cc59611a69f2ca10b9061d72153e8e012420d7f29276c24c31023266f91f8dd6f22e8b1" },
                { "ca", "5a5921f913922e72c6d25aa11cb1c9a2fcbe0f102cbc95d91383ac1cce3d2d271b93f17aeffc998430ea7386d86597fc4a83c3b8539ca80217df65daeee0e402" },
                { "cak", "0cbb7c55fb90b40349b65480052e866c6fd20c7d7c217260fcbf1a645cfcbfcaceb373f2025ad3d631390ffea91aaa23dd8d0850fd51640dd3f6d0cfc8b2cbe8" },
                { "cs", "1bfe6a609810e68ccd36cb9d770edc51386be28be20a896611649d82baaef07309828ba0f7f1dc85cd639c19085e9bb3ccd7bf9146545a39628fa33ad8f276ca" },
                { "cy", "be7c11a83b81127b0cb5918bcc0b76c11aacfea67dd39f6f3a37b1448081f8ce0de770cf4aadc747fdc451b52f8fc969f1ffbc4cd7e268e122eb8bcc0ec6cc37" },
                { "da", "c35367c61347c034e2490964382fae8573176fc84d47bc8f1ac4244d0f4c8ad21880f142f3f0832ea3b34bbfbc7db20f40e54f1187c318321d23ed0d141d4995" },
                { "de", "b5d1c336e9d170b940fb1a739a50302986b77b093b07bedc8252d64ff04d314b4fb2f35a24e9389ab39d488afe220cb5b6247c63c3b466e00df375e5a344c5b4" },
                { "dsb", "41d0fb383601b57ebf456fb625e148a5041f99f6ceb5ed85163f5069649bb804b2f5e51e69e5c05e0c78ee1b968a583a82a4d4e6f45fe40c2da5fcc1d7c9122d" },
                { "el", "0c259b0a39ccaeab7ee9c6a20ac109cd9bd2a52399b2e43417db0a05c28b2528caa9f83b175eadef3db323ed01ee8656ee580d9b660cb67478c3ddb3c49e4e52" },
                { "en-CA", "6789c6dfa897c5035e3e5cf5f94fc7c25e2d1d9359b79f6cbce50826110f5673f3877a8c06a1570eecc6401bce9b1d05a77431ec4c216831f64c35d16bc5231e" },
                { "en-GB", "f6b54debf4c0c7d90d5af8001cb6513031af8c87fb63069d10b31d50d3013c3e17f24a5dcbce9915fce8d9881dc3ae82ddd3ad304da33561c1e3fcf3212f6d33" },
                { "en-US", "fa5e1e558168bf525387b22842a622b46bbe11a2d867dceac53fd511dca40c572ee46c8e33f7b644b3878c2687942687a577f74abc7cbdb36c75ef74054566e4" },
                { "es-AR", "f235b222bca4ae37eeaa9d9e220245088e134292ceaa9f9cd5b1c41edda7087ee455906f812737721fd98f80c9df2b9cb37dca6775812b69b4b0ba45f19eff8a" },
                { "es-ES", "974b892f80d6432310c2167ebd2f45dea63225a181853fcba628409b40606e25c16874bfd3ebd5d653933ae55ad8476ad1fa5e0ebaf0bf689d8b2e5806379ac1" },
                { "et", "34a84be2e28902958ece9a632efe8aef0a99a90187ade9c49f2bc02b27c41268b785b171cad4941cd4aa49fd64df8cc64cb5c6dddc5da25e3bbbeea16e651893" },
                { "eu", "444739756e9e7693065844bc4ce41ea4d4f136c487ff6b4035492b450e0287a24902baf612503177d8ed49fe24e357ee239cca4430e0151dd9addecbf82bd7e2" },
                { "fi", "b7fa9906415b197d6b14cf19d055c11c2ce0504e4876180182ac5d2c6943cf31db946c62c1f4e412f5d35d3a7d42d71883677a009334de0225a37737e7bec8a7" },
                { "fr", "f38c36a408bb5e5977e1476052c60f1e62d093d5849333c7c81f98eacb7c1abdf12b63387cd9f678552de7fc9e757ffc84156654306acc5e9b293ce5605d00cf" },
                { "fy-NL", "69a0ca15c7bc2de35449d195c7c44110e9d5101c67b222852da5c1f361da798503376655707b223dff5c04f1b8974faf37598740481d29b70e45150e3614d237" },
                { "ga-IE", "6b2e78fa722f6fd8f3299b118244a59d231467eade65acc7be3ae92349bee053cc9846b186889d387b4f478e232f1c6f88c9531e9a5c0161de0e9edfb97b13a7" },
                { "gd", "024ff39303d645923e3d50770273bfc9aeca5100b7fe592da6f7deb3b54170825f2a9be02b650f8e702a3392ca1a9143b60ecadb2155667285f3f3257b3c4bcb" },
                { "gl", "7ad8fee393433e20566ae7b66d8db6a26df109346ca907c163540c32ad48e5f7bf98567dae571bf60c85abd302156925a121d706d2134b0bd57288bfe0e0ef37" },
                { "he", "bf9c0a60a60d5fed13f83ef1efc339e9b5456a4328409dd5d4b8832662726e1fd0ddd76d3cff27eac7d26a49304349509a80c58e0ed7e8f85a2a9372787d410e" },
                { "hr", "4c38d324e63344b59bb413261cce9e84c76101279dffc482225d7260ff22a57a6cd5a991911243137a1c7b17dc1a5bbf380cdf0f1755a09e4daadb1afaddf175" },
                { "hsb", "de6dd4787fbef800bc130ff96cce4076d879cf6aefe0f2620d60af1bdd72289759656307caef78bf22e669ba17ce0e912a707177bf3b41988c5fa32f37d9d298" },
                { "hu", "2809458243d3506a6f8a864ba912b3dd3691071f3e1e127ea03b3b561dc01749c6866ca145e0b8a48ed3a2968d263b79f83dc63d59446dabe231b9bc6daeb838" },
                { "hy-AM", "a2475146bb9e7bf542ac5a492267aa335542afe0b840141a6d0165ef657ea719abb7edd61a17a50d72923285fbdc9acea8fa9f0bc9b36e0abc5404c13b552b76" },
                { "id", "94e25073e2f64523980bd098005f9eea5b1f28f505187779ab8de98a77cbc4d5ec9e0a461c7e3505d9ed30f1bdb7238337233912173930bd53cb97ee33bbf00f" },
                { "is", "203f27dac404563e526358c4dae4759286f53332566fdc458a4b59aba511332cd529f33ce3db06a17faaf37887b5badbba7b668172b1c1a3c0fb0f1486280124" },
                { "it", "96f06c9d606de3d2eeb9c515ed77d7b0431b8292ddd9c67bee17b5353973917b2f4e8e1469bc37a56c2cce185aa89d3cfc6c181de2c51dbf86253eca39027401" },
                { "ja", "0cbcc6e731ed31e81f5e2a3acd40acebf95ac42ce58a92fa92d5aa14a89870d93c8a6b046f578759ffb1f67f8c6b3ab1ad869587abfa5e834654207d585a3037" },
                { "ka", "b045fb3dbd7cfd7b18263bb7ea9a967087cdf61ad0a89825d0d5cd1025f42713eee8bec79b74f8bd4229df93dc3dd380e41ed1ace54dae7fcc9252a6445b3ffa" },
                { "kab", "086ac96342212dc61ad95a50e9500b3d3a88df532d5a45f6b9ba35a4013497514750c06c987ba7ba6a89380cf9965aaf539d11118db6dca980c72bfc01f28b5c" },
                { "kk", "9c9d4c9c423725fe6f84acd547999f9de3bcd95e1953d0aefb62fe05a58aa07ccb793f4588ffe34451983cd281efc2bf6c7fa1e87a2ea24e5dd1e1cd60ac91af" },
                { "ko", "8529669f5647a3d24595d22dbbce258607bcae3381cfb3897188a20e33ad7047cde0f3ca520e322759a9812b2042a625c65260b4ea05987aa4c8aa17d7dc8516" },
                { "lt", "9f1291c6f947b9c14523201adc86c4f0873fc3317341fd49976a2efff5cb4944fc5739a6ec147ed87ea451a69da66db6fd755255e10d7dff75193a437e06aaef" },
                { "lv", "f9e989064c70148b2a03bafe8c376d4a5b5d4dc79c9918c66cdd694893dafff97820e0d91d923fd0fec342694440a9cc283e4575435a75157778801625cc6b0b" },
                { "ms", "c2492247c12c4a5ce07d2869f13bb954c1ebf5082c6cbf198e11db499a4cb314447f232f680893cd6fac9ba542dc3ff5471cb2db8828bd972c8c6f28cd1a38cb" },
                { "nb-NO", "f69bf3c3e46dcf7b1082bd895e7925f21943b186ac4ebca405e5fd3bc20171bf7ea176d283aa3f9bac01ed255febde92c7d980b727fb0e7d745c2125fa9b523c" },
                { "nl", "1bf033890a5c32f84bc0219a63b70fd065742adf805187b8420696e67834823d34bd786f86a9aa6c4988be60b11c49ae94a8a9772555ec59bc0e7e4d5d4e15c6" },
                { "nn-NO", "059b8704e2ff2b75d6e92e07e71e81256147b16d3ffa73cd73f4338704a6b61cb1ae25806cf89ec0b0f556b035ba31822ecdb6e4d8cd218f54e8a3ff705bcf06" },
                { "pa-IN", "5343061f4b93319696c4ba92d4ee27f67062df15f94369ba2b0946f2db6729e1871bc389baa445a78c7a51d04859ec46a9b67bf8cde8ccaa24e4e49ac272e8ec" },
                { "pl", "b169dbb6f98f50f0a9dd60e285f8c37a32e3d17214807ca7c3a31f709105b9c4f095fc49b94cae2788e23b006b7c0fbd9995145947f295a843198710b2fb1c23" },
                { "pt-BR", "82a319c99cfa74b86be144a50c80b09398b4d3aaf68c19a5c0f90248672db0bb9fdf052d8372b94e2c8175b4bb2945255d6e4c2958b5c89d0ee790aaef477644" },
                { "pt-PT", "04e0956bbdaf88420470de780d60e9730903453943309f6b3669ba03fb53d7f6a2a4502463f058e45ef370dfcddad8dc1c4740049f755a8930becea2a4171c10" },
                { "rm", "49e628f04f08886d5c2eb73a66c26a7f7b020f9660af3df6b98d67815f56ea6e42c8da787cf8cecbc8df6122a7a5aa7516c3911bab19d84d4bd0695eb54af585" },
                { "ro", "fdc4330a05ccfd3cd25b2d5f89bfef46403b09bfde88ecf0eb48c7cdbf85a97ffa42f2c7c2526c09a24f9dd291662a9cd1e407950364488a33e5adee1f386e69" },
                { "ru", "44afa33a4a023b9a676cc0f798019273cbaec137732b5996fc18036107733b9d49207e18d2877426ac06b42acd9da759c2bc1cf2846d96b20206ac4b9e58a07e" },
                { "sk", "b4a81c9d6f7b1574af92931df6f428e1421fd128ddb2f07a028fb59c18fc71740d77779f548f93f93b491d8fdc08d11d9325d0edc3c308e4e176bd501e72d7f4" },
                { "sl", "50939cc3ef88f87dfd3c2d0c5849c616ace4c116ba826f5a91d58cff692fd89be6604d579c67b02c280b045df15b938654c324bdc0cbc64e15abfdf758e07c70" },
                { "sq", "cf5ee292e32fc674498aed84fe258d1c2300c7fce70f2aca85dd012c1fa922db80ceeefae41817c11ea735f6ed459fc0f644e109b247d447b6ce0f8125ec2879" },
                { "sr", "0fbfd82210472354b9a4d56f8edfadac4383015b9a907f95f5b7a5e636dfa8548056192705d172c3a772be973b3819134c862228f873b5f18f721858098a5cf6" },
                { "sv-SE", "8131a53d62d2f5fe5d7a970b20534c8d501cf3d8cc736a230723da93aec898b1155f6c50a49f4e29b80ccb4e32f4cd399f8c1b4d30fe861925bbfbbbc1ec1448" },
                { "th", "01099ef525491076c917049fa1d479f590f22b580591bb4cb14049d02726389a0742d294cc55936b1b79580bba14a0d210f8fdf260d41a7dc51699b47ba6a9e3" },
                { "tr", "9675ab445d4d81f9979be37f51d30bc2e6d07f96c899722e0607e01f82b7444884164c3087362454f8e8c5ab8ad80ce03f8153c432e06f6bfd45900b848266ef" },
                { "uk", "271ddb4a30ff9766abb6533fff68a27f51cbe9e6b29acccee506def96d9aa552bc6fe0c2a119913495c412a2bbae7b688f8e9d44685098885e0847f68d8ad6e9" },
                { "uz", "471af77111fa2339d7868f2a3c85044e63726147696e718e52e8820c7d8798e2bba0a15c14c0b471fde2e83432b28ccdd8560e778b353e0518ebe8daecf051b8" },
                { "vi", "29ed794d8917382b8a66c1a2a6b41b43f20ff4083df203312ae4fda706f6ece77aa4104463a81b41790af9f8a9e3ad5d496e077cf4a4528dc96f77847da5c8ea" },
                { "zh-CN", "d92d0ba086e5790b5e3b287e4de1ea45c25cacabc7c010692b46d0369ace005d0060d0d3bd0c2b1c1639b69bcd0837af2149a2253e9dc44416515d3c45a43607" },
                { "zh-TW", "b347761d9719e4a7619f8e50ba08ab6252c4e8a9cfd5ed89800dd2272e3574fd14856f7f931db9ba07df0ddae92418d73b6522053ed9c28c681e1de762f4a71e" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.0.3/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "d4316eb137798122ff423bbacd372f3d61a6f5b48254a78d3b9ec5ec3ea8b1025f881aa98e14a1bc20815581a8caf613ecfc063fa4f00877895434e9665ae1bd" },
                { "ar", "365e24b03e787da574d7edb9c9d2a832ced104022bf3532d3449ce0ac38af929f172b2db9a3bac117d733894480abfb608e7059cfb654a43cf6aa1235d27883b" },
                { "ast", "8b84f04d234ccddec7e2740a44d6ea0c6af027fc41a5c4312251cd16cec8eccb99b0b0d0fe7891a1c8a9ed8fb5587fb11dd673e29f25253c5b6c06a2823eefc5" },
                { "be", "591c6e540c5f21fc1e9fe110f8cdc84ed3f06bf465112ba11b0d961db8c52cc3a900404726bec835ab8d80c49567b3f2ec933724d2d43a254ed2f59bba64ed0b" },
                { "bg", "f6fbd92d8b64cdc0624e1aa1c65bc4441dcb97a0464f5f1a21e404eddabc56ba3ef846eaf0f70947be6a48bfa72bc3b58e5e01c23ccef6146a59616f21b6d3ca" },
                { "br", "4b0ca897e599864e1986e9a2fce1d0dcb366dc3226caf28163c965e722ea4c550aa54bd49750f70cab6296dd63ac78ef82189596494dd474af46a9b50080bdad" },
                { "ca", "87c5864447fdb82d54c63109a4757ef84f1ef583126388eaf63485a7eb2e18743cc113b9d3600e5c0b3683c4a924d5800786095fa9e1689f5e6b6f54f4a6e672" },
                { "cak", "b6737e38b05ed34b178d62376b579f7bcbb6835bee634fe464edf8e336f8ef454ef3fef5636458f50915745641d5aa3cd9204d5980e2a56af3880952070fe7a4" },
                { "cs", "ef678efb25601f74cb7c7df63696b9594c346d267e127a57e90ae56a28eddf036e823e564708c897c62b21ed886122e272b1f993f9d7e68d028b3fc1a402f474" },
                { "cy", "9a89609cbf88804bdff46e31e9a2f9d63718ce56da2f407b3fc5a398792a20b8f7fbd0c46dae6abf16d7fa0df061a21d9676153489fc61e7995c57724e92dd2c" },
                { "da", "731ae0c4adc6e6c4c03e9a5dfc0ce4d2049ca38190044d14b41f7cd09b7098f2a276d9a9b2930bfed9662e7dd8f19d7f60d9eedaf5bf2ad0b5fe5ada96cd717d" },
                { "de", "e1feeac4b57093c157c63b89157917c2fd2376ae4f4630789886eee800f072fef81155f3b25eb60d5d4a78afb52f376735860b6afcbae6fc3a975fb37ca3fcef" },
                { "dsb", "d2d2625a7310fc6a7f3d01fe5dff81e18ab02d1d8487d4d795f536c0d5c58687f1e539f8c65996a8e3c7ab170536d5c3a53e8ef78cd29d8ff6d5508b09cc3110" },
                { "el", "a9c448ddbd6aaad9dc2e31599629805fb7d24d2070038b12888975e4bbd15b61275011160daba8adcecc0bc62ba374e0041ccffa9926c97658d8c3ce6a6b4c61" },
                { "en-CA", "569143f2a7ffaf35c207d6f6abc2b68e95d4325135f960b5f66b552bd1091be5232d22a483971e14733755a055d3cbaf7cb5017df818852789ebf70e5a561b01" },
                { "en-GB", "7ce4a66eeba4897adf45da5b575729826bdb417966a1ea7ec4c86b3b1bb8e828e8fe80d0ed789a56e86753e6e1227be227c74a11b955b57c2aca3615686c513d" },
                { "en-US", "f9329968fbd5603b903a9f7ebc5357e1e3e94b7c11bd5f323725cc74eb306aeb126914bfe2c57e7cdf851d357164546c0727f625dc90a86c10435ebfd309791e" },
                { "es-AR", "03882c407773f40c88d05b6d25da05d1abf533eab62529ec1c66d7bf4db1acdda8931ae2408cf89de52be8c84256bc9e597e086020ed4de7ee554e537197152f" },
                { "es-ES", "8f50b6095cdc49f1f55f8603c4dcab00b620479ff38bdb7f5cd6caaeb0338659f1870ab0f46d30edd27296a841ef06909d2c1b5dcc94786894ebc4c0972827ff" },
                { "et", "ecd0dcad4fceb8484aa64be87eb30c3b5de06b679e1f561103d72ef29ade4ff14d991c4f4049c015c5b6ad9da182a3e200a973720e403f1a43fef06678112ea4" },
                { "eu", "ae3b49353590c57165575b451a5a571f628c6b2adc667fca8973cd1215e2d3dd5430be0f02e7adf7ecdb97acefc815cdd251ed6d3a92a8ced68f3e0118c2d636" },
                { "fi", "21765112764bd901eee98a68f1577141b10769eb76c73f2afe5412fb5816339f187ddd9bedb660529764181035ecac582ceea6bddb989a919079f22a6705c4f4" },
                { "fr", "c904b31d99c1240518840d2a06122daa537f6be9c81dd9092477094b78140ad63dc796595625aa65ba6f14c33b049e8bf0b9c294c15b3424c20052eb185f3f9c" },
                { "fy-NL", "ba3878daca079358b7e4a8610900b8faff5a14701ef1f4980fb6e1ec67db56a00b63b3b54fa1105e30d2a1a2968e1f2131349ac71f065d3f04201c1182e6cd67" },
                { "ga-IE", "7ae01a7483c8361d878a5de1a7a0db89119ac15370506f56251933d162c69c1311a56c26d13e8a7f2ea949afe932e353affe46ba631ddf2a96505cce9d90df33" },
                { "gd", "597bbec339937038400395ef2b9184ac7216426ccd665a0d6f6d83ddb84bdbc7b53416af352c7b58ade17c1299ae9b52c9c76c24c366dc982a184170f6749d41" },
                { "gl", "0c15f41a456aa93a8b6ac3df7e5a0ed05e50da3075dc0fb74da6c1ca14408c2a20034273ad3361448703a579a0797082e8ccd8809cc5b6469aef5d6b270c8d55" },
                { "he", "13c91bc4e496e6f93c31b4fa5177e0dd38b641ea6c700cef2d74c4d16e1f7635b0db491a73f4a24474916a879fe918143c340a328a0fb82c503f2c4ae0c55f0c" },
                { "hr", "0dbaa57adf1b2c129d57d4e56cc66717d6f483afd22a407656a2744d89076ab55efd06183ec945e55c12035d3017692dba58c8735be84b98acde1acbf4b69614" },
                { "hsb", "add53f574cc3d2ed90b1d9a49ff3818afea757e28c227c812fd69462c146a2e95ee266354597a7adb26be66c1277843c5367fdd8d9281a95bfe9e2dd732a11b0" },
                { "hu", "729156c18309d62e064f1d10d14e5a7812f4df181694674e448e31dbf356e641b85884c754ed5957b3c625752d0c621a40d357e49a1095170e5819688af6276a" },
                { "hy-AM", "43ea8a66a1a00e7c7c75b872d0329466b74122a705ac0125aa341a89cca77c3c1ed454324b989bd779b498025c7a798b30a34a7080206d6c882984f250edfac5" },
                { "id", "310fc77a719db8b447e758989d646839b8c87c47c1824eb25210c3ba32007c4667641f4ff90c91624853db11cc62204859c735c846224883d0fe4a92e3418fbd" },
                { "is", "79e15200d5fa44f295389a9c5726d07868e3622e9407c21e3382ccc189333f99e65dcf814768f22a6a023d5280df0458b2d3bbfa5fb5d1b4ec99c9401182468a" },
                { "it", "26dd169f12f55fc04cabb5c79bd0d223b4c5fd48a1b6c9b683c20db5fa4fa1b78e52fce58b7cae5d61f367a4a9e8ebe8057d0dced32d2ac9d44ad8a13e8ad842" },
                { "ja", "d8fdc736d2d26d5e29a314e8ea46121f28575f5a6bec9325510e02c289da6dafd2eafce107d714f5f6336247cf4a1b0ae70782cdcaccd566366c021a97fef352" },
                { "ka", "bd39474052c71e21c97bb3ba1afa35d1ba23aebf0ee0b60158b5df224ce95756c5117a081441ce215679dae59983cab4913f9b49f785c1cc262dce8acee0195c" },
                { "kab", "5cb3a9cf6cd3f4e26d76459a229db35565073eba46ac66d90119786030feb016a0b8250b8cf956309459f159aa631d952e34ec08e8790ccbb7ef62375f4143db" },
                { "kk", "c246d64c77529b350013dbe0a652b07317497b0a07a033683ab64429b018fc12fd60bd63b612a0c95b2a8c5f544980274b6cc555163f4a9c9617fae5944a69c3" },
                { "ko", "6b73b1b477a6e273b276b59536f618a89865d6a1ec4a28cfd5f9137539e086326f7dcfae02b86dbc0009e1624a97e25bc85bf94647566dd3ce5f6370c0dadd21" },
                { "lt", "ce74f25b3dd7e3043b2631cf3e98a06be8ff96b4c7352362c365e52cc7cce6c6189d0b17f7bb015616dc404f728eeaeda86d23a4987ce06796c50ba0e16addda" },
                { "lv", "c2c7525716d43e49a814185a34117d9c5d2bb4ecbd85a6d41267183f60d9c50326deda85768b21868071f4325300148bee00cb35c8bc9907e82294ff671f0026" },
                { "ms", "d7a8ee59e0d67049d0f92e4713ff10e4da7a3b2988c36a146f861f340dd04fa5c8db2dee3032b90b071c29ace7b8e299714726438ddfc17455e54cce70b9c1f3" },
                { "nb-NO", "19b287f2d55d7a4acc65dd48296641811113659975e03bbf049f6b788f4f28b77f2304287dabf8517c11edeeb859eb24b011f505d637f33f75edd16cd3047d4d" },
                { "nl", "c1496eadff0bab79dbc74a702d7d6ad82b3e95fd253356d2bc8619bc35a064649b8d09ff143c1122c67d0d3b8126a36926c71aeea44204c7e3ad833e67807716" },
                { "nn-NO", "8250a697d84ce1cdb2a1b8848e99c2a81184256c3cfde04073ba996b37cbf242b9e6101676f198ad6c62a40843f84f018200f907d0e14c326aeb29f416b3ecf1" },
                { "pa-IN", "28fd08fc433bf127b5b3f2275458ba12bc28850b0b138dfef31268a4b28997e87ed4568c31c548a1398fc46df1b24643d142e7c531777afcb9831035f73f989b" },
                { "pl", "ee4372341b003733b56a9a4c00e08768eaef67cfba687b40d30711b4bfeb8b7ce4417a51d6bf0708fa1b363f020c979d407b85a6e46cf4b30e9f10f920ba46fd" },
                { "pt-BR", "eb325851136e2db1d68382c3e8dbc704a23a257f22d1a85d551ba75c7828d2ed9c9d4b59807c3bf8fad805b6430eb8f955b09bb6d9bb18aeeafeace32d70517b" },
                { "pt-PT", "c6ede336f11f9d23a3b4887f588c18545079fde54c618d48038e31f732c52972dc7996a0b9f40cd339f56bffcef8aa835b04cf978db071f42ee7b2f5241c43b9" },
                { "rm", "eea3ce3d83c020d21f84ad56eced737baf7890e11cc11696f3716a30f897eb7e5bf479362d73ada00a23d1c77a2196160d5a5a0e047624af806428f002c8ba2f" },
                { "ro", "1d634eeb5bc9e700c4996098923758d64512ca8b8c598c07783f78ec19ab28a7ee58ce0c1050f7ec2a815e570b4361285c543555462a43a99ffd4da058d29fe8" },
                { "ru", "911591b0a13ee581bdadb8a2fb129391148fb17326ac169aba5575133d7bda643886d9526695b2a89340a03ca726d3547960d12dba1f0d5df00b74d87f1bacf2" },
                { "sk", "970db58dec1264a74e866ea6c5b9338f0c5aa906fafed3a973638b8aed23322e4268caf483d9fdefe547a641736a55d3033ac48a7bc10008f8926ec5120a6673" },
                { "sl", "906c781ac3974a7115096ab3fff2253dffdd9322c5c9d83550f10f956c4aba520b69c3b2209eea7d7db9d4a36a73286a8932470104a25928d4049df6547ca071" },
                { "sq", "062fc28ebf1bee60fd18306a72a6ea001ab71241b98e5066af10c2a822e84484a47da5bef3cc02f18b421394161ded040ba16ffca7378687e47c664d9318e947" },
                { "sr", "ffc7d882f987ff102f02f5a69bc10985b319564186e89d70f18a35919e572a68de0b2e5f64bebd02f9f5cf92e26aa0609ad41c9b299adb85bd4c88cedf6cf121" },
                { "sv-SE", "9de596b8ddb9a67b5eae43c60b9a7033085d5d9cdd4d6d07293e3255e0c1c7734f7377e75f3e5e3c1bef8064f8d30339d65129f64d88883594b79908076e32b9" },
                { "th", "33b80795faf527000314bcedb912003d5387ce8a9e5b73b2d6fb8c9aecc1c9d5b49d810219b835f69ab12904609da3917eb06ecb1e777c97c64d74e09dc418df" },
                { "tr", "f050d9dcb2028d57cbad3d0d596177ed02bfaab6cb5c6f8c22a175a529348f4ce5cdf8fc62440dbf21d059b9614f1dbdccbf73d841c8f3d1ca6830b0490d25d2" },
                { "uk", "7ddbc8da63a684cffa300a3c4f79854b06e1102d3517fb57627955c3beea905787c8db3092a777b6a2a60cd24501cc2125a672a6f006566a4c847f770088d195" },
                { "uz", "296b7d449a7b5cd7197c6a516920e5f40253a877b26cef2f0a3f4943b8b1f638cd77a40d7d5118eae65a19168ccf60c0c7df88e6879f0e91b9fad7e125e7a822" },
                { "vi", "3a743d28d13da42f5607d2db646c5ffc72c38a16bb54e7bee9c626a17cded8fd383a8d50f028744881c1f68100f5cd886276bce6a1adc1405765bca89a0e255b" },
                { "zh-CN", "5579dbb956e503294a14e5780a32b7d8cc8979dc80f837e35993363a797f76cd0c92346b222fea07b4948a98c4b472d9974b20e77e5edbdf569bb8a7ebffbe71" },
                { "zh-TW", "e2b8592f773d16d314a5382c7a4e314dc367ac32feb12ef8fab301ee37adc5105d789070fc7eb57d1578d7811b7cb2882a6bc894c896a6a857d0d8476298c577" }
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
            const string version = "91.0.3";
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
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
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
