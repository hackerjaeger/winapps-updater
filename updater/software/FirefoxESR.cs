﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018  Dirk Stolle

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
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.7.3esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "675e5fcedcbda1c6b997b6d868ddf6f4b2e6a465e9fd0e9b91a169430cc15f1aac547627ea144b6eed51ed66a76b4e5f1424eabd8fa81f560d1b0c10ce5437b7");
            result.Add("af", "ae605197870bb2b9688d0c84b5a5ccba8a5dd6a9c75693b6470bc53e9d59bd38204f2efaea00fb513aef14991984303d754b4c42fa931c29da2b1bba2bafc0b1");
            result.Add("an", "e5cfe1c79fba903156423b934170b085592530a21bf33665bfc56b0fead2bbe6e6d9e4a5b9dbd6e0f64cc20aab53d1cb4e7f4f21a0a42758891a40beac4437fa");
            result.Add("ar", "e89f1ae8c2fc8e75a0356f0f0b728d21c2864fa8ef1b6915cb15e922d2c1153f995f8d7ce2280a1f35f970deb3c90a73202d2c3ee1e35ee482f5161e8a36e612");
            result.Add("as", "5605bca45e1a11dd4da0c0268366f0b6b0c6132661b6cfdd491db8fed98f8ff9c863435179e5fcd0b275ce9f6950a931808663e288f86986750eee79518fe7fe");
            result.Add("ast", "0055dad47eb2c4f9f9c508e9261b263d01838cca5c92d26ecdd80cc4e3a2d9be897ad6aa2a323134519117717078d9e0d791397c8ebc755d0cf730f5a24c5bae");
            result.Add("az", "bdc291c1c420ee63ec0a4dd69885fe58a8bed61415d836287e5ab8665dd0a846c1368adbcda6a5d1ed94acf6b401b742224895949cc1587c6c9f66d18a1b35d9");
            result.Add("bg", "afd9575abe637c389f02255825306713a7a6f3bc63c9a03d84d0edcd280e0a3cb26aa94f8659f620dda10b440c74e65e696005c2a91f75c19d2741f8d7d6922b");
            result.Add("bn-BD", "ea6e71304b6377c0b20775fd8a240b455c135494467ac171e2eef5058be073cf6c05addb838af413e1bc29e87fa1b5cdf91f402d8c4ffdba1607b14ab2b17152");
            result.Add("bn-IN", "154edbd6442d040bc5793d00321ef383c8fe23b23e87f28681fb4edeb30d4a0fe67d082968c444c6e2f88c9e6577e4cc51438d8c3dd58aed1a8ff8d726cfdd69");
            result.Add("br", "b8fcaa1bff03abc2bfa1d40b2fe4562e95802dd1b992f0977027aebb8682c59fff145274653cdfb26f83fcf18fd940add546385de51e586c87f032516406e2f2");
            result.Add("bs", "c361e3c9f2e9e6c39178a8f4f04db0c2484dd8192fb6edf459147d914be03217136476517d52eedd3ad4aea6f9fcdbdb71c03e065dfe904f5ac4d6826cfbaf1d");
            result.Add("ca", "954e17ef98c5bd11f350475ec84b2abccd456b710b80c60a6355feed19f95fa8e3a41f37c6fd66ef1e04308b1336a96206ff081fa857db739f9d455ba5a2a69d");
            result.Add("cak", "582343e7cef1e630ab121bdef0efecd038bd7e9caa321a3aeb38945f22d0658f047dce4aeb476baa65790c8a711268d286e8a23c47a8ed8357251221eb7b902d");
            result.Add("cs", "5b50e3be1de657fbe50033fc692a17dc9be1436804b9d6c263050dccae96981695a1b9fb3e5964ba4f9e21493eb543585c4da3a7753004e93bef95acd3244599");
            result.Add("cy", "58e4de49919f6b5d0b9301582a7d6c6106e803ac16ea6495f36d8dc06e96d3b648a44ba565e5e08aa9deb13998d52c67cf3b1d336fa992599e4a8b90111a37a6");
            result.Add("da", "8b736b120dce47189362f6ddf9711708d3de6022eca9d8a996fba476d9073f0f0b22b7f8c9e28657c329ff81f715cbd93bab72b01753c3b7616d7ba71f06d52b");
            result.Add("de", "90760a56979722fbc4579d262c4a934edb4ce622ea14e926e818a7e243802cf1b5008bda926dc16435328126a7e372f991444c6fc9e376367cd5d1457f1f2ddd");
            result.Add("dsb", "0cf194357253201a1606bdf7e55bba4bcf2c7479cc7281e0c50918ea9f293ea99292e9662ddbb3ec4bb5bc7078e305dd2564b38da92d92e86813795983b42281");
            result.Add("el", "35ed2618938c1a6096e0d8ebf52bb5738a7699c0d03bf5fdddcf05f449dffb7e176526264903e9ae3670aa04cf05867b7e7fee0933d7e359e1e313dae852ac53");
            result.Add("en-GB", "c311c5db27982ac3a57f48d8f0e722180a461eba5fb33d04028ff730dcc8c93f21433454aa4e089ff2307739cd087e9beccd6daa6098d84dd82a639a5453010d");
            result.Add("en-US", "6855edb221391dd44eaef950c996d47188b840c62abd4c046ddc7a6e87a2e8c184793627243c414e124ba0db48d07a74f48f0bc13bc16392491d5d0d863a9a1b");
            result.Add("en-ZA", "1abf4f2b1793ab5a0c65d3bf74849b774fb1d4647192e7c6bc1682b1fb22d8c0823789467736398bb7935af0a10d293f17baf7f52fbfea6880a66a97be3a571c");
            result.Add("eo", "65946a9cf809c36e5dca62ec60a196b2cd9d7122e6aef813a7cae7949fee9ab4213694de30c834d3d83bea7502d0723b5f3e8cf44bba657265deac7764549b88");
            result.Add("es-AR", "d28df489051b851e53bb596f0b00644e39f0c17d113a30dec178228255c31d9e983e43d39e2d43547f77ecc5b169205fb803e2591d8c3faaee4b284e06a22a2a");
            result.Add("es-CL", "53af87875db9b8d8795fc75a888f2ddc16402f1a392b45de20d9175533d1ae38aa27e3186c1bc5c71c255bf484e8119f75777dbf4a6832c2570a1021dda72f61");
            result.Add("es-ES", "f035d641aeea7e5c1315a7573802b203e2bc5c38d939a51c87ff1164c478a5a06d10efdc2c0de291c08ef563f6d74607e72d532c48c2311f992a095613f28735");
            result.Add("es-MX", "e72e144b1a47d3ebfe307d498fa3a64c0d7301894a2c54561fc1c451be158b4a4c1046260198a6c18f42c8ca25b0e53e1b23c98c91e74ac3e57fa92ffe2d1070");
            result.Add("et", "9316483edfbc48f5ec05a5db6498dfe9a6acbd8895720f2d0b45733020e0efa74b33122b09f28f2fb4af65456011d5d880e4e5bd744f492f5815093cda921797");
            result.Add("eu", "fc8810a80a48aa7b2c1e4129df90b1250731902f5e24a815c2b04507f613e68fd6895d2be3a78f4dfcc78936ccc4b40a8a421543bbe20d4ae7ee5f22fd6647f9");
            result.Add("fa", "2033419bdd79d95577a0635aaf6dc147a95c89663360c22e8ff4a7ae5be7d0a13dfbe3cd9128b11de802ebf34d2e0f44496935a46fd63b8b0bee8c3d064db165");
            result.Add("ff", "48af1177333505b5f25e78436a19b4bf84393b1c2f7d1656e309187e26dc306c2de7a8960c870bbda8b028cca3cfc535da2ba8badfa2e25a6743786aa78b2f7f");
            result.Add("fi", "90684159607ae9ce50d5a4839e81ce94049720b24d6926464476061aaeb9a2c7445887d27202c67746167c0c4c0b9e20270dc36446e9b84b037965094e55a016");
            result.Add("fr", "f7ba76c6468c6a9142f55047752bd20634ed857dc180c6eca9b09767058f3b57db65bdbc3fa209e6b3958d63ac7c0fc72894ff1e6d080bc948197a0158a41908");
            result.Add("fy-NL", "e644385c16d55df50e8065983c42b92abc0fb90d20bfccda06513c118f24a885829922a2868225a8273c0bba2f902b0f5064bd93d3521d21542a4e28d9d1fc1e");
            result.Add("ga-IE", "53be289f956834ee4fe05fb98fb2c8690872abdb61480db28e81f94f0c8d1e2761ff9baef310834ebdd9ac688c7226284d0a1c7ca27d8b800c2a704d19479e32");
            result.Add("gd", "68afbd5384daa467c7fd2b4810ee4b7673b30d2958e9c865e96a236a5a8496647914e5135385c750a5e17f320e649ec25ec46e2637fb4fb39b6e9c1022119365");
            result.Add("gl", "975bab4d544fce2bd2b09eb5b0b7840cbaf3bf82757698d607fd13261b28574aaa632d3ca26f0f9aa175b83cb279c73108588b3c8e83653802ccfa86175b9f3f");
            result.Add("gn", "894798750463090e6a93a652f2c8de0b681a12b3e54e4a3314ea6beea5275cd92e024b560acfc64b4cbbb094af31acfaf7dca71560eb9252f7c4f69579d0beef");
            result.Add("gu-IN", "a6fb99c03c9665554843e83b0ec6264eac37386d3d5734fcfd65c810eb90e3a0fab940fcd7df9e93992a2b9b7783e58258f83a2f549df111f132cc05102aeff3");
            result.Add("he", "07b8caad2e7e1bc630f56e3cee251e4ded406724ef2d111fe132ecd1209635ff5b5bf861514a94bc6e970879184dcd88b1c41aa071aedddc9397acadb850c4a5");
            result.Add("hi-IN", "fd9c5d96eaecb4ae261ff0fc6ad84429259c8bce9e662bff029baa80734d433aee37e4e81483328377385048a4a89c745fb139730344a1284951bbfba209b7d0");
            result.Add("hr", "ef5b5a6fbd766c56db3efc64bc5a011feacddf9d7d6f4a6e3f25e5f01bc96d5c10b83dc474f561ffdf8b0eae7b3a44bdd6ff0ec93e6ef720076b5ce6cc26ed19");
            result.Add("hsb", "44519a771b20bbcadab05fbe996c18ed72fea0d50cf530b27f66e1b98c2cc3587d8ba66d1cacd1c368ce5808afe142d62d73cf0d65c8f58645ab0be2a18895f5");
            result.Add("hu", "f88476cf70f5b3f6cb60fbbee2a6d8a4fc9312a5bee056cee2df281c679e95517812fe51d904954511b09d813f7a06096456678c01bacaff89ec9b48ed29c060");
            result.Add("hy-AM", "b6b681b0fc11e12a70357be445a1e41dd7f1579d16d0621fff0686cde9cb00407229a355f1b5bacf735e52ada3aa5bffe159ee11acd3a47c3fdca5d0842d31d6");
            result.Add("id", "83bd152a8f7b289011c95d7a1e88f45aec76c4cadab12f062dce5ca63895a1383e59fa24dd5c5eb70533d846a1880e1466c4fbf0e9fee2bfd1717afe4c70dff5");
            result.Add("is", "a39dd184f4d207bd9bdd3c7e9a81abd44fd4c8d8b452c2c9ab5eed10cdc4df01898f9423a3c612fad096d88d4e29d205ccbb4ebd814402e2e58646a5cf829c39");
            result.Add("it", "7441815150447f77c86c917adff2004bd4b1738eaf6cc290f01c0e60325beb53992d1e65bd940729df6ebe4e0059016b99bea608901059244f137b943cedc76c");
            result.Add("ja", "ac2d09df75a522722c36baa38acb2a310b0f7b776ea1625689ec0e4c99631034eef493f0d51825c6f640e99e602c05958f63f25ff41726db45f9c7aff7c27c22");
            result.Add("ka", "0fd095e09d7f8cd25adbb02aefe0d91794311bccd3397ea81f237d9adb561a377f81ab0b691d15f70202b51075b43feb0be7f640aab8ec9ae2b61306b89b8e52");
            result.Add("kab", "49a32a37840610fea3e725694d83e1a1aa49f8c233e3eada68fde397b8c19ccbe2d2baba12de0cf243947079d508a17cdea1984eacf81a1f901eb44e5b001ac0");
            result.Add("kk", "d5c0365ce57301b15c2a0d96a4e00a34f4a43a122f7e7a49601a1cea6cdbf27e1e0f86d9559e79cfa80b10c2810c3197f0773a097054e062f4279f62df654c79");
            result.Add("km", "b3674bbfbfbeb08b943937a0024d7782b9d309f7aa76419d425caf58df6a1021040715f9a7d6670dcc624182f004a0e9ddb1ddfe35b71d8f6378080318d7fb22");
            result.Add("kn", "d8b9071cb69df63fa1aa0e96c5f0783c4b62b61d2c051e865e2531851b5b2bef2194f1c442dd4eb1926f8c5597e0580ec5c0263d2cdf06081570007c2645f6cf");
            result.Add("ko", "2d52cbcfa32a252a586c5551e7becb9def218b34d05958594060eb51dcfe057f8dd5e1ba5317c330cfeafbbacd9eb3c3a5428ae2d314074668a7b6774326af52");
            result.Add("lij", "ca1f3fb6c6988beebb1f2a73c58a5c7305fa12c13052af4b47b541649939d86a2c32a913c2319896898ddf0b32319aa8435df0ea83f86f0a0218ea8c8822d30a");
            result.Add("lt", "07d4158e36c0d8137db800fc5b9e169db1ca2223bb4ec886dd1a8dc3b09e9815d16c76bb2c175dec12fd4ce8bfd5fb43ade7349092bd30ad1e340645c352de75");
            result.Add("lv", "cb75975f2e3f0361fe0f053bb3942fadc55e6145937e6272382ba0834f2ac60a315e7c6765130ecf992c1d93db28d40e67297d724df23ff95dae77c1f4afcd2f");
            result.Add("mai", "920a2d0ddbc2ee0cfcebba60e330e6cfe87e14a1792d714a5cee457fd940eaf0c10cb797a89b7982cbc04d659c03128597a35d54564c0fdf4b583a8a79745e5d");
            result.Add("mk", "41ce20594b083b665fef20440daf9d9d5bacfb0a040d9be935cd878f44cea6e70bab4a9eaddd9e82fde9218e9febb06813b2fdbdf39f4211f5dd36e8c385e58f");
            result.Add("ml", "4c86730386e2aeed24562384db2a34c28a9d7b1b4b62f85448804aa764cca518e5e59541a43ba554f8cf7887c22af1b9d3d64a2a910e050c2a27395f2dcb0d57");
            result.Add("mr", "213bb3e1c2a201cde0e377e3ea751a5d11165e0e7d6393a52a3d6e1b98399742974a1dd881bf34a2b48131076e71216cba490e0a6e03365cba8e2da4db324e9e");
            result.Add("ms", "85259c1c66804822d6845a2ccca5aa2c91b32d72a014a6b18156ad509f9f9625d77d83fd269037d7bfd99e86b3e5c6b4da08bf30408d825484796cf63e374e46");
            result.Add("nb-NO", "12423c94cadddb54c6509d8d96166cdb4970a48b520808f66236374fbed097fbf3b8ed474ac6d3433a055a080e9dac47cc5055660580ab0df39f23c37c8ecc90");
            result.Add("nl", "4bf06b171674d093aec3f91f1281c7a58aae6959f8aed987543f984ff16a6ffa94cd91c9654f93b77214a91510ecfecc98cd3e47d6bcdbb4d0055ce076df70f9");
            result.Add("nn-NO", "5c70fb144bc094b17a27964f627530327e6bf75df6e76473c55887af84d78fd8750242f2ee73ede14f4df8c4fa60b7b2e0a382de8cd07e5938d5d30ff4e650f0");
            result.Add("or", "1373b91d375c73eb500d029209f32988dcd0a3cf8a8cda24d75953868b04c144a19187a4a4fefce530cb0f250de343b3ce68e53ce380afa2062173b7b3406bca");
            result.Add("pa-IN", "c6ac3bcef8b1ae4cbf37a17b60118f021a5b04351e1aafebc8a2b5a599cae895bab1837dcf88b3756c965c1852970c16ed90ba7f879feebad9a3b36fcd2a6c2b");
            result.Add("pl", "0e0c2fa9aba4ac1624994377bfedb6d58b40cf026a1a49349f92e6677373fe40b32623e3bb91ea8b89057ae672f5a6dbcae28ba1c0fc4aea6cf9012b8841f3cc");
            result.Add("pt-BR", "ce053fcccfa5c3d8fe3efa6a040ba8d3feee31f8fae811555fe2c02aeb7827448c14c447039d60aeeb47c3594ed1c013c95a76b185557aaba523177366f20601");
            result.Add("pt-PT", "01fa4d5f5518d484b1cc01deba646d8a357c8aade56a884468dbb0e724f279786f9d7d7a13f6ce166b50b6cf333907fcb3f9b40c554db4699147b65082bc4db1");
            result.Add("rm", "81727979a6f70528f87c2efca97ea188746d821bb7ece0afd8c72c62206e178d216eade82b0ae25485211d67fefc3ab40a84be2da11dd7c3969229f941432fe3");
            result.Add("ro", "cdc484609d4a39cb11b5987014c517e916aaa89dabaca5dd236746cb3784a4fd2f9f610cd8e5a363ffe14e2f0733d0772a35df6ae39c51adf9c768d00c287082");
            result.Add("ru", "d2d7a8408ee6667fc225dc35c4e9198f0374d68259919ffb63f026b5ee29654f7976a87a7d7a6e9a3e25bf92def6fbc3ce99d7d49675c7c255afa63db2498bb8");
            result.Add("si", "f5874f11c3b09d47d34193464b5d415de93fe1a6f618014904b1a899e06f2c50370378af19918e9976957ccc043e7694b0b1ed5d998557521be438e58abd2ce9");
            result.Add("sk", "98f4bcab7b2667d5ba8d7777aa800ee298e547e0c27f683a3c0ccd7c67b3d89e9db90d500bc22ff635d36f8214a48b7964f8fd09a813efaadac50a7c0028bd8d");
            result.Add("sl", "a730c7b4b8d06bf3555e28b5676edfc71501417deab73f04fa53e28d500ecb11e3f1af954a83e9902860c1c237320f555c64c32fb7048a38096b649b9c1d558d");
            result.Add("son", "5bab96d6010debbaeb674c179f226b4c5745f620b78f4b478c25ad46a3ba6c3e74c977364c05dd73ead7ac607c533a23eb51ff6b4a3210be35ab414743615ec1");
            result.Add("sq", "40fb051f21a1bfd89814314a8080580bc729788bb7b1ff68c066c47b33b094f4d06535a198136811743666694483c8fee9fca5e448016d0c3e719936ca90c91b");
            result.Add("sr", "407c899eda17e525228e968d040c6e074a102d2b6921cfaa6ab9bf1ea3213ae832a6158863edbd06a22b2185a7b213e44696baa07ba33be45ceffff15077a467");
            result.Add("sv-SE", "8bc99aa17869dd1882d280061ab412e3f7c9fc68a9f00bfcaacb42312a498a579834df0b93b8b4c56b0097d07989c9e2cb81d51bf6d1376307db0a055629b871");
            result.Add("ta", "7b62f2cb9f2c4a7ecf8c45a8ba6544eb2bc0771a9da6b486e5d95520e3a5054a669e076a90b8b8abaa1fac48ba511f4ea2b35a5fabb8cf8ce6b64008ec30a0d2");
            result.Add("te", "0b04baae77cdd6a3abd6b4476f14577daab9f6ec66b892e9039245349339f430e342ead76f68cb8e5dad70ff4ed7910dbf68115848516a52430b3c55ada6979a");
            result.Add("th", "52a2adebbd57aa8d94cc683659b00eb701f3868678967547397a79dcf22831c61bbb1c41b24eb697e6c01ba7c6aa49d720593ca14dd3e015ddf2cd0a856129dc");
            result.Add("tr", "f9b2679786331fc916f75258c153f6846d87f232800da83f82862985ba0ed506983b3d1aaf5ed494a8d9498713a7f7b85ccad4400896c3e256e661f162429a80");
            result.Add("uk", "3da996d0cad6ee541569e0301b225eae23b1982cf6f8f154f7d2549411e947076f361121e0b6eb505ab65b1f8cf179b188ee7a0ae026ca874df5b8936885f3bf");
            result.Add("uz", "4290c9879b602a319bcdef0c1b1a5d75dd26fcd22dec0975ff26130fac80c42160858d84ec38a4bf94226d5aea5d78b6cc8e6de6e6f41289ee25c956197e7bda");
            result.Add("vi", "0d48f6c7dffb3cdae08bc7476d0e61d491aba09b4a62ecc6090479e454d92038c304d848d9d097971cb9c4fee40286b95e5c8eb14d3ac6fb096e7ed75ea3a838");
            result.Add("xh", "1e160335183bf0110af048e446be016d5b908621d781b325118deceff7f86c31d80a061cbe9687513074dca4309c2d3cc23e0206f07dfe763b9f835019ea7c18");
            result.Add("zh-CN", "0d324cda4d589b0575d12dfb652687a8267dc7ef499d30cf015fdd46f6dd8faed12a46ee0b55391c6117cbeb21a9914dd5f5292facab63f6b3899d1d1a6aef69");
            result.Add("zh-TW", "89797dcc1ad33942862c9de541b289f1a4c276b5a415c1949456f94002d1a4b366368fa82bcb2b92dd7612633f884fcfc8bd72aac6224fdee00864f0211caa55");

            return result;
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.7.3esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "11666baf912fdedcf7b89f567eb97237f8b45b7794c958d64be9bc44f92e887e0b8861576d234c9b57a00563d9da0ed87bbd3588364881d5bff37c2c037d54fd");
            result.Add("af", "2cb482b9c0269117b23b705cc889d89d323940c26ac7a15c86676f071b551f737d1f62eb026044273429a08b66dd4c50bfc17e4b168ad60cf421a6921991ac86");
            result.Add("an", "05c51b5bec88a6218251f08fbef459d496fecf12dc120869973ce86ce41667159aabf60d557212ff13a4fb378f2fe4711cd49cb8c036d3df3c48c96d3fd16d4c");
            result.Add("ar", "4d68dec10225824502232827f8b259c738c508cd8bc4e8a104f37245c591500c19618919f6e8378ff9ba51378f0fbf3b5a4967e98be443b90bf9ceac84a082f1");
            result.Add("as", "d80588c66eb9a2ab1507682f0499ad9074e8190ed9be745f2b85c150c97e196b501db671af2069e8adaec040eafec3a94b78ce7f0bc0d3dad392b88250d7f970");
            result.Add("ast", "31abf9c04c00edc178fa4fdf71319af71b17d98d0f0972e5179b76eeb9df2f9c6da98563d60fccd8f7eac3acf0924a2b7e69ba5aeb9af197091e140a728470ce");
            result.Add("az", "789a27ef405ec95d654752a202a912ccaf1ea2c62ea2d110aa7fd82993cebf3db53da9ea053dd47c4bce6cbb22961fc5e77b24f08e8ef3a1d7c18d70072dd369");
            result.Add("bg", "25dc83b5ce58c038e44701a69a08f2d896f0744bf3f6a975342782e82832a526b7b79f828787742faea15ec70aae48c0ef21c6ab22e511f728339576f7e6c238");
            result.Add("bn-BD", "beafa352e908eb24dcf387057dac8771659344f8722f23110dbcd91983780fb383202b66b2d826fdef7376933f5df6dac77c8306686758b0b82e36711efe3f3d");
            result.Add("bn-IN", "705ef62d00bfc7ab6e9e274440a203197dbe4c76c5a826733be8b5438a7ba0602242f62f9192ed6dc21105bfb0f6f1e626f554dbe6ada4eb507fe25ffb965bb8");
            result.Add("br", "d58b6b52f05cd0a84158403b93581e3645489e30e563933ea02d49a731fc6b0ac3d54cdbf984a36deba01bc6dd301aeae92c46367268924ea65b6b4568fb4c07");
            result.Add("bs", "a2947fbdff6467c54e239d92696fc6ad4cbc135392730ef6ab1fb94c16ef1dae82539aec799cd37fc3bf3b8237e8335b5f2a3cfb00a003e1168361f0cc17fce5");
            result.Add("ca", "77b6236e4db5b6694a6f9f8a593036cc97cf3a189d75b3df581323540b0a492a09300c6f408d4ef2c24d17f45fd67b31b4dd7efbffe286eedc9b3faaec972066");
            result.Add("cak", "7ba5eb6c455139ce24655d5afb3c559505c2bd63d3baea8bf0ab42f2a75a5814cd16707218f06e15bac181ac0e754bed969eadf5dae04035f9189a3530356bba");
            result.Add("cs", "b6b815ad8e9c45ddd596843bb150785f27b5fa2bf0798f7a9ce9722841c4f1a83739668d91b6c27ac605941f377a2a3162949583749efbe2d20d561fca020911");
            result.Add("cy", "d31a47698242b6b012c8dd1605e047e19c1ae64b14de0b523ab9f5ac36e310d9473093b274a7666fec0cf98ed6f8406e5944b9f21797f0e7812d5f263a7261aa");
            result.Add("da", "d8c5303879a8835edcca1f109be5be4fba3d528341ad26c6e938840705aaa270da0a75541764bf7646c3b2f72239c60fd29cb2279ddecff7d51e82faf33a5830");
            result.Add("de", "1c654d301843603dd6ae65b856af72886547f30bdfadd4659f853c1446313626bb578002273dd8ec6759a7ff6c31725a4781afbee811d5bd692d02991cd1e5cb");
            result.Add("dsb", "bb36ae942dce43bbc5baef519d9b560daf036dcc12e26f1a12cbf5f02639da3f9b9f31f6637d78358ab7061d44ecfda02c5def1b4577de37d523103068ba8e0e");
            result.Add("el", "e06f0758520d64be2ceeb66b5d4b7d62d82caf7b6e5c558657d96da043ba0cd89b9cc8f3ccf7f9796f77f4f5dfd9be4eae74c564e0a28b7316adc914660e1794");
            result.Add("en-GB", "8f0ab1ce585cb416d209ee3ea0bfad73e1a1d95912f08a5433dfcbdd5b2add9e50e82f898cb024969b278aacfe983ea5bb8a09b5ed0a3fc836b0e057edd52ae3");
            result.Add("en-US", "41b330adcf05004afeedc155b90933bf5db005e19e5196b08fcee6febeb6ef031ebbc3651e4b540b65ee3c0611322f102c181b749e616eb8a135f71bb0ff73e1");
            result.Add("en-ZA", "d82f0a5b70c1f779b52496f4983034402b4828b6bd9caa907cec41d606d52bb26ed599f95f7a1099c7280af4a7e34a7d0e6c5207dc8e38b8fab52b3aefaaed0a");
            result.Add("eo", "c80267dd3b9c7539dafd2c18453e68639d0ea6f61b9890f24cd49b7187acc5c84e98864b2db3163507e6421bfefb991d53b24a4a305d7698417735b101550588");
            result.Add("es-AR", "9a2f1f7e3fa0ff1527cdac6785c6e8b02541e01839e7a30b7d87d9d94e700881bc7fdb35620f59854635b3f58977dd19a0187daff3f58369ab1acb0baf0fb701");
            result.Add("es-CL", "ea6f47ad6baaf3d2eb52d3fe2140da9199c0e1f33b938597c446e2670fba23eb9fcaf0199d236ca715946268aa302589ec1206b2be8bfa8c221b27ea1da7a572");
            result.Add("es-ES", "8f771ae3e7723c1aa52847054f90288fbdcfc0b31a8bb0401fac7f3a870d126e2331f4cfe34cb315383df0ab7b0ac3e9272fd7bd5b359bf9c8fc3e7edff2116b");
            result.Add("es-MX", "60dd311d71ec8df2407a1f9558d08ac037b94261190d0b3a3a470d2b48f6b80dc19dd8c932068e92b689704d66a0a9d56770ddd92b202860af4d8383ebeb9c27");
            result.Add("et", "305f26d1f6ce73d347cf9acb22821dc95b8db4ca2a64e8a688d3b6d389b9a45c14718879c4991e302281deecc4d24c5d62c91c2dd90278ec35bcd6825474541b");
            result.Add("eu", "615d83c684d12855549a45c6cafe319d32567c50ada1a2862a900b70c042af3f1e41e567b32ec9bb16045611a4c1f2e5f7dc0de5a06e91c2691118dde480bfae");
            result.Add("fa", "8f0769d895990165c98ee14b1539a9729e19e1a2f18c57880c90af197ea37fb327198f0f4383e72023c4b8e103cb84308fa4a87aed359170c6ce0c92e5aab053");
            result.Add("ff", "100f897b79047ad1026348f3e24ba04767195ecc8e61e7192a7caee3c70a5c3e75986b10812991abb937e0e7e3b93c326a03e3ac8f3ebb89c4f709e16d481a99");
            result.Add("fi", "f1b71ba950963db00fc0ce2afe01601d65dd9474b8012c2bdc179b23b9266a00b4ed3954b6504642b49cd415544a45edce5a28035de5ca7d4458dceb48db927b");
            result.Add("fr", "c3aa3bbfb00c8047a2b44b64a7502c09be0bf47deed98b78b89b66e9288046ab6c5a8ffd4dd6ec15919daacc72b083ce6af660acd786cc2486707b679b35721f");
            result.Add("fy-NL", "c40d79065be8de24f8e02e32211b882fa27f2af908a85b489885b2543fe12901920f1d03b0749ff6541aafece44f98a54aa6fbf53c61faca9cb678121ad0f6f1");
            result.Add("ga-IE", "5043397eaa2f3a364bbfe4b43d258692a94282643ae8e5e34a2bf60eec9363bb3ac0b702d4f3cc436b3093c4ec19b287184d96e809b08984c1becd887d3fdd20");
            result.Add("gd", "840154abfd8606f7aebf9028d14d7e4133e2ba1ff5547318299feed3e943805b33a8a1db30589021f74dc8e56d636919631d0b297448aeaa7ab8110dc959c41e");
            result.Add("gl", "4422d681568cb06b71270b7ff740bf1b080c1f3ae5fa9fa06aa52573ed9c4b62707bdb8148f2ad1c41f3e8f809e2eca1be3c3bf77f1a93babd32a209c9cbba7c");
            result.Add("gn", "7aa8739cf31d98481e0e6cde361d886916435721ba9c47a5b8b898d80400e7b21423cd57adb171d24372c9f65fe584338a5292df0658063d5df6d84d59bc0d6e");
            result.Add("gu-IN", "7fa9b23fb0a6b50e6ebd230a79ead1959a087afc0b169861297b43318c695127a424f1b304304272f204792bbd17f324a774814ce2e5fc898e2ffcc516339d08");
            result.Add("he", "a2a890bffc84d42ed59aea34de865aaab94d69472c48ab2ba7d0b4b34cecdd5ccfba9636696c64dc2136d54e75cd6305f7fbf6ae78cbad9b7904c169e988510c");
            result.Add("hi-IN", "7a096172933aefc60df8a560d5ab1b06ae124fd34ac349ab903a4da33ec9dc94c9117ae46181a3711a0595d827e592234dbcc7a3d49c0a73ae226104e9688572");
            result.Add("hr", "b9a55a6dba13eba32777e87f4aeaa99a5dc01edca18ac0b32c8f020077dafaf7f0903e838c26b4132941d329095cc0f0c1db2a9fa5b2b9f319e82da32d61943d");
            result.Add("hsb", "5077ad5432a3f3134d2147f2f1e3fb1f9b5fca95efba0bff467c5a8392c9abb7aadff9ea8a07a2ff7909b3d67796fa247eefe39149c061a186e66148fa7a208b");
            result.Add("hu", "9ed963b0bd67914e9672e8a31e70e7fd76ff0ae975438a66b9e1a7a8b7c0a778bfbcdc7085192673a2d1381966038b23b7705191a50a989502586e624b41a6be");
            result.Add("hy-AM", "ff722349bbc8c9d36d05cdcd133db506bccffedbb6b548d33d7f18505e0cbd6d3c236fc76f192183f3d6579cef6d0f431dc8c67148cb0e72cd72731a0fa69add");
            result.Add("id", "16ed04479b4f30f222044e1193f054bbf1449ec4c1d63ad841159dfb80632486d6d53427365a05ec99c12cf912fe6144f6c9ff0f1dc87dc81f64c926dcb11a01");
            result.Add("is", "cf0ae58df326e04bf648b73420d320bcdbc84436b550ef739ccfe782a54b2db8d152b43fa8a626dffa97d3c008d490db03836f73492c36b8c915d4ef6c2cf98b");
            result.Add("it", "b9f67dc68f9f39defa519556cc76ece367354f2e69d4af7ef24d500d0a738736dacfc7b40b3bdf03452cec205f99a976a7f4f13b2ac954ee05bbb547632006b0");
            result.Add("ja", "c23b89bfe34605f3e00c6d7f7c9d52ada7836a9aa749559ad64478cc3cd56175c077ea0077b968bd74146c12005efdaa43346b7b0ca6d21f6c7d81df9add5db4");
            result.Add("ka", "1783406b721f9f8a7a71cdb24854e0c00af896f32940fd4b8409c384d69b7ebca5cf44a514e6ae2e5167effd4b36e5e589615d8dbf294ffb3f630aca0a1c32f1");
            result.Add("kab", "4cb393753e4b4b34e440233cf6020d8b3d006356aa40ba8932870a76f96616bcff8f9e4dd1318fa7bb3def1715db7001715ca97f5d4ffcda8b8350d010c28a74");
            result.Add("kk", "d1e4965e91c8cbd299183d98c9d6e430a7a9bf9d9808d4a4def57077f2dab7f885196be18977a9ebe136671aedef7488f6f6bb578912f1329b3c24f65399b361");
            result.Add("km", "65af00cebac04b3f90c4d81c8e3c7f87eb07a0a89d9a6bd2b7ca2aa3c4ee32e89d2cfe7a2e505533c10206f14f4a6287991ab0488030e878ca64f370da98e931");
            result.Add("kn", "3d85222bd3cf3ed8fc9f8de6f5881edde260f1f5fc90200b338fca37d57dd532b6f7fb9095559660cac29ab2f1f5a9dbe6d9e9191e18511c63c9d1ddfd06f108");
            result.Add("ko", "25942a22a8e0afe0e5185c11422371c64c836a5fd13122eabc9daca750780218e251fbc61ef3476e91bcdea2df4cb5cb8e4db814ce016db721cdb6de39a9823b");
            result.Add("lij", "1a120f46c2c822dbd61d7a420a379bc968e4776e20de93724bf1e8203c9130a8d288c4d66ad357af4523f89b2dadbe764fe020310d301168f64c0f52aea1449f");
            result.Add("lt", "92586ffc8814609d369a9e6ef87b8e550b3ee0ecd96e6ca25442aea3774cec53543149f00676dc92c750c647a68ece49ab61cc9945b30cb5734320d56c57aea4");
            result.Add("lv", "251d2450533a1c647e6a70784bda87e4d03fe55e911be92ceb11afc5ebbf308ce1cd9977fa86917cec0f83c5249aab7c70aecd8dce4f25c5f4399c242ac6cac3");
            result.Add("mai", "0e0c0b8cab4af1b323d4374e3213b45c8cfd53ffdd44a20a9ef98bbc2d68147506f4fea56efa1f7184d667eb97c8527c2907ca4ab2554ac7414e549795db76db");
            result.Add("mk", "5801fb9ae8b044fbdd8354e21f3f32d7e3124b3d7c5045a3051900762ec917c9a46f9da9fa857e7c5784267aa8ecc4644078ac4305d297b35bdb20459e006126");
            result.Add("ml", "0460eef27284f9254b4c236bbc9e8febcc58e78f1093296365e1312c6921864602093f46ee98e36288f3edc87d762904676da86057db62d4456a55989dc09004");
            result.Add("mr", "34e24b28c9b10b2d4e2ab048b89b240747b366a1f5be20eb5e0ade89c33ba3b801bc7d434d07a38ee79f6b32d5e3aa9240dbe83e5c0ac58cc57d81723ac4648f");
            result.Add("ms", "b9ea5f2f642017092cdda606846a3ff37dc83c99104aea0f9db6d15c1443aa215542311313f3e33d7b8ec4976e548f2d90a25bcf6ec37054f9230833d385cc16");
            result.Add("nb-NO", "f8debbef50b3b9e88e88297e5f384a42488b4d92a97f456c655af1465b40b762ba58828693289e23cfdffe8fd5fd0c13fcf37097d6bd8799ef2e375bae5d7e00");
            result.Add("nl", "bff29819ed4898da005f58860fd021bd01a713daaf9373ed13a9ab3bada33cfb22c5154215b888da3fb3096667baac92062f26a9f31bb7e14e33ee39e824700c");
            result.Add("nn-NO", "d2362075240351b58e92703fa19da303a7b97cb0ae70f801d8394a76bc29716c7ce45b8ee5d152af3f363982d9b0c8df8b7256adad2676a66b9283cb5a677db2");
            result.Add("or", "2a70e99186d1feb741f96e3d4a2bf5a66ffce6b29947df115c39151000cd721a4717df86c13eb0e2a653b78df71d645d7b6b8ba2cca914e4e28b3daa4732d9b9");
            result.Add("pa-IN", "497c784299e128e4d5d49371bb849bed78a93ef336f16c3ced26217f46655110e4ae6ff3538f82d0b9d3a5707cfad2ad8ebafacdc19571e84fbe7e7e50e4ec1b");
            result.Add("pl", "8f1c316cd3397d7d567739dab3b7723d63761a8f7cbb9808aa469cfe01fb6708718534ac677d05df867754ac0cd373b793b39ce3692a5d20599ce71cd64f18dd");
            result.Add("pt-BR", "668a6a80ea14ec5b91a92faa75f8f04e04bf681270de1d87d1ff05a826470ea7fd8a1a601c956dcdbde48d86d1afdbb853ea6fe617e55e36e8e791d4cb86983c");
            result.Add("pt-PT", "0d9a9676fc81a2653ad7b10c8375dbdb15e0115ceea08f328daba46c385a769053da9fabb394b02a0b3800913fb0066c3d640eaf30141cb727a7f7925dc321ba");
            result.Add("rm", "08d1b78af4da333804a899ee83543dfb3542f0e0b3379562aa21328a647415679d05be1c8d225645bd9a339cd3b58d332286776790c25465026a07e0d9aff9e6");
            result.Add("ro", "6a89446ec2ca1e2fcbfb01f6497b57cdae0ce10630ed23839431ec4cd2ca85b4c4e03158bdf1f5d9c09accc2160e767e9af295cbb5e86581f4ccb0fb194a4bc3");
            result.Add("ru", "9cdaa007cfdc290db1a2ba7aefb83970943b2910b654b924f44765c724175ac7d6f5997e68995701cfb54ed484a6201c0a2d78fb56e4adc8c7a2c0913a1444b9");
            result.Add("si", "0fbcd188f3d693fd06adba11ea30d46a73ef7da68509960cd2a7151c14d749fca7c9245a1f06e197b8fbfb28b4bac93a5b27343ba7326ce1a468d7f10616d85d");
            result.Add("sk", "933af0b1b68b9c1ca35e8d813135481b12c8befeaa62680d61cf30b82397405d236d9d8de51590f34f7e1fa59aba3a7b21072aff2b8dbcd57a6d977a2f96ea6a");
            result.Add("sl", "493c4a7a7cc8dd59b49a3a2808e6b2a1044498e7bcdc46d0b2c7d51aba8d148c29213005aa99db8826ceb5471e0a9a9ce1cd4d6991356fa3a3739378340f076b");
            result.Add("son", "87180dacfcd2e84e4d9f22645a56e05d8f470e1c583053c99f223e7c5b645e66a4a38556924fd6efe0bbdccc6cf36ed21548947e9225f79a246c5c9dcf2cb09e");
            result.Add("sq", "b816fd1bef79bddc7e005031375fa72358bda75d89aa9a0a860ccc9f9266c55acd2716d40bd6a415d3433a1d96ace4e5825badeedc229aca056e2cdc13c7e993");
            result.Add("sr", "77ab4fe1cbdb7c01252a28da0ba0a16913abf1c4c741347e69e8809825466c86aa0bc3e08047ab4f75c3a4b3b79f582742d488ea30d2c75c78ddbd5fb66af38d");
            result.Add("sv-SE", "7db1f3d8d229b5427566136bd24cea59d17234dd8d31665a4954c416452854e847f98e600138f291e1a5510e9c43f09d74401101d5a2a6496d8428a96394cbd6");
            result.Add("ta", "cd1dabdb4b1d5c58178d694e5a5c400cc893e9d8040486b03157bb769ad68613e18e2de6485a65a2f8d830a4eac00bd7874e088f8f6e1e93161ac3b316afc5f8");
            result.Add("te", "c14eb28036bc3c2f6e5046ff8f68a668bc9a42cf65670668fa5b49d867e9925e87b83e3d92328b22352014ce986c3fc1d7f9ac0a276fe975fd361a4034dd438a");
            result.Add("th", "10c17ce6407ea915938f725db334a12f1c8dd5d6b1e49ef4e855ab36ee5625169e691ad916a18717cdbfea6225e77b796a5bedcd427daa5a5d67b96678ff3861");
            result.Add("tr", "ce828d66ca783668f7cec3ef0b0c403449df2d73fd9e36f686a12da4608a53e87ba0ad6a82878177d70a81f21300c3ef2253b57082f9cb993f7fd3188330c7a2");
            result.Add("uk", "4d8a5464dc2124fc2da865d3c340dd0a28e2475ef6fa303ffe536eb98330cff42eaf817f5c435188e8ece1f2eed47445fa5ecce765ccab1be2b32cea6e6d6146");
            result.Add("uz", "8c0917f4b292d468fdfb94ab235e7dd250a143c5da0e6d81ccbd88bc92a48a3a8f04ac8e9af62663ccc337ad3280dc6ae81eb32ece0270a113488e71800ff9c9");
            result.Add("vi", "562ceb4bd051f2ca427aef2c41b69b6197d4c31a20f3ffc6ac4b7babe699166099e6ecf638f1f216bd7f86b5bf1ff225314a0f0e5321aaaa32c419150a91e081");
            result.Add("xh", "56bdd9f0e09d96cfeaa87f219e5bbb72fb000657f1f3a303869a428cc26275edbe6bdec8c4e2d1692e932eb3480bb3eaab7ad2debf11d9ec8095ea2590f0956d");
            result.Add("zh-CN", "d15fd6657d20136d8291dcf97d89e7f6fc108eaa3e5e1867a70cfb1dec11f63feef20f3ab327587b72c190b9f23ab5224922f9038aa12aac15f1af45a3bd3c31");
            result.Add("zh-TW", "ba31752199759712b07911ae6218d46a3329b67a1d9979da97bd323a33687bd7aad805948fa0aadd20aa802dd661ec8ca8e68acb8e38ee148c03135c00d03204");

            return result;
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
            const string knownVersion = "52.7.3";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    null,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    null,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
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
        /// Tries to get the checksums of the newer version.
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
            } // using
            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be update while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return new List<string>();
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
            logger.Debug("Searching for newer version of Firefox ESR (" + languageCode + ")...");
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
    } // class
} // namespace
