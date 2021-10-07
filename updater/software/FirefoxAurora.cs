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
using System.Linq;
using System.Net;
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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "94.0b2";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/94.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "90a828da76fb79fd2986d83df9b3c0e37a7abc5af79b1f423c6dfc0823e77995f78111c1e680af35e61d6cffbfc06d6a39e36bfb22d6a602259dec46b7b24a9f" },
                { "af", "67762c972745743b3877656117239158efd42da002c01a852003b3a90b4661c9d505c3aa8a509bd38089e38a4186cbda39ac2f0ac0962cd1887756599a344525" },
                { "an", "8319ffba93537fa2641962cc15498bdd97a83dfc09ff5f82b62f0fc5e146621eda84290bb2f233547c6aef3674a1a4f40b017938d6ae601b6f32404418755f6d" },
                { "ar", "c90f88c616f14e6fc2dc72cdcc48a9b4f46e455b886dd75703521a013991741417a0e0bcdec97c03faeae192aa72ed3f180143c9da107fb8b351fff734fff665" },
                { "ast", "a37ba458ca95653505fa6e92ba708de6b578f35a4dfcb908768ad80333139701322d4d20bd231db95891e7ad07166999bc3193247ca2dcf5f3790266d44f2cd1" },
                { "az", "c7afdffa2340831b7e3f8616e279c6149a955292cdf65649c4240bb49f6d30801b11934884593448b77f08e3d9d0e88e32eca007669c177afc683135824f57f1" },
                { "be", "27f7fe61b666f0cfa61c1f9235f888ee70a948a77b09edf2066052502a7278c67ceee21b91ca9fa617f03c3872784dc0fa5b883355ff41f46b92a563029d3b7a" },
                { "bg", "59145c85ef42c426ad65b8f89f092a27455b7d5b4d45f7ab8d39e4cceb0cfb0cf274e834055877ad8106aef86f929790bbe5216fd9cda93e5e64fc94ecaf7cab" },
                { "bn", "2965cc64bca142ba1d4127b50f040becb8329615007ea9428c3836cffe7e8e946b5423fa63e731546abf33c1f5512fd0407c27909d46d1bdf676139d445aa270" },
                { "br", "3a987de735d6a0e7b4481de62607c7575b7dbec1a9803da4e0be2e31a21f0c44ffd7fada2b9785aca0b72ca17d17791934ecf1e388b8dd8c92fd43d48d5502cf" },
                { "bs", "def090e5dff45b171db8f1e804ec72cf5e3a5b0af86590673fd761190b61f4bf9cb1f86b4528dc54b83f4207b444a40aa60b3e09668bd8fa4f94fba431f86b58" },
                { "ca", "7e891720b8f1da94041774618b121c567220bee4b7156665e557f3a5fb3f8d828384c819a625c0ea2d3ef3515f5d0a97a49c1bebc2fc8ae4058adac8bdb8996e" },
                { "cak", "9f1613ca6756e48534aaed944dd34d4dd9cd1cb73ae9e8797b01b93df0e68e4c01d23f29c931142454d554ec07f5f0c329a18b00971d87ab582e89f779e90ca5" },
                { "cs", "5caac3b86c4b56e6c7afec067766e7171252af904310f83c8ab39c388faeb7c96eef163e3f8bf2a7529e4ad546423cdf11a6e8f99ed434b70567b693607ddeea" },
                { "cy", "509315c354170b2f21cbf84244dbd633d21f6ed6b4bb85a8376293b2128a676c6f3409f1ea085e3b9458e68b97e2c907c04828edb3eac2324f11e57de9016a3b" },
                { "da", "984cd107589a57aa68f757ca72bb61d29d742fb5c31ffb67245d6a3410d4f10f1ba5685dd6670144d8a79a213b14cac0f00731483ccf263878e64b796b59ac1a" },
                { "de", "01ecd62f754b97648b0dd48872d1cf57cd03dab941bebf53b2aabbe508e6a261a0c579f5f06fdb96874077042e99d8aa61e4736fe3c48cb504b42c0b3724fe6b" },
                { "dsb", "408aa473bca67059a0b2e3a2362d56c3aff2de65c43dec55ae71372b8b6b18cb86333eae85df240968fec7506c96265910fc2dd757ea9b4513ad7eb7a569a46e" },
                { "el", "afcd17068d44f42b1c5d4a9c838ac03c9a37c14034b30395031708b5270d00d6438985f947004cc9fb82a6fed854b46ebceb254bb943ce8699aa5b8318ad0e8a" },
                { "en-CA", "25019fb976f45d1c853949da8b98744a751508fbe6fe61e64c6d21a87a167e030e4e15f93951650fc16b658ede09dbff7a50cd355bc69c9bb038de71dd256a0f" },
                { "en-GB", "edff0ea7f4ef7aca6819414b473fe74727b5913ceee0716978ab96145a974a202e68e6cfe608482e2b23d071d9221b38aeabc717fc183879dc2326be78738cd7" },
                { "en-US", "b9e007611711bcd777c170b51be8931cefbde92993f5cf4c2790c5c22bedc98d22b427e2af2bd5a28c2a95b5dde0df5c20e3bc533bde6bd14ef456d47d9dcfa0" },
                { "eo", "f3722718611cf2c0c6ab3c2642848c1daceebe0bbc4fb04e47cd0ac5aa5aafe633b02a6a24d1dfad8336a12ac160132a7cadde6ea0816e4f8d665ac5e1da0caa" },
                { "es-AR", "7fdd1217ab7cc9f83404f69990ec39d6e7f5973d8a01621eb62abfa9d4fb7757b6424e093e80f260e32210551756d6245ece517d19d43ebbe9c58b69b0ad009a" },
                { "es-CL", "572bc13076733ef4469926eb53c497f35695ec4acebeb9009ea931eea084ae59275e63e188d1cf0caf7cc7a7fee4b9785956531c5b66efded94044c40a70c45e" },
                { "es-ES", "3c08ce6f9524382e1eda4d08a7e27acea01ef33e15f112fe20338555bb6d52ae67e7523554009b2d0bb12122e3f8eb0eb27947b60772df2869bc46542c511e27" },
                { "es-MX", "e1446ae235952beb5b602b4ed551e970ae0f11c23a11cc273a2b182b53a828fe1d2079bd0353cccf965a355619b7a5e1e8b505d181182d28892c2cd26f529c22" },
                { "et", "7f8df265058bcf6293f6c8db4757eb6b85dad8feadbee2b41f6f16671b5f02be3ec4820feee2d32e71f2288e593e4783bd7f0105d74d2229c6ee1221a1b309ef" },
                { "eu", "cd862b1bc7d2af110e935cd48761695b6e52b170091c4a9aa2bb480d9fc93c47f7bd15a3b4c0af3df9b6f7486eecc9a74044bc34fa695052567647702a6d509a" },
                { "fa", "d4bca8dafb08432ec2d57f1b8f7abcfdc27993955b78188cbe28d6250dc73b4ced3af489c39081f63907133f9a3ffe54b2fc50079f072697bf2c418ba2015ed3" },
                { "ff", "01e050768541b9b22dad9075e10cdd945c53706bb30b43117eca7a6e1ea6c975b173b0303a5c52e46e941b6a0c3c35224346a9d52fb7f3fc81f02a0072b9b5e7" },
                { "fi", "cec966310c006cd7364078a83bf14ae15bd52a0ea5e6c61eec4780be60edb11288ff45d2a7f18a34cbdc6c98bd3f3450b65a24b6e65591182e974e6fd93108de" },
                { "fr", "08daffffa31d794b6ad4b318cca7d002c8eeadcc4c5b0e84f83389d9ce1dc0a88c3f30b2dcbd622197e6da2ecffc8d5eb88cf6c57a5ba074742230af4c79f440" },
                { "fy-NL", "b58713fad67ec6e43241a0c63403cb663f4cedeedfbb57d89cabc975936e410b43b31de96ee2d1499af86777af1d34a164f54693853dd773eb15b70acfc917d5" },
                { "ga-IE", "63a5ea6e59f9cf2a4e4550715cb17c4bc3ff3ce7a85f32b6180c8979410ead154737dcd172c8f34bdbc207a024b1f2fc56f3270b613706c651314b26e3f6359f" },
                { "gd", "fe2da45b84c62a5f6ad6d1db669014a3d5049760815f57887e5a3f76b393a5afdd7b970018a40c6e6195c9aa64d009903a63483329c100c564475de627055372" },
                { "gl", "704330d85ca36be45d328292e54d4a712df138c74d5dccc91275ad7012198a1fadef1723000cdaa96e75fa0b199e9c3c35a55caa5da0137e56cb9d9a73beef36" },
                { "gn", "f16c8b1aaed59b26896c0852703bb1643b35cd18eae0f953d97fa0a44fe6f629a740600a8b8cdd2c251719189d9e8b9a8f3b543813ceaf3d8c4372a596dfd814" },
                { "gu-IN", "3f1d885f7dff766a05a235ec96247b9ea6401c579f4ab5e943d87df8145fd206ffd2d2769877e3f91dafca3ee7cf8bcdbd7fb04a68ec351441396a7d59c152b7" },
                { "he", "9745cf7497b7ce9fdc924153f1e5bf1ec27117cee1fcb5fa90b8885ed2f932cda23be9d9bf0502382f6f457841b2eb37f57d08254c2ff01af424c2d6ade94990" },
                { "hi-IN", "2b4bf9272b9eabee4c75315f252752183330eb5b69c17260c5edad19767504c90c79326bb65d5315aa9336ec403aabfc844e6357f2bdcfee3600ab2ea5973a9c" },
                { "hr", "000c21f475b56d874a8e8f3c7d3da01afd535741a40fbfd62c18c1c4c14368e32e3ba01ee4b6e51bab1356750419f2f6bba8bbacd9a35c6c24008fcfe0bdb088" },
                { "hsb", "3dc62d2b0cabd06f20ac8ee8f7d0033fdde870a3feebd312d774ad1fe4792558c6747726e68c4de7e560850ade45c7e9eaef425417bda78b9c3f0c8cb61a810f" },
                { "hu", "47a5fd7c67bb6bd18f4fda543f250f3a926e4c1202347dc787f8c8609193fca3c8cd0e5752f3f301529851124521467e7d66351999dd6c8fc145ddc8ab58e8ba" },
                { "hy-AM", "d562a278f62b6f4a1db379b573046dbe72e952e9f93a69917bd293cf29f22289917a21726c09a923534628b406dcdc38b1284e401779b457aba530274db7b31d" },
                { "ia", "69b9d53d05be94a854dc1542c059b337f2b009144aec2b272f0c4a538a1b5b08a506220643daf84a7fe698c8dd335b496299bffb5139e7696e3b808166fdd2d2" },
                { "id", "73087e252186b1908138d356aec09174a25e1d7b3f7ad6258ff96cccd40eaec688370579ef2bea797886d6603120694a9f1580793ea2e123c07d30de830b1cf4" },
                { "is", "1facbf1b82aa1dd00254d132fbec50b1a5a6a27df8975a42e2b0ccbb64ce207b59a472fba8865204e8ce347f91db129e2f0a876fbc9e286757a7c80e0e2f07dc" },
                { "it", "bc8d9c987aecf63d79c1b0730905b535c551d6c6db602f58c2d9ba757f5210abaec320044c9799ae2dc05c5712472f0be7654900f7322ed3d15dd30b42909ed8" },
                { "ja", "9302203aaaf67690366be87dffbb28fdc46b311f46f0ba22c98ade09fe739631d11e46bad35d2ef6a4255f6292d4d2a1de35c649419dc8f669696b4f9b07f8a0" },
                { "ka", "2719edb3d8a6a75f0929edae18dabf881044471809bd643ced7924c99bb705ec95863112d090ce4e3cb47a9eea0a753c9702ae56657863518081f4b472ebecef" },
                { "kab", "0c4d08a86fcd03e43b17d84a7ad320e85624b83103dc6bac2ec1aa18ae90c12767cc544f15ea3d5e61a535f242f7889200db767fcfd73ffcada036b5a499fa12" },
                { "kk", "2b8381e125ff82c29aa1017cfd5b05f568c6d258c950b1e6ec004a0d896d90d6ac4ecf9175d0a6a27d679f8947d55634c9864ad5d7267c0d4a30ac44045d92b1" },
                { "km", "e5ae46c50b0148a0ab8999b4b221cca3c9b6d48c467740c3cf1e545effc3ad12b389c38d930b2418bf01e32ea3206b4ac69fe0133fe3f7bc730e56f74b5f19d2" },
                { "kn", "8a0dab8114ad5676633ecc999f795b029848f04fe30427ef704a75f6520fec84f9ae98903aaa52977d4cd0a361a6a3bb04a017eeb1e7a6d95f25bc677893b4d6" },
                { "ko", "70fdddb8878ee527c138e0fe13fe7b9699f6a00015015dc2e0b0e4f9e511986eec555f4c2ad2a9de4ce4aae9f87c15f29c7a3f48a04f639fa63fc0863c47ab57" },
                { "lij", "de026e5c96d6e3eb78fa74afda038528586a106321b925b7a54f024dfec34e58b2f3a38265d33239b94c2a262740a36016e6950e620fba3cad1e84705e91493c" },
                { "lt", "e281a906d6cc14042f655145d38455f90d7e791a440d80c548004ff7248c3a6250f150f7197077f773cff0f3de7b725c931a642f7e1696b1070c566107b017b2" },
                { "lv", "04139c425118480f1804922047eb5d22a73c2058afa379d35106501d88f9d28a3ece152d51cf646160bff454e62cb8b82d7f0cd70c2a440787d20bf62cb12aee" },
                { "mk", "60783874801be7b68b91f1dc1c1616b464e07cff78e72e4be8c15fe4d58d7cc8bad5e6974b1c48f99b5a6dc4f30a654d3fda7ba801b62f6be577c335119980c7" },
                { "mr", "f392fa5fa1039c9d96033ca3c7ef2a49a36b64ad3d5cfc4ef5bb3fb668641a896504f629744dcda215cf728f41e95e3a5e78c8b78ced0d2a223dac3c16078322" },
                { "ms", "8da4d09bb1343efd3fa7e7f0aa19d30335ed491ab10bf9507d0d23072bc36b3eac8acef392416353d50cf627cd516aa3a313a2dc56fb71e88fad22d0f7b1af66" },
                { "my", "025fad0031fc09b8e46a595d0ad297f17d6f491a3e9b01b8dee8239a81d273da3aba9ddb1ebcf7077b6a1b4e1af12ac639160eae0eee53b93db535f3e714436e" },
                { "nb-NO", "8636282d9d49b0098675d34c25e7c9df8a8c2d4513c8bd962344b6e65f34693e431c509b7e1d5906ac447f0c81a84537e13d027d20dcdf622d7fadd983f2ad5f" },
                { "ne-NP", "ccd7aeb9a76ddeaddd649cad4e0d09f47f9e4e674e4e314be1ca68b60ab3bc11cc553a9f635f5736602285a310bbb2ee6a046bf7169343f19238cf93c7ad9622" },
                { "nl", "1dd93513583205099717937bb46ed2ccb5bd3f2d9b5eba673ba57c00250bdef3634ff261fe0210cd77b73dbf05e47c3a2ff0977bc46b7009a59749486e8efcbf" },
                { "nn-NO", "73540c1a4f79ec8eb64e2cf3208457ca2aa83bf5e0f698c9183210271b837dd78f5f9f3fcaa212d4a24ba010419297ff32a5ed14e5ca8299c3b5a6aaf8523a0e" },
                { "oc", "c956e83e87dc0b4671b3e2742e2bef2c3b8f2cb813a6c5122a89f5b4411ca494119c8e7bbad6cf0d9b3369a2d5d1636148d5a4a80c1f07c2e4426f798106359e" },
                { "pa-IN", "e9c3c056a26b45117a78d012ff3ad7a128e13a36c7a03a6f656c641277d1b77b713edc2e01c28b0bae8b09a3d11884dfc272ce2d2b850ef82b0a5b5761f775d4" },
                { "pl", "7183e5f980ab7187f89c17c87387fbe0f2add1b21607030ecc697f3c3d3663ae84424ca9913ccd03c361fdbcb7de976a7e1f704c58bd8febb55f0632f823ba26" },
                { "pt-BR", "ef069bf012b5355cccfd2d46eedfccc4210fc846e72aeb090f83f7393b089413279b06efd391cd8cc4eba74ce5e9b8e2a876d2dc6086826be95844251b1019ef" },
                { "pt-PT", "51df717a71e0c6de3b878402d1a932240cfa2f4561deb2c63e082d419c368a6e450167acd34e6e67e0c9693555e957fa0aab2e9313a378c0c8cba47ebbe4d28c" },
                { "rm", "ead57d01544fd7b70184af107ce5edf3cdb86dee3c78bb4cd3985786d0273437cfc6fb06f877f3574f4cd358d9edbdf5c228af6864d4638db30957d96b2ebd49" },
                { "ro", "4a9500fa7f2ab7fa32bf48af28dc40d44206f51f05fb54104689b89391b03cc5f2d55eae3ea7c9f1120481126e1f57a714c573b3d410772c36e26ec9736faeaf" },
                { "ru", "95b4c945eabb8f52aa6bd0957ec5faf0cf79f6f4e9989312bcf3bc76231a6daa4f35e4c98ac625851b97302e7122b3ec92bd6c6d45dea21ced23fbde45324c12" },
                { "sco", "2da5087ac24237458f291d6ce2e1238d9f59091cb459ad4c874a73563ee081566bdd288877e9b59042402de344d16c2b7518aec650291412c4ff8444c6f09461" },
                { "si", "9a2e12f69e48b32fd6bf09e9a4a200e9b4f5e9e9271eab091013dd4798adae049e3f9b9991cf94a3609a04e2acc227249afc7299fcc817d53d08718c7e3662e6" },
                { "sk", "ba36b010ffa2e29022801026e799fbe9292947fc61e36e3c06a16798232994ebc1c7cb53332b3cb485b763ed9babe6c5b679d580df91dde74ee28f808154e9c0" },
                { "sl", "6aa49e5178fd9bde7648759ef5470d2316689b713bad05354e6240c0069300b52bf350321e59b64235ee9437505aafa96fccc40638ec7710c755838c433dae90" },
                { "son", "5ab1ef8869906826fbe571c4fc3fa76841b08e9c119dd21a2f3535e4165bb316074120771c8950fa3cb53617966cea91857f3f09eb603c4ef7accb42d2aea5ff" },
                { "sq", "782855c68feeac18ccb2e266073bd5b8e37628ecbe3d8d5042fbbb98e48424e3565525a5ed036a73a4d2ed03e3a4c32fc4d396f13e2d05af987668a328dde95d" },
                { "sr", "ad5a9d2d82cf62fe156a7f85662dcc44c3105dce66434396dd922e2472dcacc9388b6b4f802f18eee61598c41885c0ff2f310cb64e87cd0166393bf4702e90a3" },
                { "sv-SE", "647c37721f6591bb7955fbafdd1074f94a06afa79fb4211c07a670acefc8586e552b13e603110d42740267e3305000fdaaa7c4ae776f19b7f0b793db9431ac5f" },
                { "szl", "c22fb295bb337bfadd5701df0426885e70f3280f6f7a3616f4d5ccdf6403a41efc5fcfa8c09682d9e72919e6bb10e3904531aaa367c0589337dd1acdba776aab" },
                { "ta", "cdc611e01bceba18ef9bce83929028398e390b0351d64aa186c6af0618c9218ebd2ebf318116dcc1466d54545e275a5fcaf57324aba322ad594d284d974e396e" },
                { "te", "c29303962d8ca02ccf33f230a72bee43e3d6883ee4942602865de0e2d699c891d2a892c282fe0373f3eff015428c5fad4044e57988f79321f3cf24c2bf41017e" },
                { "th", "1d6cd62fb50868bab69cc138e1cfb044bd9584208a0b50c1cae6d1ef328ca2c31b83b3f4a8cccd076503c1a5ae205d8be4f33562f8477fb22b33160bc6ac5677" },
                { "tl", "24e37a7c335115a8f9ef7ae656d9ad814f5841b4051477902229acb42b5bfe7f7d7006c1fc903563484bb17da23324e5c7459344990b3df2dba2db50c27f9f7f" },
                { "tr", "74299a174ade220f5f4d1ec1d327e0d3d9fbc39c5ae92dc5f967d8071ed5d42864ffc75673ce6d704853b8c0b5174aec231d51e527c114ac0ab248a0c1115ccd" },
                { "trs", "5a6d95488bc0caf22ea822e8e21d41eb73233935369f214fa4736b08f4e49ffcd20cf438b6000d7cfcb2049f3e33c95588119035b70aebda839a7941551ef7e3" },
                { "uk", "68e5e95bee88fd22748bf57ae5003d22447091d70290f137927cecfe11b2c3c6f4655e3c6728f2fe9cebea7d16b1e30538ce2b9070f6acda04a939514050da27" },
                { "ur", "1a4785d86923869cef2a0fc50f5985e86e3b34650752cf518e4bc2709c3cdfe09d7073ec3c39934d598520a274098aa4d9fa2a23316e778136e04419609e2a34" },
                { "uz", "9146367b1f08388d76a0b6c14981e866a5df9d5ba934a100d4e7ea6307b819bcec4d3450efee8a3f1bd6ca0cdd68bf4db93637deb180bbd2d80c7867e296e8d4" },
                { "vi", "5b761a90cbf03306ef3cb93327ac932bb96be7272c21b53f1acfca9625c0ad67db93e7fe37b353dbaaba11841b71c773fbf3391527a065058c4a92186750a259" },
                { "xh", "89ae8a80ba9bf12bd09351284e54652d8b4779432e341541416d5ec407315951ae6a26f1887f7927ca583371660fd70b5be7de8c96fe1ffaaff54c0849906375" },
                { "zh-CN", "e1013baf3878c57dea4bfd5f5bea259512f4cf02f3e7955d1ceaf397cf7cbeccbc1e9d60e13d35effb5e2afcc0d86c70e7bb1ec722899e53a89d44279ad1e19d" },
                { "zh-TW", "06a16b86888f8d9ca285f654aff2d231c7a804a1a29edea036ee5d56ff395440e651ea3727d07cf19ee7824ab7c94fcb395c4e7747e0ca5f34317a1803c6384c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/94.0b2/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "633625ac5935a71cadf7c97aba6c099cf45d07058101de888acb79495a6412c19f7e859e9ad4dabd2f8149342afe9b3420c10940f2a0fddd250442911dfccd3c" },
                { "af", "578d1405efa816ec0881348b01ded428213d7600fd25deda556350b1842a856868cf2e573ec7eac1ed7e449aacba51156937f2f633ce6addb4c0800eff81b096" },
                { "an", "cca5dd7ae1471147b84fc16a5df82ec6613a5fc108cb65dc85bbd1a001809adc287800f3b93d7972689ce87db84db2971b5f2463dda6b1368ed2665eab45c677" },
                { "ar", "35fabc75eb0f87f4ee5e3c1e44b0c0261633403d4dd236740d5aa25594a03f720b3642b06a9e8df77197de8ce1294cdacb66d7584e49968f9e48af12334a90b0" },
                { "ast", "e0356825374c3df13aab96e2fbfe7823490be5f59067e73eaabfbe804de335375e1ed1e92b5e987f3539bf2b43d6deb5843a5836b1676bb3bd255830d982aad8" },
                { "az", "65ab2e0027989674b5ff63e0dc8832271d06cc84e23f2e1003e85a79fd670df634fb31c396bdab69f690cdb9fab4539edd2bada6d370364ea5e4d69fc8a7429c" },
                { "be", "069ed1bb5d6b717ce482841cc86f5cfad86a0d7916410b6bb060ce89a8cd20c54c749ad07daed61fb23e44a08d0784470bd9bcb2ee93693ec4e4af96d50fbe1d" },
                { "bg", "66fb14c30840d6b0f09be713e2a76369dbb409a8a879ea30e06678a4ecfe01d3b378aa850dba83afac5b09c0bcb278bdf1326b012f16a080e48df0f445899fd3" },
                { "bn", "462607b313ffa6b306364b7dce104580ecf1c13fc50710fcf59c9c30dbf4eae961c986b275400f2dc75f2407f274db0cd5bacf16879768a36c02546982dd0363" },
                { "br", "3cb7ceeeff1e1c590329ebb446044948c3a1717f826f57374e19bcbfdd109bac18db021b4ef0cd43c2b77e0a82be73e20c124cd9f7cf6c183e7c90e8355f606c" },
                { "bs", "d799c31c10bc26dfcc8f2104f048c3c20dd4882c098cbf62fcd00d0041b49fa135bdd017878b9d2da34da3d04b7b8f8b8fa3370c1eed4077692a421ae850adbf" },
                { "ca", "5e68e654c240b6ca7e6447367dc0517fb31e39aaf1ab5b3db156cfc4ac98d9effaf30a28c0a142c2ae9b6827304db07dc19bdada5e2a2ebe32f0ac8117f36e1e" },
                { "cak", "0e68f19c71b6943f83ece16c28037b4139dd5c8fa02f191d1b3926211bdd37254041add22fd2b4d6931968cf89c259f5c0a2ad44f194f3cebbe6c390f9150d9b" },
                { "cs", "64650995eee04b57b196fbf3ec3d2df4025e1baab714ebc12ba747cc4b93ad5c79fc16e560bf90a0894312eeb405c19ed48976caaeef49d84e20da949cb87699" },
                { "cy", "11135cb872e22b0e63918dcfdb18d421e7a6cda0728e89cfa473209510775757e733a7a42d1bfa06b083e68eaed7a86b270cce97b99413e7d5ffe7b9e9b31dc6" },
                { "da", "73c88a2fb691faad6db6b1d2326587f87dfcf85977f5790fccdd11eab20479993b33892c75b068fc40277495767b909ed64ac517f1bc4aab6237bda140d81f48" },
                { "de", "9a4f14623ae6141467949c447bc82aba592218096a96530513820d66aa5a0d05de270385675b1f3b71508313fcac4f2c3453332dfcbf0857a76655e2cfde71b3" },
                { "dsb", "e91d9a6b195bf9d2993bb44419d1d9189ed07350506c31f626f58fdfca2e7f3e8c08d31dd6d7db695320b3e2aae4d1b9661c793d1dc09edfa44ae4fc13d62eb7" },
                { "el", "4c79d3e1e28e7c64625552e47f5f9aaab829bbe76e0d8fa67ba4d039574d84950174d68405225a52993d438e394482889ca05c951d8317912d871eaa33e83c9d" },
                { "en-CA", "a422eb1b2f27d3d75e55e38949d2d55d0bf8da4368c1fc75f7e3b1db62f6d471a0c06185ca573a7cfd5f88df20830afe9915e730771099b6d7017c6d21d4d833" },
                { "en-GB", "c5589abb57ecaf737253d730f0f40115fc22c19d147fff648d381abb77551b3f08825b33a52d1c35532a744df0a12947d9578556577d1ba2c5a141191e796a26" },
                { "en-US", "8c00a576c944dd18f252ba3b2f78715eebe3ccc09d64b4b3b4a87a98367400532d70275e59c193948fa54774e3d0617d782b1ab830e6c9b04d4c48c6ebf2b02d" },
                { "eo", "35d4b6f769f698a86e6c32223f75728b10b896eb6648438b233434c8f19b801ae8f942a1d439bd39d4803c409e49ac9a9d99b39b3f1b331f85aeb7f0f86b931f" },
                { "es-AR", "391782e477aed679efb98cc5ae46cdced921d2263f9770d26b144110cb189c2a3474406c17889f7d8b8e2f6f2f5598f8422448fe14304230d49cc9cda4325017" },
                { "es-CL", "28784f33eeabbaf7c43aacea08b095357e944f255a732d97132437215288cf1e42cbf7afdcef7c533ce9f020d9d10b24dc275d855136f240cdc5d9601216cd92" },
                { "es-ES", "f09e0f6d35f1b8861ed73497a23bd7f3b57c4fd4eb4118d895ad0e2152abe8ed02e1bfaa395ea2e53d577ea056b4b26c88c03706a0dd883c80c3c9e28d86f347" },
                { "es-MX", "9d833500ed1f67b3f9ed6c5102ecf29aa562ea7b18be6b4ca71554e4796c047153641080e27b8141632b3729b2e6725e915470c638bda5879579d4ebde2d7719" },
                { "et", "cb9a91b269b4ebc8cc2e2838a3050a23bac697a18bf9b740f44807c0b8b006f50410be3b6216b5afedc4a04be2fb8dffe542e22766dd9da23ae9013254aa5f79" },
                { "eu", "5616474efc4fa2ef7c71eda2cc5887e4af7437c92a25d77c0bcfb372d6cf9c5e1d98aacaf48c39af403da14d5cdccf0e1693d2a995585371ff4e8b27bae4a68e" },
                { "fa", "b2927b0864a2d91b8319c7bcdcfddc61f5a8d2e827c8f6150435e4f0f9e6402cd06d81fbfdd3ec004719d0944cba3889a864a5bec9f76065eac35c66819ac619" },
                { "ff", "7eb646ff1c11d0ccd318a305fb1fb5bbc677187978cf584e89473a0b20ce8dfe6faf1d68a89ab24810806d5f4f6b76a9ac704841e956dbce668de9c7f8eed168" },
                { "fi", "927abb78584ec49d8c5455d4e5d7bb6822c5be6cc8e6e9272163c3eba430b581dfed8f977d1d8f6691f6e0b4479e9bfadc8f01a87b20ef31b10b27ab64296c45" },
                { "fr", "f88371656ee32f152f5a6bc5bce2f2f933cac28bb3c24b5bfd262fb6ab458457ec78efca2a33118eb05a6a3f19da8688f67bd464a0746c97ff2647cb5c8e930f" },
                { "fy-NL", "4c33584d5f3b46adfcb906ded3bc1086a413ea6adafe3240606b91315cdb746536dae1f1c8ce6f410c564fb489d4c1ac45f1b5085e6d0286214a24dd49d138ac" },
                { "ga-IE", "e355e6e040bf37108ab1918e996849d6bd50b120c2c73de8c180d9cc0642a48dd09eb64eb4118e7aadc19061362b2702238c8308bc03f463e1cc828f2909f74b" },
                { "gd", "d465df9d68b03880228e6d95d4e2ad882a04034e3367580a7b011e8cf517d598e993f27d1a5396d26df64cd1a49dac4717ae9b6749fe4809ebae5f352ec8c58d" },
                { "gl", "a2368d5086e03a2aec75ca37ebd73cf21184b6d6c8163151a923e07a09ad0044a254d2125afad06e446e96fdd05a33d2e9ccc8dd9fd8c503dda3760818ab5a76" },
                { "gn", "e3c3a93eb63e30807d853172c05d79537d842bbdc36a2fcc01b3ac6b60018afe2e4dddb3e25d0fa71205183387901df640995b5a1808bf5199c74df4c88aa868" },
                { "gu-IN", "e3bd2277f99702b719ba2af708b8a024e6c19f3d982df70e051775c3956606dd26fbfe15202135164281ea67c216854f90a47fa59a63f51258b9a59ed4e9602a" },
                { "he", "c66626f91f7c81e00bee9e8570b769105ca0cd65ed7205b0f1fbfa63fe8695ef421e2199102194efb6260de7400e7994d1a19a10ba4db00a70938a7b3f05cbe8" },
                { "hi-IN", "d6727f3d85b399236a935abbbb6a8c24bdfef4b5ea4ac916a4eb5c87a2c4d53e145d3b623eac42910cba570dfe4f69fab731ce8967ac090ef6f54ae700c7149b" },
                { "hr", "60ccae6227116734a1863f2b1fa690fcae715761172a07bab2e19da518acd4cea3e9c39c4fa35e0e0c3428b28643d01ac108715a97295990a7863f435644aa90" },
                { "hsb", "522115bfa2981ef2fca189b3602f0575fe42149a9b150e2af49ac9c693bd58a81412937bc80a3021e2524ea3efbf6cd7b97bb3e5163a0b58a64a5b9087073cf3" },
                { "hu", "8c2acce7134365ebdee93c5d0cd71f94e08f4c2cf3773d3475e1068914a63bdc933015c8aa41399f8870fec3766c3461acf27d1f4d543a8af984a15888b4cdea" },
                { "hy-AM", "e41471fa01eb781686245c5e266ad67e61746191a2017e8f8ab5244385489bd0b72046129123344d5ea8be81f0e1d2e2c065595da0ba07ab7f56070d5e9eb31e" },
                { "ia", "2311f680d59e6eb24b2096384db2d4e8e8da91ba5d167dd50ce7c3cb7555ba8d03cc38a85ea3c58a6de15e52270ce6c0575f93c1b5d21ebb376832fba164e6fb" },
                { "id", "81aa94787e2507bf6e4186bbe8f662f763b2140e8fb46a9c47212d5210b09182dd7e35f98b655494f0018c7e91f854ab2538ceebb4f3d806d32e510857621477" },
                { "is", "1e2995f55618060780ee0b4682a56ef3dcefc169020b214d0bf303390bc6ba5499021795b212ff119f5029181efe13ee616ce6b004efdedef3319e7480ed376c" },
                { "it", "5b70ae26c2f1f065d65bcaca70ba1726fad8944ad353c9db8f03b41c9ecfac2a33fb4c2b4768564d92f604cd0a4058de84d76e26d1066a3a440edc72fd4261cf" },
                { "ja", "46f01452542a4244e4775715aeadf75daec392c88f24ffc167dd683edbf3253a976b95f98ba05ad4dcace816857002eed777a2ff26b640b28ce800527feb6ede" },
                { "ka", "3e42fda56b42d1161b3c26688c2e713d53dcdc6284bb75898d4d77256545a2123a2d41155706168d7caf748552c4a6035d87350094ce149fb98d77a579d74b76" },
                { "kab", "66d36c414eef5fd44bb61656f31f5dd8f4bcebae316ab13eb5bc962ca46df701c38c15886c4320116790252d89059d25fa8477447ae3ea324b905973b7dd1c1c" },
                { "kk", "70f101d1750706c111004a2246b6c6ff2494ec8b3635c9dd69aa9d4178d1812768cdcaa1346e3e099ccac4102dbf6ed7fa0dd975b870600e8a7d2a718ee69915" },
                { "km", "8fe382a6694dd779fd1bf300c14a1acca144797d318b58f32f79a0d6703bc09547f6696c30d5de2f6c3fead0d83ba0dd8029f260efe6400aabf8ceb1ffa82791" },
                { "kn", "a0a5419af1cea0f95851b0e28f27f0677506e0bee1f74cbf2afccb6c641a7941fa0047f15b203489dba0cdeaf43b137d9f6fa0a945f22e1f3de891284b5ff3ea" },
                { "ko", "1821438618a2f4928ba2f8bebc70c602a83d8712b5004da1cf412ab5adc037f83e5c8f06a6a89facf9bd9a450934b08083d2bb313cd2b982afd8718a520d5133" },
                { "lij", "2c4d99b7f4152654c3486e67a56337f57b4d17408194889fe65c3e16442a3c93298a6f0717607c78d31135890ea8592f50c931dc5607c42e4e965ae962bee1bf" },
                { "lt", "6d4b781190d992900a57821e3dd4e8dce57aebaea044a8afe746b81bc144debd3751b6936067f8ebf52741373e58eefff660459480008c18ae0a80ebd7069ba7" },
                { "lv", "a2c46f07fd066f7147d8148d20df1f22988c7818b616a81c3ebb6993d403d86e97479a4cd9fa1f6e011f9ed3d9f4ac7473fcc2a89e096dcd0eccd07c3c1be6f2" },
                { "mk", "ecafc643287d870f958c3513d52462e415ab28a587e64ef7876555534262007b2f49a18ff669944d491765078e5edf7c9078eb21c8123b962f6ac17771bd2b95" },
                { "mr", "93ebb20be254f7ceec58eba6352a26647698a7c3d7354f3eda7106a0d3dd5bcddad46f42d931757cbe7bba4c666437a591c7099834f7be743783eb87930c42f1" },
                { "ms", "fe9f587c90aa8117d27bcd3bd6c805f04dc12208b045c650c42fdc8c8ffd69b682fcf3d28ddc2f149b8ff0de4ee7f34ce58daf7a37d5854fa4beaecf1f2aa7f1" },
                { "my", "5f8b28d6a6efc65c15fd1dfb71356f64db3a61a2cc64e91f2ab1e1ed39ede58e00ba07298e3931fd7c0c1eac3727a85ce69e10859836e0c618be6bfa79529b89" },
                { "nb-NO", "28b47c45a1daa3021e5f9d55d393d9b81d647c3cc92306e8ff1c1e012db4b13207e17b0b7edd69184302abac15be1b4a1b7eee67a8c53a8e0bd056683443cde2" },
                { "ne-NP", "76ca7a672e31c6f981ebee26a56df1f31b9e4443748725903fb4c202721dff417f052b11593e47d79058fcc5cd2c9e802f36b6de6e9386f0c822d1a6a69d8655" },
                { "nl", "dce7450e238af7e34ce9b4361d1a2fe8d76a84f13e713db4a2219a8b851749d621ee3a26cd5b91b76a85967e74f5345c295ce9e2a79d237fc8ba7f8a766a8452" },
                { "nn-NO", "c5d7b6ca03377ebb7c7d64e3906eea55271e2a742a4411f6e24fd51313e24e22173a9c9341edd831ca7ed870aac436ad6a0b43992a1ccfe32bd0ff5a749a033b" },
                { "oc", "229a27c19e52934d33bea2a8e293d7c5103480076bbeb9ebfa7340b3c1814c76fdf76ca8e42c511383c86f8e1c53539bd77dc28f167d125a5c4df46e17416205" },
                { "pa-IN", "4b5d543734a712ac31b528d8c635bacfb74a73de398d374f039e5cff568d37327a9a888d32cf6877e5d9ce33e24f539a48a74c495182ab29de1bed6191d04f46" },
                { "pl", "dd8d8743feaed1b482d72972f5b52a9ec331dd28675781baa42e6aec4bd1140dfdea9b900d0b91c6a3c69ccddf1cddc8e486805284ee8d5189cd3555c3f96f43" },
                { "pt-BR", "6d8a58de5a0f6343c6036c71e89d3a5e66aca2ce8d4ebde45114cf07668294be87641edfcef6e8c643e5d2277aed2f6eb5e3297999e6fd6f6bc2beb380c07c74" },
                { "pt-PT", "8bcc833784a88c6e898ced18ffe259cf7458b28e609f1c8149f8599c9545eb2f3f492f48e4f133bd94a33139adf7d255aea6d150b5f44799807a8e16a94f73c3" },
                { "rm", "e7f2eb5e44e20c1953fe67963346b6567649a13e6f0f1a559f25effa3281a1f7f571fca2460aa685d4858f7b2b2523f1075cc04b26098ec145266d34ca01a2f7" },
                { "ro", "8d41dc1aef2f69558c89e411360b686fee69f3f380449cee530e86048e2ef3063b76900c64338f1185c8e13d9b27581f17473aabf0b827558bbd24d5c44195ee" },
                { "ru", "115fa77496b1f3bfcdc4b814c2e9ef71f0d559a14ca7203644feb02d2a65434a78e5c379d54ca574cf675c5d1f4311c52acfda5d9c472850ec3b7ec080d9eeeb" },
                { "sco", "c0680a01a9759428c796d0c558eb9adc86589fca77b055e557909110a46576736d47728e29a3eacdeaac0037257b29593237ca287707c4212cfcfdbba1c2035c" },
                { "si", "d63ea46d5cce116c9ff35c86ca6349cffa59a9139673f953f2cf7f4c6d9dc040b06c5f4061eb73d6818c1aa3428be70e03e0c341717eb74e2c776c34794fa532" },
                { "sk", "5945abff08764627d24265bc901e1d3ba382482e7015c4fdbd10c6c053f1a270d5aa21eda8feb9818a1be4a3067930b12b08b651c7531d746757bba56ca029a7" },
                { "sl", "c883f7362aa3512794268974d9ef633ff2c3092c49d65925d5cd50454dc415472da8bc7304ea02f4c59358025dfd8fcefc06f4f984c4a5134bc7ac582eaf0ac0" },
                { "son", "df29c6e17f09f11d18b0b483d8df2254741774f5e034cda5972b12d9ae6a40e567d8aad5ea90e668a053fffdbecc2518dccb3651e30f8573fe5be315706a345b" },
                { "sq", "cf8eb0d66939a2f312515195c55905318c366decaadb5b224a00c7474ccbbe7573a4b1d9e8003bb076e7dbfd047dde4bf0924dae94c2140201b7b2e23cd8994c" },
                { "sr", "daa107b3c3618e253629e1b0dfa22f03ba0d2ea73785fb6a84a0a54e9886348858de29f3591171447b00b9b0cea9ad348580b33d8d1b18378f956e19ced7889b" },
                { "sv-SE", "4a7e6f485492166d3f23e29aec1badc8c54a8f2a723106874958b4c1211ed53cc524bccbe4d61965679c9c101b13bc6d16c7305d37f8b022884101c52e2a1fd3" },
                { "szl", "154db84e4bd39a71e357c25aefbeed064b6ddc7dd99ddb8b683067cdaca395617533b9ed42e4d4ec4e9b4be494a59c233c473a9beb7429bd706db790a51f04f9" },
                { "ta", "d80af5ab387b19905e09e4329fffbddd19794ac28cd49053001f159c85422ccc3b262557311f1bb7b37a9082abec2142aedfa26afa234d861a7c2164709b2d14" },
                { "te", "0f52533b21958799e0d5a2d50d9f4cbf6c4c3f4b99e2e8683b3c748434238f3d4405a84c138f7134fc8ed7b9f0eb4bedf8ef2ac773583275c3bb128c8f2dfb62" },
                { "th", "db009b72507bada3ed3aebca91bc544bbe25735b8e3e638667dd7b243f50afd885d6c9f2127b79af4067c6de68fd203b92aadaf27255d51ab89b74157e1cc974" },
                { "tl", "6aea2012bc0f17095691d3f26cb8bed3b30927862faae434f4e3933d0e1877431a69b8ce55fbff94a3cd5e5d0f99e7ee255b318c10101bf843dc1223639d7956" },
                { "tr", "1cef6edf5eddda12bf4189ce3ceadc7231e423a19d494cb0e4d87b430f739723756397174a40d73a2d2156a3eaae27058635937fffeee62ba7c6c81a709509cb" },
                { "trs", "4cd2ace5446f77ef8f089c0312d9612ca1efdcf6de572952c7cb82bd37fc8401150cc1fcf91f8eb941e88619d11b0eb0a25de7be45cb9c43db813d634206792b" },
                { "uk", "37ee11b8004fcd2be0dd2d27a34fe3cd294242eac9bbc7218a96d76d568859381438eadf2c6b263efc7e77288fd421254eb70dcb519f5ef4326634c37ef204b1" },
                { "ur", "db48a9c673b21845d7db534ae3012d2349f2221c7100a3e325ea95156f7c35b4505cc918cbaf234e6d29367fb712846020c243a9ef0d3c05807123e714d5829b" },
                { "uz", "bd9e5f67e2e93e91a2e958cb5c21c0ae4696fac1e7c8197c55314b282378fe0496f3208064d969669b3357aeacce8a83f3fc050970c4ad102015aee600f0f069" },
                { "vi", "388223fffd22f47b008bc8341b58ec4aeab5fdf368c848790506b8121fcc9ccfec99cd33edbeeab5e6edb5599b91759efbb382ba8a10ef7fa3c5e2ea643f94d8" },
                { "xh", "0ecca580b647a160bea0d03f919ff69f5efff5d982680772756219e248c8465194dc9f266d6be240a5ad5bb687b488a515b8cd021f011eb231966f6aa9dc5e7c" },
                { "zh-CN", "dbab770256c252ea484e3c56c2c777c1ab4ee35b66cc492943ee7149cd639660fe0443ac6e2682b2907258e47376b76a3de9e9412a3dba3e69ffd8187e1f18f2" },
                { "zh-TW", "ea5615a3c9741ef4931c9ad36ce88a727b91cf99e1a80b88058798f69faf74c074836d4d459103b2904330b8c54ecf5c7a8de9e4ee1ab1976334ca4e9d1fdb2b" }
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
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
