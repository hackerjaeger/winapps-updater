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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.11.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "bdae89c9bd613ba9944bddf678e000896be61151d8c8ba3392be3777d05b3fe75136c469263082da461d941b3f66b3a4e2cf9a7556d815ba0a64d7d655d4fe72" },
                { "ar", "d839d4727781894b22ee48a493e4b64b31cd092b726514f8c280859fc22c63a38c21263eb0325d00578764ec8ad90f92510047614d96d91b838791d01de06dc4" },
                { "ast", "28ed8fd5d28433497dee5094dda0c6064a18a527d29b75484df3698629619ab75434c8d2ff9d2e793938251504722e36949c7f70d5ed1da372568ff48639e5f8" },
                { "be", "f81a8f44967acc09155fd5a013730043b114ea076f768ffb4c1d51049e64b93e8d9a1d06ac22c04ae483a594ba1eb3e013c939439e5d4ab3f3455f0b34dd8199" },
                { "bg", "cb3f195f7e3ae808a6699b52e7f1e3d5b10050f6c050805473d3595332e9be1afe90960a929d26da5f981922f7dfb3623ddc2e58943ba752fc85d0a9fe2f659d" },
                { "br", "5e56226e4cb9e9ef2a107849e6ebddd10324562eaa63b9450d2f7a6e6b431223e4c23b869b18e8a3627bcb5cf07e2952c481dc7f477c59dff68edf0e0726e79f" },
                { "ca", "a1f75a2a2cd9708d76a0c1470df4ca6cc19dbd2fb6c8f6cd078bf89ed11c5589c69d01499003472080fc6209211f2dd088d2e25755183c22d41da34dffb3f866" },
                { "cak", "bb46d7682827c6cf4647d0ef193914e735f942eddb3d9e0c3a7995b578ef2e431c6c3afa36cf2f19ac083f4675c3abb6ba16d0aac1bdbddcc65a82bbeb08fb2c" },
                { "cs", "54ab523a1c672945401e590f8a76f95d044bc21dda5f56da53b2ecb0fa5dd427e09c9b363611b0c3f1d4b4dc53473e914c7f1ef86bad5d015d2fe9d0ab2824bb" },
                { "cy", "3bde1898702a14b22148f60ab0a2996dc2dc6063e7c1169773ee666b9e052c303b7912a5da1138a3d692c04f5bea920d6c7edbb40cd393dba0ba8bcec15ab613" },
                { "da", "d1a8e8086cb1b038a8b934edef6e50b6cea59f5fb9f2723d2060c9f7d8549f8c948a4f5d20b33dd4d4e25ab93565c2c19d818d463479236d4dfb56d881a935ba" },
                { "de", "b28b9b6fbb45dc44e4777af7a84ff6cb5f791ffbb59d1c0b30ea96d2dad0f0f18f97ff5436a44f6b9527e6344fd75c6dcda22ebf956aa7fe1664b75225d24f7c" },
                { "dsb", "47ef8a7b082f5e7ad09bf2768a63a51f4ee0ed2a2b9b2ae1cacd0ee21b29ef118e3474c066f4413a19d9c2e0fbf9c464902ed9a36ac511a7b9f4fd1f174cf764" },
                { "el", "36fa3a3f021ec91a8dbafc0d9c73d43d5118974f1be7fd6dffaf2ebbcb4ba37c1e6b61dee3fda995a5dcd2657b6ee8052f8546279afa23d24d1c0f09517815f9" },
                { "en-CA", "02154e189fa44cb3c6d4c604fb2360c71cff4b559c5d041a988325f188921486a844764943129a65f6d5e56bebfe876ba9930924812a3007fede5346e998e2db" },
                { "en-GB", "21f54a5518bf196df95e1d4cb0be9c0c38f4afbe3cd3eff68cedca8065e54249ae31727ccaf158b84ed21cdf6ca7fb635c40377c4b448c4da251aea5c66a12ef" },
                { "en-US", "1c8f68f1ad859852e65c892ead6079dbc12aac0c7922f6a996d311c8e456fef0556476cf4485badd3a40aeaa2da62b68dcf95b713d65387a25d270751d9b0798" },
                { "es-AR", "ce0696d6543919c8cfff748066c6ac39e697deccf18ed52f8cc3887bbf03e089e23f68595b646da67b8366b25f6cd69bffbc90417b7843153136a85fe9a1c3c2" },
                { "es-ES", "bf931049b87ff0d0de7b10a2bd6c97007860afe240ccee98b8f821740a1daf9bd926c6f795b742fca9126b2bffe4dfca3cc6b01a4bc8a6b158f11bf22d847596" },
                { "es-MX", "e433e3ffc0edd8c6ae48ef30f56d2a77ed031450be0da9bca8201f400e0d2aee3299e3a2d3c006a9c782793e5f7a509665c133e2d7537101a3051685a3cc438a" },
                { "et", "ed3f072debaeb29dc2c3ea2d85582f2e29a0eaf301e02cbd46729a908f89bddf4a42111eb610692bc4fecdb42fb63067d0d67007e46fd9edd4fa0eb8877507aa" },
                { "eu", "7c673e18a5ae36e0a564a2a7b911461e72601d069cc878470390a0a60e4b8947954001d9a3b0c9e1c200e976df3d70c5544406d1024e98fc9cbdb5aafb22c6b1" },
                { "fi", "655a2f5e90fe1607e8dd4ff80f8d3720849b6bbbe12042ffb96f778e1dcfe35c6bb25e6fba27bab4e7696aa4569075dedf157449781301e4e165c14cd223f9c0" },
                { "fr", "a630507d8124cbd44f777b37f5d53f78eba9dfe5bd05c47bd592438c05dd61c358cb1a60a9b09ea99b04923c996bb5c9fbdbe14d22785e1647c44c81423f7cb7" },
                { "fy-NL", "23e4793abb4a5472b3fd5c973e4c74e1259bbdde7a74b6cfbc77e974aa3024e08ff4e2ce2636ed8986cefc955f9fd387496d53437f9c1d2e39d66eb7c31ba7de" },
                { "ga-IE", "0c5d0dae593cc5a24eec95d4ff085e6f229462810222009e79f11d4bc701a89f0d140d240b9a9ea909ebbb2b6400b23ae84e75888a4a099a9cf6c43916f05793" },
                { "gd", "8d2f0e3f156b32fd31b90351300c164fb3d42e148c676830dc9c344532559a74d4713edac769031ab49598e15cc0a2d9717643d7908e56e619bea9197d365a0b" },
                { "gl", "ad7e30b760d93b5d5fb18ecdd08a4cf6c1a02ce69f2883e2c47d35a03919b17b8289182ea3f537f76596390b1a74404afff75cf9155ed80d9def7f3de7f48187" },
                { "he", "b20ff476a5871d5a81b3a0bb5854689914dfe6b53d7a2dbb583838da1622bb04e874103f59e9f15bc72360a006ea8326fdef4789472098a39d9fb1b0c074ced7" },
                { "hr", "74826083d7fac7a15a395b669cb97e66a6c9d2be549c735b43b398a1a2236129c7b7ae22e47f7b155f8a5276e1d31f4e6d2ab30c7104e3421aa13abdd83a52c4" },
                { "hsb", "2067028e5129215ec839702052a29ec28221c634e9d054e907bf5771f99c8febc118f303cbba758d91914f5d1776eda5a2733e78899c724614199389c8bb0c7b" },
                { "hu", "76de4149d761b6fa8f8b28479ad8300fa063e9edb9637de0e74263808b772b513d76d66ea161d91a94c9644305b21a074e6c991bdc0fc262f04e85978d17c85f" },
                { "hy-AM", "3f5cc6b6d4afb04dd52f046516cf3c45fbbcf536a0bd309189b1cef0778548ab4f2cac9fe6838ed563a992ba00d0ff5e981960dc5840c6fdfe87bc4332e90117" },
                { "id", "5cc606cae72e5236e6460bf48b0635aae742779a2860915b66466e6c8fd824d0105d6673d2df47080f07699d87c2e86c3459b40aaabd235d3bcb25d2cc978111" },
                { "is", "99cf8b31ad6eb50050e26ed8352f17b8c9f0f3092b5bd3bedddbda779c45013ad4f376c5cda418b26d4c4335c46af30acf8bda8c6f1eb3148a4e1fd5427893de" },
                { "it", "b3f7734120e4a288155d1a8904694a959062b1a40f7d8cff5579d9b4010cd3fee06ee465f2b5c26bf950e4f6177a86b498192d874417882f6192c05a0430c1f6" },
                { "ja", "3290d2e92a1c50eb45366875b1e9616673df813ba2928e65536c7f3ce9705a227dce5407d2c35ec8bf747e53c4f20463e4bfda0f2313acb8eeb66db00ca5acc9" },
                { "ka", "946bde92702a3710170af8b3856c768b5d7eb3a7dbf11c2e6ee61f479df574ab3be2d1aeba0c338faf95080240e9b644503f418e0997f000085840e4fa8dccc6" },
                { "kab", "c4e05f330d7338ba4e3764d66cc7c1d945b0d532427bac50a77ed66f66be0b88ee7dddb8bb8abab4892606fb54477e1c38ced6e0c2aada5b8ad52d6afdcf30bf" },
                { "kk", "45df73b800349b3bc157251423b5aa828eaa5903984243cafc96404e78805d1ec985b72194edc8b19e554945be3e71bb972e9a3e52c83a679d6a7aff335e8c04" },
                { "ko", "ff23b1794922724ad3f67639394db3333c10c7607b1e509647c3e7a66f87047ab6ca2b701332cd475b2be4515e3a23ae96e6b0d396cd368d392bb80bb257ebcd" },
                { "lt", "5870d17a26bdb9f1e7b1735ddb88236a460eca0cfe926c6ef4a5909452b3e2fb0d2c7b748c59662ec21cf0e46f6a1423af011cd5bd90aa1882b0a557ad87d769" },
                { "lv", "e1f329a21b55f7d4388ca562df67c0fddb488b70ce5d5ef666315624ba3a34536f02b779b2d8ee560d7e82238860d5dbbc9f84064d9d7994f45be1b991fb04bc" },
                { "ms", "698e100556ba5a6f3e30e7b4a9ca8a8fb6a20a325d4b19c7eb26d5bdec7af0b0ef872332cfaa8aede42d7c215f478b576d0196c40fe4552f3795028c58ce1a92" },
                { "nb-NO", "eb0db89e7bb4ce86b1f1cc6e52a4a8c95fff05a4da282e32bac1f1f30f5c5cbfd486d9c185d4174defca4654cd08eeb6c10f16fa16067eefb9037004435a7165" },
                { "nl", "5a7547f4e2eac1972bffc9bd00a36aeca390fd4d4620cff14028d6a0db0bba113d12c3af1d4dd51f649fc6c35ac6a107754f31ca7733e19603bc49981b084abb" },
                { "nn-NO", "a6e751d9dfbca427ac104b969af723b841184cb9e389961acdcd37dd6e8b834a8222e056cb56963a0de30fd4e454a52632ae8d1ef93d0f97dd850c39a99ee7b7" },
                { "pa-IN", "88da2b323d9847aef4479be5b17a911f6b58af5863e0662aafe0397f74a9238dea78e25769cd7683b5277ae1980e4d7b6c34b4ce0f7ec6f2429b13a5e7107f72" },
                { "pl", "207effd5b4eebdb6279237e19bd11cd39baaace9fb59af73ea986ccceffaa94b71d57d9fdb86a2c8103713e39deddea493b11c57f53016c64cb925b5a3318552" },
                { "pt-BR", "3f1a4eb74cfca419c15b03732e12dcb0ad40a7d5a82d33b43b2d9b6a7bb2c1b8b4b225187018ef2f7e45229cd0fa3455fdb1847fcc98c2e01ecfbce357124c6b" },
                { "pt-PT", "7375fb0439893f155808a509b4b620921dddddd168977892adf4fb88a3f548f4630c91c9387187443d2ff3b21f4101ab340eda241c791ef84305f571bbbe4d35" },
                { "rm", "5ac5016a5f77c2f87457abdd44a5c01339d5cc492f28964b3a85f45442eac45b066c758491ad794237de02ba21fdf26bb385d7ba9b1202be108035b50cd875cb" },
                { "ro", "7a3dc3fc004746c7636cf463868660bfbbd5810d12a8bf6e121f45fb3da097a9d9b61bec492e135764f8646a80e6e6184c07edabbc357d97b4101836e13c7217" },
                { "ru", "6e2c8f64ad3102468def3a3423eb4ca703533dbcb9c01bb66902fd9ee5b0bacb5bc2cbe87a971b892f8e7051c37e97f8f803aeb3c078e594bcb9293fb20cec44" },
                { "sk", "2fb4178f29aad3046c85a13597f8281b605ca4765ad2039e01d6fe30c60e26b49d98f45821fb25952d4882e34ec28ede7c09a4b040a51c5f7007766c2fde3706" },
                { "sl", "f423bc373f5d3db8fa6a970d451b84b252f3a3fbc46d76c190f751d476021e2cdf3b845001bbc6f8c38c706d04a2f928e60a53592d1bf01026ff17aacccf4536" },
                { "sq", "01e12891537a3d62a785355927f65bb1b306536872e49eed87ff8a0e186e909628c667b951e2d112d0e29f72ede579050b40a5267a321f585cb091d39a4ff19a" },
                { "sr", "f0be534c72303442246a78e858e104c12d3108eb2cd276e7c68d5a5254310f084c690aa40aede63a11d4119b387c93b6b1df8603c608090567394d8bf572fab9" },
                { "sv-SE", "c55a7bf9a8181f02ab332d175a7bd1e9b2681314bd74f80d656cecbb87a0b783f2bc882e4ddd723b403bfce9615f82e35994b470c728d569ce60ce26c563ccf0" },
                { "th", "54157589369e06586746abe76848ba9546bde40dbe87406daf5c8351bccd1d372f6e6eceb5df61f30328eb5e7c6b402eb5d592cd08323e93cc6b43ad5821e1c6" },
                { "tr", "6664831ed70db003906a6f27b91508389b187287b3b43ec108df50319591cd6b82d4f4dc100b92365a6f7f32c696cffbdda0942377ca9ab8e22bca7fede7c407" },
                { "uk", "ff51a12ca97560ae35b04a6446420b8630ca23b018e299c6379cde7e182b521a0ef537f29da18f4a3b81f269223babe8ff42a04a96b6ecba10d9bee6a7c80aad" },
                { "uz", "e8300277c625658e217876dad675401cd9555725be7323dc9bd191b7548d38d664752b97daa4abfed242ac7096386571d7540bab16a273113e43be9bd1c2d065" },
                { "vi", "44e49a0d4d55824a36c30e6abeada3368ee34117b61c485e30abb61cf3ea1f3a698b33c3019dffee9b6efe108135b278d26142b6ec3cea56f0ae307afc750104" },
                { "zh-CN", "0275a925fd91a8decdc88ba55ff7f3bd06756be7c4303d7ac70fa6b8efa6c6bc6f53a179dcfc94cf66df4d328d94dc7dd2438775cf2d45dbcbdbdd1d2fdcba37" },
                { "zh-TW", "d6c968fec28a99e42055d8381564b71faee9f004849ada7f66dd6662f2d5dd582edbf53dbbfb888e50daae0f7854ad87b00bd808bcb5547b401ee506baf90e60" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.11.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "b5be780c1c42b5c78fd5fca9711c74a6adaa9ea50998c505f648f9c7e2a75811d0c1399c7bbc7903d63c489230b4f81ad43ddf7d78c48e828c06c5468a767384" },
                { "ar", "b6a2792072a175da3424216763a470b5e6666f8df1ca1ead5abcdf4b363531730be93f55c77a6a0897a8ea244300cea1e2f5f3cada01014d1b7b18a1d6db32a1" },
                { "ast", "83fed49c34d155d3bf37bfb0ab1953096a0a8dfa81cfc5dc83d5a72d60338a57d895ab5058245628112c6ba976276d43ebd9357fc75158fe1e64b75ce6fdf6af" },
                { "be", "5be2e51fcba7b5c6dc15b136c7c165b0aaee734ae7664bb032b8b88f65f7792ec1d4230757b287914d72b5d7fe813ba3f9337c23c59806c0f22c38a2a98d8f3b" },
                { "bg", "f44c40e7ce91ff2792b5122ebe692006a615b06c750b542c7278b12afb49570b49161025c8f5672cba66285514c2bacf6660b5e6eb5aadfd1aa128120aeaa469" },
                { "br", "47e199386459cf4eae82ed35943eb4c85b56cea2d2ff4937a33e3d6f3ba490964948b9127762c13d7bf3fbb0e3519b079c78bac5f94cb2752a07a37e1d6fb9cc" },
                { "ca", "2d325308d683c47f5ab65398649a4fbb74af116643c804f4e7834cd9b9a6c2997b34991b2595a40ceb7b10ed070e962af01c04caa24e3e4b6322724c51178a6a" },
                { "cak", "a02641cf92dd2ffbd427376e8d37cb8b44097a35e517f9f78f8e92489aac514da66fae8a32e22c35fd84807d8b707b9add852bbf6c6393ad13b90853c98cdd3e" },
                { "cs", "79923e1aa10f32134c86f487ca66dd7c85473d3f09ee4caae623e0535a4d2af5ff43969e307e76850b5b081b38d358f2ffac236e6ba1a8e32801253672210ef6" },
                { "cy", "1128c5d26b47421869be872b7d7c88b9e26716d83127eb7d5143494a769b3e722aab555d2e027925c8c6beb54104bbc27e19f8aab507c71c5f204f04c6d77992" },
                { "da", "825d955f08574545aef83f84f2c830a6f45ac8374b36ba702372bd0bc07de39e3fbe59e52b1efd2611bfc9c6102e90b5eff57e8c40fbf31d63836db14133a221" },
                { "de", "b1e8d0f6bcfffdc98241ee82fa3ecdccbc4a737e8720a341528a3a8d33b6f89c16a62f99d8db064596c149a534e8ea4ba84b754babc05a3bc250426e03292032" },
                { "dsb", "d023e94618d7f14491dcde54b7622aa8ccee5fab32a09d8b799b04948a3c0ba735520540333ef584cd8f6e8373c431d7c27278f2f05486b02c3f757f74214cfa" },
                { "el", "ff6fe4a0a9569f0160a7c2ae141456545283a982bff2ad38aa51017b624829c64f8678b7caaeb40ce74240e386a11b8ec3ad0faac8aac8f886f6d19a69ff9823" },
                { "en-CA", "2aec2c2a071fad84d791b0b3c5e9b0437bb21e1c72641d761fb538f9750344575ba0b8729386b736843b2cb2dac61422028f696c2de55d728076dd43bc49ce53" },
                { "en-GB", "24e0caa014fda1f41d7ab5f1966f8f279fe297602d04b0c6b7af9fc12199c94df061986ad0e36183280cfdf4b4cf50318b983513997720f5e3b251aca8ab2ea6" },
                { "en-US", "8c5d7786e4cee3e9d5a9ee3ea13e1708f52d0f67673c749f5f738f668aac4ddd52b3d22bfb92fe78213c7ec35290ae584f5fcc789d783d3c4e6f38688f0ee455" },
                { "es-AR", "8be57e59d9aaa2f2fa85b016c87024c8fcc560aa02645d99d74c36a841c96d67efdd02f5f5aea64a0ab98787153e135ead76fdc73b9d03dc99c43d26909bd484" },
                { "es-ES", "4e862c12a1d34d7f1ba0b8bda2dad135f29483597697517448345ad5edc6fbb769948ba713005972891ebcc31ae3f4e3e92aa19e26fa4e6c1a6c6c023c93832f" },
                { "es-MX", "00c7741f6ddde55f8a7099bbb9b96e37c23bccc60b4fbd6a517b986c2ad389791cd7430ed227f5c33a4d4ec6bdeb064f8be9bd21e74ff89a942dbd7f6e584223" },
                { "et", "e28968a6938187c1966d4e539179e48456d5640653d46237a453fd49e0dc8775b7b1b361ac0d7718bafd093dd1a722faf7a8b7256a03dbf41a27ea29ca7df635" },
                { "eu", "296950c0e122caf9d7a025b24141680c1a6e965213c6c0ce63574672f2017536291f484dcc0253e0596c52317c4791d8d6ffe44ecb8b935e518b0556b45ca832" },
                { "fi", "3546fdae2f4360ad19b10cf6e78748a11c4e086f5a015ec96db38df04c8122e99d9b2398808a8eb2e3ec58e63068a2d57e0392a46e89252e497d08c37b8671ae" },
                { "fr", "7004bef6ef13fb4cb9a1c97fa1339bb2531100e4e9ea1e12d427c58af0598b1be2f4eeba328d66c89901f028615b0baa70ccddffa4ae3dca1807ffd6f671383c" },
                { "fy-NL", "bc2519b1001ec8cc200fd318f22d65803a1be8af13fec739b7e8e6aef51b5a9f8de7aa172f8ebae16b677d2ad1dd912047f884067e07936b095975ea121fdb3a" },
                { "ga-IE", "a1fb46e9994cb36590ff6358ee101664ff5cd5eba5eca60991d6f407b8dd2757c3d7af384cbbb3d062dfe1689328b174fb7a70b6d549417c25c664bcdd6b737d" },
                { "gd", "4918f660ffa098781215da429f69f0d692f5344e23360a8e8268a7ef96ff7e8fa171bb5df0bbb8489a89515826d1a6eb64f33ba49640a23a3e169ebfecb62423" },
                { "gl", "360eb36165a1612567f703ba993a39cd616c28aac09c7c15e2a822cd5bc4764fb963f7d902e6885b637e3c69fdab9f2aa8bbd5e834c1e0c43f13f5deabb3cdad" },
                { "he", "6d8c24483bf6e7fab633c5b2e87364465efd309a28bad52021f5d202e26f0d78f6f317cccd979e3411c2b13174f1fd52048dcd0e4e4ac73109e9b79061e0333d" },
                { "hr", "b654344dd4e0b1b5d3d7f64a941792ceacbca2d937af1e464701ac74108ae93fb5379b2bf5a191e2d55a49e81e862f13d6c046948c8a0f918bd3385e7b66edf0" },
                { "hsb", "a0ec9822f8a21a74f032f7083cc82e4d173f7bfee6fee29ff351c995038f83f6cc53f14cfc577a9cd6f1c8ce2a8568fb7fc52f6c6877c0e83bcfcddf989eac01" },
                { "hu", "b68e243a89af82e4b168dfb4add20f9275b85c683abff247c20f327ae5636d5aae4394aea7ab24ffab689df5197bb3445da4a8ed94d3bae2973dec597debe9d4" },
                { "hy-AM", "44b5eccd6432e1eab544f8d907e821b89296ed987f754134fa896166150579d488afca60e5ccd79b74e1c387e1655d25303b0ae9df576993eb5015a1be4ff235" },
                { "id", "d5c498661b925ff9760f625aa6409b3efd50db6d46b6cfaead6e6b483f560c00e19b7ea1419958b35eaeae77eea8baad6ba117a41b417d54e5e6c769cf09c2b6" },
                { "is", "4504c18d357d42f3b8ac4cf0abd8ad751d11d1a1601eecc5149bbaff88c1ba6e45a4820b7d91a5acd28e8e985e36cdc34e93a0f4f044c73f91205d6feefb9225" },
                { "it", "fbe3ddbf7f99a52fea31fbc9fbdf964dcb4e1cfdeb3459e20d73b7fd0536fcebeb712749e0f90592450668d8d282c8d7c38957a7606b9bfbc007e8b802e24269" },
                { "ja", "c3b42a43ae6c8004934f591609871f0c57d0b1706fb9465b367c96ce0050c2397cd30c852cb441823a116c57d7f55155db12997676a59634042d2324e9d2e5aa" },
                { "ka", "f66218579c94ac9dffec9ab33dab5e49b5a8267ef06a82359e872d2f708d9a6ae929ba17341a18f4634449849465e150ff6f89cf248887c7430f6bb45ccccbf7" },
                { "kab", "5f9c4f2f64cf9a554209d92c24a995ce449b0aff6865cb8027c423224778b4ec5c8080a0c95638c8e17b372de753480658bdfd6d2879145be173cc7b9e033ebe" },
                { "kk", "9c600b301a3c49438c667a8be071486a3298179578cd940df0a96eeaf11426476b430a4967c821ca3e780cdf2420027aaa3392ed319bc200bdc132a5ee54c8db" },
                { "ko", "04917b81bdc5f775d8eaee344b0088375ed355c25bb8d96fb8ba8cdf662c239401d11652a5191e89b10c8ac46ccb159dde56bbb7cb72a106f6fc725809258fd2" },
                { "lt", "faa817af68d8999d95156c8ea0f8536aa9d13619b6d461e10668297571d5eed2a514d9f02e166aa477473900d89c11b22f531c62eb9a593f7aa49f75389a32a7" },
                { "lv", "650df2fff0aa77372a4bc71916cb4f7774797e54b4bc380e66e9bb69b6b321f81a83a63d6857f62cab0123f324ad40ca792b14232cd3df7ce89f14308f3def13" },
                { "ms", "71631e9bd8b8577a3c39f0e3f9c2fa6f8e483dd2dbfe876116408f89271a734e4a642a22b3c59946106cfc4008a5f7a7de7f78bac6c81da766168e9fc0ad9acb" },
                { "nb-NO", "8b6c0cefd222c453ba7a1caf579c932072efcf3fe70ab4530728b3c3f06ee1e7ccd8be8bb428cd12320c94e63ad126ea1243c434024d8f7edc130fcf984e47b5" },
                { "nl", "53ec94331c1f4b7a64d392cc42c86902b62895cf2e7977457783b5c9481f3b9231e045ba1c744eca9d0fe10dfa691b5d1be9c799ffc3b532f38f67f20a0628b3" },
                { "nn-NO", "796396ddbc6784fbdb4161df78601a5860bf7c8d587bb13b42bdd22b9927d6c873d4c08d7f715f2bd7c9fce4dd6cf9e0ba6caaa0c472c837ec474004930365ef" },
                { "pa-IN", "4837a9f3a3caff19548985fd35ed6fddfb3c0de3529973dab286f7d8d8808d904bdd8bc547314f5c1b470845f2bc69d451087a17e2867375d897e8451c8535cd" },
                { "pl", "7620d5ac4236afa1103db7ca9a7f85b631ac7f1f085290defb1a66cfd06819ff3e9eea2a4518ef151cb867aea112d44069cbceeff918ce77e94b4b6082b67c6d" },
                { "pt-BR", "5884aeda16fb4c559156c5b705ea8f7882672da8db2235a8e838da99b05eaae221ad3e0982aad8e1eec770f79b449106199217a1b726ffa57c84f209109e41e1" },
                { "pt-PT", "8389a6dcaa49ac55e1ff9417724de1468b50e5d8da88439dd94922ff62c93da6eeaaa808ec0359a1de34cf7e9af55abe891947289918cfe27427d8c4721b3e4d" },
                { "rm", "87f9ed98acc60233f031a65cb4731689325f50d7d390cd37e1beb6fe5cd4392640803d70d61ddbdb728ed0be16283d51a17369602415bd6e659fd6e1b46c2e07" },
                { "ro", "3ac61b93c7bb95efe09aabc2e816b1b6283c01d79eb1d2098f543c6a2ef97e595982e7ec3311d3e665acea12003fba3bb1e9b889b5ec6893e4b5533aeb4a7f6d" },
                { "ru", "930cf4031738eea4032fc4c477b1b06788e2af03a8c21a04e1fcdd75b68cfbf7358f7db918c5237d03a9e6301b86d186f6a8af813247438509cf81aafc8f2f98" },
                { "sk", "0df7ab9ba3366847e310e2ef3f7599b9f2786ad4a9ed14a2b660e3343f0a5f667e5c4374ccdcaeee15ab2e4fd66add11e37443e8507ed7c43027ce7ca7ea8be1" },
                { "sl", "fc7a6d5c8e9bbfd5eeabe37cace8173c90636fdf7784bae58561d86d506421d6a40f5274ef66018bb23b79ab275717d527413123cde790fb33300be87059c3c8" },
                { "sq", "5add8d78f216fed2f64602856375be8a5321e2b9b1a34056b6edfc332591ab7117d2cde08a8d0e59255ca1baf87fc21b182769f912b5ed9570a7ed8ab42d93b0" },
                { "sr", "2bb427d4f6e58324bd01480cc59b0d70c31b38f87d1c4166d99ce229c6dde2183900d24decb62ef0ab107838c3cccad40e305335c392435847fdc22ab8a100e0" },
                { "sv-SE", "c6986acb332a2443b631c536101465bcf578c84b8f1849900e8e20cba3a5eb395f1f078f8d982fbacf3f953e1a49e226dea0b3bde231f48a12eeb1b905303e86" },
                { "th", "5de0c0f5d144681ac163e9a4f939a186bd4affc3ad61f338df89f07f17b3258f4cdb6ad2b7512bfe93f1cba30261c7ef21303a6769b77ab5c30f7b74b9baef67" },
                { "tr", "184290eebb4ce4291f6e1e952f3e5233fa005c8ff3acab8a8879adda3398dcead0cdde5ffa64a7415bf8895ca807dd97533c4392884af68e6ef2566b3b82a3eb" },
                { "uk", "81896cd5bca56fcf6c6d5f1f0d53ed99ece9c92c9aa659c68827587b95c75e36d48ac1fcb3c51a68bddbc94423d790ddb1f31fbecb75ae54746f833f4ccca1f8" },
                { "uz", "ff598399df4b1f41e9a0566612f3d81ccc894e337643eaee42006f82832437f895d36aa8e0585890bc4934bd6211b096378ac3b3336aea224da4038198d78c87" },
                { "vi", "1290d1e06620ba551e3da975aca961a6b7359dc02fcee2475433ffa32698ef6cb26b50f2c19b7f74103a396651ec9dbf94e36e903317328b7de0e8d66714d4ea" },
                { "zh-CN", "78571a393b8515a70d02f36d570a94c322b33537880cb57a5c23992f353cf284821369390b815fe8c9fda498466b8b4068a805f85a58d4683be4df50df3a7daf" },
                { "zh-TW", "ac98dff538dccacc7b4eb7c4315fd68b4540ae52bf9f76d409c28c55a153bb28580d153fb7967e54787d914398374cd9eab178125d9a760d6b86834bc8d3ced0" }
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
            const string version = "115.11.1";
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
