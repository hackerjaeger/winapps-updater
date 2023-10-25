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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.4.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "9a94fa6e26c98d1728fc585957acb32317c8c700bee7d1f7cb5b313723195f084d178c27285a6c6578dd182b3a709405ce8415483ae25da6b02448c1cb5b1895" },
                { "ar", "6332a75ff4ed15fea950e657cdb01a9d17cd2f615e6dbc61c71fa08f3b152bcbe825bff97111ce619baa1f93f4adfa1699e8347d1b97faad495e7eac061bfeac" },
                { "ast", "9f708a99375445dcea2f61260b048db15dc2dbe768a563d9291b77266655b504b5efafb4493c0d6d19685808ea0ec6934731d315380ee3759d725ec0bb0c3a08" },
                { "be", "895979780c56e4139fa529ccb33c1adf4a35499b2f29aa90e9e4da2047033d5dbb12b5ba1c05469a1069d9d211a69d96080ac07f47a17e9252851d750dc1ff0f" },
                { "bg", "74f8504eb00f2921ef8021ecdf566f5b826004430d7b6ad029aaba78fc85af7af06806392f1d6adb4924a4c42852179fd40d05e86e393a9fd99aeb4953ac5ec2" },
                { "br", "75fa76dc3c2dc6dfa179786ce90fb5bcff4c0d5bac4ce3fd6e3bcc3192d0eda8d1147a21f3d1f41d2e9bbe6e0cac7516a3da2a1fb770e45cc42e2b5d47ba12af" },
                { "ca", "2034d89c3878f7f614606428c0b72bf3b2b4512295a5c4770526ee9dc65cc7726317629a00770c0f043741ff8891ecdf3cfddb484bf930c1342d2cd35aa374db" },
                { "cak", "c76a29ca09d4900cef1f06474cf1d2ecd4f4d670674dcbd40a876f2c4dd6b4d7df6e89f43299b60daccb682827520bb055015e6f5f86179eb58feebd11690393" },
                { "cs", "2ce30e2ba6463bc950c378524daa780fc6c7e4da8abbccf7dd5b07cc0e4746d3109d86f0235fbdd402e5825c36f45444cf53f6f350c20c3f3611aeed7e2c876b" },
                { "cy", "9eb332632d14ae7f5083294b51cb89a7ed2ab358faa93b7c9f91445138dfa7a3a8d0b5acad7b91a07b64231700d938e1498a1393c81eb85214f057ee12e66349" },
                { "da", "9fd6350b06c2dfdc7d85b75a3f49e2c93850b2bb959a38448420fa454391ceeee06334f62e598aeeca5e83a0f5c484b86e781912d925867c34ee156c8c4f7e02" },
                { "de", "327ce8428944a0350d49ab3570450e68a0ad9c85dd3b556dab0649183c712dc4e76d453671636d35f0e8585f22fed9b02e0edf3d52784c64218ec715a34cd695" },
                { "dsb", "35bdd6cca615c1797d9cc6324f10e1a04b5289de39e8da3af7fda14be4ac8628153ad26cc4927db861270ca37360976e8f0ff50588d372cb89ae820c27f1cce3" },
                { "el", "96670894f32569dfa8dae5da1c7b475228e64f90bf8f3a1712d15ba27b316c897025d4509b315f939b9a6912b8219568e5c5277f2991ace27b0ce455b87147e2" },
                { "en-CA", "1332a2a4c8bf01913b873488a40aa96abd52b69484b7535726186944fb221d5ad5a8da14cf02ab5b480501d8bbc4bd32b1e91c9a171386a610434158d9718e92" },
                { "en-GB", "3b04c3f1f73782eb833ed1467f93de6074ca87e66734ed91920f1e6fe0aa79e2c2b197526fd108608cfc3b1019648a3fb607fc621c703357ae145a6e940e2b6f" },
                { "en-US", "2448e04b4a4cc15c59f1340c1fa0f88abca4afba0e5551755823a44f0db4eaea564f736f463e95ce33078b1675d954943035f38805577f45736952f7e7a8d4fe" },
                { "es-AR", "ef8c0736aadd91bd404a5489829432f8d774694a21d6bb6d5b8efe2245f2ff483ff387533872ec9c7bd06895c5b740da8a5c2194c4ad5c493652b7af1ae891f8" },
                { "es-ES", "d24a817bde63e4ffca09532ee259493d2c3ca96ef0184c59b173282b52ff4d3fc458112bcd0d164ff208c14a5dab50f53a95ae2d20d50ec3f824096c99935b3f" },
                { "es-MX", "8c20618b81634aec74cf4214741a2f5948d5ce83d136ab5766f87076baf353481d13aae995a440d546e70388ef5f349a4037b2683d37896a064e1aad16eb2ff3" },
                { "et", "a1e4c06a2bd356281833d3ee36a2375da7cbda3bc110658b54f52c917f6bb8b301efde64afe918d09fe062b729324f0fd6af3295dd0cbbd01d71d405f68c4909" },
                { "eu", "d7b5bdec42db69a1f4f921e8ad65fa5d9727b23cf83bd8cfe4b89d9eab9bdfec5d9c45225e7dab15af52df7e35afb5cb91815677dc133de039ab247883e66348" },
                { "fi", "0942a174adb85062852ef92a6a1e5986f65b0b5dc9654dabbcd2ec8f3669c690863d944b9e42e7e89cbdfe80518d39ddd7819564f8cb6a04fc5a9eba8f1d7877" },
                { "fr", "ce373045ca96447fdebea4ebd953db2556d28ca9d78cecc24259ebe6172a9808681353a832a56073ad6a9d8ccbc1a4aa3916c30b475f5a66f3893920d7f67250" },
                { "fy-NL", "c69d91a94b0b2372262fa048bb1664f453457aba01592248fd41dccf0c4df3e00f25a922b9338d9ca1415d886b65ce284aff13d9de4504d9d2f9347b49966f70" },
                { "ga-IE", "5a03615258da3ed2d5f44dfbe017e7e59d0850f0ecdea06f20df80a2b28379422c031deabcc4a0424adfbe553392e2e8f7ecc3b3db5675c336b4f46395637b61" },
                { "gd", "aec1c8349cc0db9cc17e5f19a90377d0dd424eea3b28b3782e578ff1be49cbf2d47f7c39f2d0b1371213bb5d5fc6793fc7dd8f07e0af9ed4be66cdfcab7df5b8" },
                { "gl", "76e7dd9fe86f0151853e3010632777658ba61184f4b1c9202057ddd5a210c178f278a3d929ced42629df4aaa5270ae107c0b50fe56cd7976634908f1fef05b8b" },
                { "he", "2e434e6b33d74229d8498130f6b274105ca2414eeadd3985755cb0946365e711c1a841cbeaa317aa7cde70c731ee5f2aac38657e169039eb6f4f64eefb5e48f4" },
                { "hr", "e49d75f9cf5ad1abcacfcb2f296ed0fa5d82fdb81f7c898759435b96b1fa37b19989306c6a030de0a8f3bb247f098012f0d6e1861b8ce616354beef2b8543831" },
                { "hsb", "0d88abee05b0f9781da21901347b5115c0a55c75e7b52a00cb6db7c7db39dedce27b776601e88d43767c0a5fc1b27f7dca9a8627ec90e6fce3fa4888af4c43e3" },
                { "hu", "61be35cbbcdcf7c2b5fcd95265727211b75cdb3ae5049016d4617127beaaa25868d1aa0c6723fa43559ff0c616fdc696fdceded8cc80c643b761bb6212c0ffde" },
                { "hy-AM", "fc1f8b7451906c5025c681af2c94b666b958be97012d75680f3793b1602affab04af0af73cb79c02bda71b156035b6fbc3600636e461787d02bb4f1ca78c762d" },
                { "id", "911c7cb7c7d35bb5539de4aaac8896a60d0fc5d96d6a3401642da7d981bc96f37db0565a5598f9ed3fd2345e324a7d4d536a0f1e64e85a7e2d9cd979be8dd07a" },
                { "is", "23386ab8da8e655c90bc8db0db7a68eae481ac94ab0f75eeebfb7cd1b75a164002f8ee42c2077d19e17ed68af214e73b805f8eaeb697a9151455cdfe6e149928" },
                { "it", "1106071256996551d20e7305f322c96323687a08bb84435e25c3b9f4b1ee7401bfccd619af91cf0ace06116bedda00ceb0542c4df8690d975a1c830fa8395f28" },
                { "ja", "d5fb334c24d7246045deb56ce07cee70c41c179a90dabb648801509f999ad94be9d8b5e138c4fc925e1bd5b98ac570fbf38aa71d72bf42f3f3eb759cfbcfe129" },
                { "ka", "def5b6e7e6b3edb83ed01b1c4d70744a44617a640c90ce38601f98841bc42d368eb497150b37b463dac90216be9d9dbc08f739ef91115aabf3ef0ace51c8435f" },
                { "kab", "f9adb53c263cda504da7284d34dc6df23494c11ace3acd20717e639634c56f9821df78561cedd5d9b78428355366c0e430d92941a7c9e3edf8f57597b2681eae" },
                { "kk", "a60c444f6a34cb1a94953eb05f6accb93bb68f5b8e6d5a4d664bac4a3414a5e26b528c0e288ae3333ce51fc91f088a36de92f65df1b4b0eff7cd7870aa70bcc0" },
                { "ko", "a18a14951dad9a98de6ff6ce2f736d2f25b2fb367b98307e3d28fe3844477d21f4c569c53f2d3409af8eb473e68ed3d9a693e8bb9a1c8c7aee6d38b335a67626" },
                { "lt", "d05526bdd3b6740b8be41815d678397e3dc8b75cbc8e35b5f0f5ad3e527c3ec183ff93e2df15e463e283beaf2fabf9148a8831aa572cf710b52c7267a658be31" },
                { "lv", "eaabf95e69e74d16fb8ce235b9f1620964871b1714123c786a2b1e6608ef49b71b1d7f69bb99feaf663d2d4c870e379174b8382c94d7816983149f0cecfc3739" },
                { "ms", "4c563ce1f3c6b8b8eedf7614689e7d6056e62f492c8e29020b9f40221ac504611f21d09551e3b80f137c70469f03491e49d85040b120e55f814d5499f752ecfb" },
                { "nb-NO", "f5c9783af7caef62252ebc40b5478fec15d58cd97236e90b5baa3fa6caf0888355c0777bfbb8dd4d9bdb2849b666422e7949e6cf936d4eb99cfa51d63ceb6c3a" },
                { "nl", "5dcb071c97f9d9bd677eec85fff91b24e864f10a55b81222eb6c99fa927ccc01549d327c5d967a1b9efd0b41043aa7aafc7ae3a60d503b2a847d1124556534a2" },
                { "nn-NO", "802842b047ed8fddb2a4a6dd927211eb1f0a290ed37dde3dac776de7fecd4b179751362f372d710fec9cd7b81fa52719994a81ef527f13d76867240ef8d95d0e" },
                { "pa-IN", "f46d02409df40f8263011439154aa5e77a27012efacffdcb2dde1686073601f462ecd2fbfb697d9094fbf55a01818e864d91c553fccc3e604a7c02b133a8a5e3" },
                { "pl", "11229089b0a80c9bc74215fdd8f385d98744b5a996d036956507a14197eccc265ed6123c696ce89f53f48372033afd7e942197c9d4c62e9dad734d5cb8af2cb3" },
                { "pt-BR", "b9321c7110c1412a4d0fa19c568a2e1fdd84488a70e25afdc0d6768c58339537f4d20976e999c3a2be3b7aa8c307eaad78010528ed869492a6a6f43e632752c3" },
                { "pt-PT", "cbf5300a22e54cad1cd275cdffff7196755e3a5233350d46c089e94acf977c9fdcf49b0759a43c0e4fa865f458f4615d1cca8744094f37d27c502d99c4bbd4f4" },
                { "rm", "4a23d6d19f905a31ed2c45662c12fbfb24cbf46b989f2b387633d2355eb5c00a83f93477e9fdfb81b3e35a7c557007fa055dff48a313ef0b51e4bb2722be8adc" },
                { "ro", "979f42c0807452c102c7256bb876c441da7e9890a57e7e47ad46345ae78e604c2a8572f0ceab14ec287d2831e8077194a90645eb8aadc9ce0cbe964a5da967d4" },
                { "ru", "c38cfdb6a6ea7e4100e46951586a16d32f62a587f6846cc066bf0bbe5f22b1f9bae825c9d730ace23a74bf29b13f0ef968039a6e352e44216d0d247c9c2dd17a" },
                { "sk", "ef8f2a47d2e6ac841a3ce7b35321c3697ec5ed962fe3bea354edcdef6c1540b4afb4e88ef5f70f16fd65598436561074775b207ba94311fe1c54e627692ffd87" },
                { "sl", "78d360218468409619b1b2dafa9064038704212281f062d961dd1a33025b0e265dd61a014db454cc7dc4892833eefeab84f4c7d2c4f05d837aa48bc1b1e308c8" },
                { "sq", "db8c6dd350f3fa379dbc8fd7bc3d4b3c298de7eaa24ef98cc0042b7b779954b204c3cd46739d140f244bcf43f0613a3e48e5fb94c3672f5137db39d9856506d2" },
                { "sr", "f5eeaaac4bf87f3fb0d6f4899b6d522283397c1ee408d5113d2aacda769d7b1c1c344beb87be5037ac8c47f05f4dfca260334d510bc246dc827c8f23669531ab" },
                { "sv-SE", "acfe1c2e91db997bd2233f770e59a8dff3fdd85c39e5bc4fba0de5da9d562e146c4961364b25afdef6f598265c54eab36f76be53d1d116fc1f979d2047aa6081" },
                { "th", "427e23e91d72e9c91573cf073ff4f59f4752af173d9eb5f6bba3ad3b3245c46d340e24304f97579676af31aabb7fa13e067073ff118453609a03ee7eb7d3c4df" },
                { "tr", "c068aa2e0eef94b288070c3433d39d54b07e80cf837696ded40c8d86f8bef99c4b267eeda87a58ba8b33c19c3e3613c1acdfdc980bf7fd0795a534a80aa4ec04" },
                { "uk", "8efb210d077beb4ce7f0271fcfc3b66d2a441f83112e0e59449a4aa3816b0d46a36c4523f1be5af35c21c7f2344ee6235d1b357bc25ff868365f137ce9f2c153" },
                { "uz", "56772d978a6b60fa6a3720d26a171030ebe90575323710527fd718d66d46d32664bf129d4b6c4736d2287e76f3e2a3b8eed1b6653c9eeca074de5b0e77e71c67" },
                { "vi", "26056f32320d6905541160c5f7d952109cb823230013856bb4abf59e9ef0044485c02751a87bd498255ddf463862d7ee47c4f8370a0355c2a4e323b5e3bdf83c" },
                { "zh-CN", "f044411248da949e36ca4413da524ded2b19bd6782c3980176422caeb150334796ea1a2eaaa66f4b7659948cdc0fe8308ff777a55c350fe0ba9117f309feea7c" },
                { "zh-TW", "4205921533395ea418a66d458a3003acab73fce7fa7fb627e534a998e2fc2c93ae206ac0e365bf67d0144e6f4e47ac9a47fe2bdcad0cbd86af840e771374e08b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.4.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "0cd11dba0ea4b2382a0e648da3a9dff1ca0fd63022d72a5b48a60f68a9547c5e95ee5e67b73cc317dff8ff200adc86fadadcc551f9a9a7e4192b03b85e95e19e" },
                { "ar", "271a63aced273f3bcdf0d22aad7a3ef16625a4128e1b8e7887fc31b5e1cb25a057f5d56c81b1c6013fdd73943584e90b2f64c501b29de6a0d89cb994044a0e9a" },
                { "ast", "f94b54acd046bda1053c99343271a968d9013ff419299986c5f13a980173e621916937dcfb45157f4b12a7c6781887db51e67ea63b56ba82b1ca02978d70d4e8" },
                { "be", "2d32645d5761c2aed9d25cd1694aa20fbe2d5520e84c1dee37a37b868be412143229322bf7302191f8fc32a2294729fd4b32d4126fd92973ab8e440ddb4ad596" },
                { "bg", "37668698089b9c85ea47a228341ff8bcb4a8048a8fdb81a3cc56bc5c43fc9b6996c63f3b47b40a1fd586bcf63dca8bfd6529c981274bd4ae2d47c02f772b8aac" },
                { "br", "d4371b1e4227d10daa8abfcd2097bab239c1a9fc97a66037c715dc609e51ff4442febdd0c6379ba0b261e54d6cb72d7364c4923da18e00247d33be7445651411" },
                { "ca", "2d3541bbcadec1096312df52d0c449cc4f35acc550f35e82d9d4ef33ba1459aa7b4e86c3fa0c6612dcc63318d4725c764afaa883a07d47e9b908738e518c5007" },
                { "cak", "b8886819c4c38c3e600c984e13a4ad8e3cb8a0936fa1cd4d77aff247e3ecb24d308224fa0c27b15d8bbd5b33fcc9e0f13a2623fb6b39766563b4344ac25ef062" },
                { "cs", "1fe9d3ba289bba3c83e7cc19d67f175c56c15686b0635d5b18c9710f0996d81a8882d478a2df0acc7005c0f2b262e2011ec4395b22a65272e35a68641eeb41b0" },
                { "cy", "6c236d86f3ab2748472f8f1d083ddd925b56539d2a27f3758bbf467aa9ea77995ad8f3ee681be4847f20f209d892216691fb240321ea6bc5afb048cbc6b9ac68" },
                { "da", "8d8d9b00312a10b4d7696bd4aa4cb215e5b6af6225249d4102201ac40b70a3e310c57168d855eed7506b4a9c31053f037fc8cfbe827770169157bf84128cc530" },
                { "de", "a53be59d31a0b1e7c9ef27362455ea1dcaaeb45ff65a215ccce0e7e90c1067a22631636739ae0b2b59cfc6c4e955e14e215a8ab3e205d5bef6af59edaffc43cd" },
                { "dsb", "755e121e7697aa8ead897d72240aeac2885f25ab55ef5e73e659522909b472b1957f46d6db4c90729a0c747f2e948906231e4d28e854bf064154a1a12cb1e2d6" },
                { "el", "ef6b4bb45e35ffa976a05a2e2f302265746e8524d152ec071954dbc30f29ba21fbf31257dd511b3c8dc3333c7eabde4a1ea0410837e1c629ec3e47951c4baf6f" },
                { "en-CA", "494431a8ce0758f7bdfe2bf17193a0f16eba2d30ea7977deb3e9bb850759589a56ea14a50e477f1b1bf16b07b1e320042b01f2341405a7c35e4fe1419eec672a" },
                { "en-GB", "d4aa221d1bcdd2e66825ba12635bbcf0d949529fde88657ac208e2a3776fe3c4b122cf1bb667d7c562769e165191cb346441e760b1f8cdc006c2630756468c9a" },
                { "en-US", "f97809220ed4bad0385c37938f6b3bd8c8a12f98149bdcba858ec41b78499bd1dc0126ebedd881649a09d940057fceb1ff1717ea13695f891a5a7565187ec94b" },
                { "es-AR", "e2a94029fd2c0daf29b8566c15d650a2d8956f7c6df2916cbcd6d2376e4c4052763a0b39ff840e3d65e36c41fcf6ad176f2819bba33b3a2d4e8f157ea94349b3" },
                { "es-ES", "1ce287be7034e845a823960b6b56e90fc085b59243ecd17601546250e85b160b7cdb602b83f3ce24dd6e87023cf34cb0ec1ad83f453552db47a8575de957c9f2" },
                { "es-MX", "b57bb33c1c219b9e55a56f3dad392f8e0dd86ec9b4e40ed48bc02b6d02a2cf2fd9823f11858e191902de6da0fa8488aa341545f2d36f3559e914c945c47479de" },
                { "et", "5460058ebef9962d3d40bab6e25acd1014011c88543d5f3b038f2816901f55f8f276270490af59d027813c7852d7b844156a104ce083092ef0dbcb2f722c65eb" },
                { "eu", "2b95a2232465f485bc65a04fd153c57a2b4bba166c14c51359f377331d6d7b8d34383d1597774321b0eb7a286902c805d7e755e501ea6df4e09668f003a36e3c" },
                { "fi", "ecbad9e26d114f8b0121947c936f8c6489247aab0ac9e6fa348114c567ceaaa12223df0324e100c355bee7a7e596a41e645bc9bfeb5c051fbaff9c3d3f05aef6" },
                { "fr", "fdd359817cad778d97733e70b3ebc6f8ae9fdd68d8268f6d6db6949bf274ec6a8b842303207468e4e06877ed2614d8169c817a111fbabc609972e12c8a57d5b1" },
                { "fy-NL", "4f83bc3205dcdb1eda255953682264e772eca660142a8aacbd33d3cd05bb7c8e07122257f2e6a2401d739ec436f32ce98689b292e140f5baf910b9e2a33707ea" },
                { "ga-IE", "de4fac321c2f11db23e8af8cba2294c7fc1c49755ff44c4375ba1a54e4ebe027d6bb5aea0616850b5c67e6b7abfe381c89ca56f550dba5a90a3914f53c92353e" },
                { "gd", "1be0489e23c265deee2f521878041f98b6be2d269d7ba80464b805bac16c439557d7fb5354a4b35f75aaa27b282ce19adb20305a633edff3f189e8355153693c" },
                { "gl", "9699e460efe5d4d3852e753e26d175e5b751ee891d0cdd252171bad7cfbfb032e302c3f10213752df531641e07c31919824a4360d0326d0f1b4471165f4d5197" },
                { "he", "ccfd195108ecdee6559281fe7d4e91829b757bc63a868c5c1fb8f273564583d3764a1affc0813036ab58e745e48947e5fb36d014ac53bfef59bddd75afee9b65" },
                { "hr", "b08b28b11f1a4afe3f22a0946b150da31a2ec323dc6a969ce7891b35035cd5b09e6a512fd3c2ce98d65b8e26d8dc48882ce15eae3f7d8dc4de3f4992d31710b5" },
                { "hsb", "7da1b0c895d9e792aeb961e435458c418ccc2cac03bb4baa522e1118e0e50768477d9e4b7c48d335d64ba685d6f1b5c804ae5006ecf5e74bc1636d66dc08787d" },
                { "hu", "9ed931b0615e79ae2f35031eebe6b2b84a764bf628fbfdb18f4f52ec3a09fb4c0e2fa813fa9e30c288156fb7848f6d32f831d0a884068add39864ee5c55f8524" },
                { "hy-AM", "c1b97a6f337ddc0db5fb8bf967bfe11372c8cc12a72e01468b28752fcaf5ebed7779ea12e43ee1655957b2f5aad9c6fe610bd127445d136af2a1bab1b01a8766" },
                { "id", "910b293715cb92d08bc586c9731f3c474d263433597d11fe1f0ffcda39cc79e86a174e2d27ba3817f768968d8c2da97a76b22e2d8d104614335ee9404b14e46c" },
                { "is", "b5a5ad7cf499d4a0ef7fa5aea030d6fcf6c467f75f3db21316167b3797218958860587ce1c1b5e2b8b1ec0c83d3201217e423e0a808a2c7e261c37fe7866c414" },
                { "it", "64045518644c0782e1e33b0d5e3300694303702bd30b802f35b45733c6074bf32dafa4646975742242942ce0cb8684244b60daf1c8d4f49661c12a20b1a363da" },
                { "ja", "31b2ca513bbf9f913ddc431ae1584acc2986c31492c8094feee9dc795937c434b9205a74d08b3b7c6846581ce55bb0a94f3e8e5307a59b78b6eb92af7fc02ba0" },
                { "ka", "d195b7817e85c8c693c24663640ea9ed171d638c72a96043ee7646a6569d22bff9b9cb2a4733c39122c02af30c39d4fe4295a44ac67c5d5b69213abea1af975d" },
                { "kab", "222b74d19fb4112abde92d8a871c62b2145b4f1c239e6b43442bdfc1792f4c10316ac5801d2a84ce0f9ea59c98c5f97d2cfa59899cf7c262a8408c4a531aa00d" },
                { "kk", "33c296b05545a05bba16938c7fce55a1f86642c3d9e8fa15005c9cdd617b4f58a85afb7b40bb58edea7327cf38be55a182bf8ec0201ea189f5ae8f6ff95b0775" },
                { "ko", "7fbbc8973b8b00d77bbf4e1e796d6a4be3710b9f2ebdc6d3f601b7936992ef021e84ac537e178970952d4c68e796550db873026ee48cfc9132a494cda4dc0604" },
                { "lt", "1fe8f0117bde11fe82fe3c0eb73dd96b5c41363fcac23c0f6e4e45ad0fbb3c4d28a227c99370cafcf3be38c12bacb31490353268fca1961fb2d588d741b37d3d" },
                { "lv", "cba9e8ddae354b5e7a17344dd4bcdcfa1a468237cf76431f81c5f41897ce546bca32713a2893607d9c0f3ba7fbe195673d2a11510cb9e9e41b42d1ab31ec011a" },
                { "ms", "88e5f659a3fc475626ccd245aaabdd6c6d930d55678e3ec10e97f0372b26de699295dbd7685e9303a1d166e621f752e85ea32bf42bfc853dad369d695688d547" },
                { "nb-NO", "b9604ed3aade4cec9ed4322fca436af386ac1e9ad33cdd38cce8b2b69844e76d8d912fcaad24c83f5b40676c0efa084fe6e03801ec4eeb7c640676d8bdf9fed9" },
                { "nl", "86f5bb875cde0a0051731ad9e8216a3b5e399ce95794bb0469217a230250f34dacf6f54753c4af67fe2e69d6197f0112f41a311007804195a460ae95be68ed1d" },
                { "nn-NO", "fb50fe7b97a9b71c9e86bec1666a4e041cbba6596bf703964380db4c50d9d0f98fc2df5aa7182c6ea156cfd45e3258205a6b8b8d0ccbdfed731823206e1bd5c9" },
                { "pa-IN", "0bd9730b0f33471da47f538172ce3adddb009fdb72a396cd38d6abda00b5a32304ddf329a208e7d60d010a3e4355c5addee9c17b3958569f34de512554847239" },
                { "pl", "bb35992aae9b2ca9352d1479faebfb691ce113275f76ab45ef82f4c90892a11c020aacf0109b091cf14f8a3cbd040523bd2e1cbce5e57e578d0b55c0b8c2f4e7" },
                { "pt-BR", "ef98b225d46c29c91c2809b708363f3d1e17ae782170db8e7b70ff98c23096a42033f89688d195345c2a15059279cbb889258137e62cc814a90f2d8a0f29f56d" },
                { "pt-PT", "60540528e96bdf3d50f91810df79dd36d97d0875c2ede600887d10cb449e52fc91a2404967e3b309e41d93d6d3908b604afef0cd0998a36f71a2a325122dc5ac" },
                { "rm", "a8e5ca5914248a4abeb646198df891731c6beb6afe3a2321eb5954030472ff09c948f05a4aa60c83cbb73a14d8ed1f94cbc0f69b2ae4fe81057095d994351600" },
                { "ro", "a2becc0690ec3808d42747cf97717b968478c0af1e92a1b69ab19c834e071cccd6421caf2c02360653f474615806851efb7b557ffb549d2a8a865e857200a2ba" },
                { "ru", "7bd11dfeae5e377a94a08dcc6684969045103690ad6eeafecc47314516d0586ac8115a83ac4f0769c3c81e9a500f7ab1a537c657b279df5dc202663accb48340" },
                { "sk", "bed8c0c6320e3836142c225ba26c8f7ff0985c941de4199e4128141a963c39289c59c1c8ee2db4568b0f3b7f7864bb10089498db96d5a8b252c0bcbc295d43f4" },
                { "sl", "e5283a4eb15f1d7704e4b2c7c7185a69a616dfb69dc81f58b4ba5ec7cb33b0c18b4639c025c13e3793b88b523e907dc9f37940353bcc366837f0d23c9fadbb1e" },
                { "sq", "cd1e8dd9d1acd90ea5380a5d86bbe097711efa526eaed7772c3bcd369398a81654fe16f5fb478db667adf889b3892637ec75196211a19774cc47d5056823d79c" },
                { "sr", "fbd8f9ed7d1edca27c89f28ddd8d64dbcdbcea9155808e815962c1d7f120aa9e8b575a25ce21fdff3a4572e1608c7272eb6099cc98f647cf53af8f432d18fe92" },
                { "sv-SE", "14c406bf65c7be2c104f9c8406782779156aede12b0a85fade8e29fc2d473392508c3c6609f05465df5065ba329b987f0743f52fee7e9cdc7ece684f4010e1c9" },
                { "th", "c762f379f945d53c68af54e58bcfd0c5eebd3c8de3c38386b49a5bed538ba283bc98e0c490127aadd8527d3378dabd16f293de0d5c21171c2f4bc3580438dd68" },
                { "tr", "715ec181a2989d581eee07afaf136670b803187f96ef93b64ad6d37e4cf8cfb2e3b6e2870d86c59ed97d415c2246f36e7a8d95c4024162c5f7b2beb305ccbb18" },
                { "uk", "fd63745ad775fe3a6bc77e730c6870195d6f2612b57e885d859e8abe111734b14d302e1e36211529b2a48d9173a6935f9a4ced9226c51579df6957c929c40c9b" },
                { "uz", "fb505ab254380d58a8f45f73ad3c06b6f2b4cc4ad5ca2ee75cc68573ca45894e31d0a366674eda290c080af34444e7116ca6d43a72a8c0b8db5180a2b834245d" },
                { "vi", "50b95ebce980cd12789891b9e0f5a07977916589bef1ab20fb562780661c697ef74499b2973b2020a28b556be7f1d56f46ad11b53ad81c6e1eea5b74b9e86db5" },
                { "zh-CN", "dfc007e089e5af34a9d5a46112e34c92caeed698b9a78868121c2302026cd13112d790686b1c288ae7d5ffc1132d9dfddfae24ef4a10b7a81d35f4087f000437" },
                { "zh-TW", "00e43b34393cb19997395951d8dc327a6940a404096ed43438a5fae48f117c264911266c44419b3253d4fbbbdee9db5e690ef4756c309abcc0301f068faadc54" }
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
            const string version = "115.4.1";
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
