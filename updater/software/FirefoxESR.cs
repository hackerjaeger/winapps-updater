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
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param
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
            // https://ftp.mozilla.org/pub/firefox/releases/52.1.1esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "afd9c4f180444508fd8801a78a009cd0f6bca94048e4fd56e56374aaaa1122a6d026ecce419b1c81b747e60c15bec7c4828096e4e0c81f14783310eb5ef9711a");
            result.Add("af", "218a7f5062b9577b9e8d07278688790c14b07b4d9425115893888b126ada96c9d524ee2c08069ea0bf197efd3b217d70f850bb5de4664229031f16373a28be30");
            result.Add("an", "8765f2883831b1ae916f9ec3d8f717707678d75d3b6643370e8602a88a27af26045301ff3665cc05e21dad775eee6f1c2f938647343676b99aff9e460bf379d2");
            result.Add("ar", "1afe77f79ee491f4253cefe2095f8d9480cfb00db15bde79d99c48c0d7d05ce41d0087db8bb12ab288f853606fcfe876e5a62f0e3df0fdcfdfbbae19ad7d18e9");
            result.Add("as", "76a12da4d109688dc8297fc82e152e5f97bd34b37e9825ed9f49db5039c3b003d8d14adfc8c450807224f42c4130468f593f64e1d38c5700e2f0b4452489e95e");
            result.Add("ast", "ae59450b96d60217d708e2f2fdf315f33bea9fb1aec82f7d57395a98234df1f878abd772ba1cf8de30ca6daa963578d269a3e8761f864c2375ec7cad73556cea");
            result.Add("az", "9036250baabd17777c3a419bff4bfe4bab01293ca95430d4595c1683d355a8a239018869ff767a0cf8847cdd7300ed11d216fa740b1434c80d691fce7004bf86");
            result.Add("bg", "3b22fd12a36363682ecdb838c7342e651db0f4ba37380042a54a6dc754132dd0ffa6c9612d460bbf9a10e96555473973a2deeb3faa667d13677aee0f01e3ba05");
            result.Add("bn-BD", "f54d55023248f7e94b2d9ad7cc0bb2a91c0fee26cfb0b6954c31752d67fcf14aa235a1f389c574af3d3a209f80694f3b568665d89744754c73e9d30a13615c12");
            result.Add("bn-IN", "a2937d771f7815322dbcedbfc08d60ac7985fb52d10ebb9805b86ebee21e018a550850eaebd513ca8859ca168a8902e987f96497b6ec1c094a29f302a77299ef");
            result.Add("br", "acba305956abfefe75f91be948c9a43181a8bafbe38c4afaf13552fd424e5e7b73b9d227cee8255d668371921dff4ea9b3208eb7da73abffb00507151c07719f");
            result.Add("bs", "2db438d1cb6b18babbf5d4cb5e4ae5399b7ba710bac9c94e9c3009f38bc650be73b45596ac7ab5c3b937539d15acc2d439b6d455210acb0835d682cc4285ca89");
            result.Add("ca", "c14cc9f4efefcb7a357efb4bb303eb36fefe2cce67b4ce54b8c1dc221ea45bfa7c34e129957f32562af201bcb5046097c285f6c7b8deb569c6d9179ab2023b71");
            result.Add("cak", "0b79ace3e3e2e78bb56db6a72d557536f021b761f316e6064eae12259a6ebc10af3586e5bc1d08489badde58d21a9a9a52745e88f919fda7c38d4bc1c0ad7ce1");
            result.Add("cs", "b34868041941f4e0091fba3871b9a054619fc336863fc4e53bd823fdc5747511f30ecf27bb290fbe4d5a0d76a03ee07992f994ca464161d75ea000c0264facb8");
            result.Add("cy", "218c4990461f63dd1ed39b06eb386ba14fb9657d0375589ce57df296d36a3998028c557c1ae2c4f3bce34f4198c81bc0d7e01bdf673e22b04b3a09153a4f35f3");
            result.Add("da", "cb4066bf4c7604b6784b9fcae070a210afdf0034b5fcf1a5e25615a46a0ef865b9b5c40b3595e5c104510d371a28af3901499a9bfe01e62d408b8bad44c49225");
            result.Add("de", "ea7dc9e4de8660d8ea8e197f74d49b63a6449f6bcf2fd261013a419b318bbfa0b6cba7ad7286eda7be79d3d26d1b125c998fd9dcea83dff9c5d42a7589e9efb6");
            result.Add("dsb", "0652c461fb1852f43b652d1fafe1c7e236acd864e45d8ec7c937c0d1382ea8aa4b6f4e4514d87abaeaf42d69ffb110522564b7d6acabb37c59398553219ca131");
            result.Add("el", "104e85fe1b7329b082fe7e9719c9cdc5f894895b5173494aa1ee1dc995d695094207413d56a7d5b1f283ef3a85ea0125c260408c19ea4cde2298d1bf0b1d2d64");
            result.Add("en-GB", "d3b0cdf75e6ecbdc7583735958e0b7a792e8ece889c5c1e7c9a6517b8db8e825dddcadbd0aade6159b06679a2f1862fb652f9991bc2098d93bf4c4be841d39fa");
            result.Add("en-US", "f89b5ee615702485e0d95487e9bd37461ff1de3c8e8f49266e3175d261f39a2d534bbefeff44077232bd01e29db00a370dc1a4ac7efb0d4550beed615b257fef");
            result.Add("en-ZA", "b8b4617f9c79324242d1819bc112e7a84b6e7f961a65f957105c856fe9882d7a787db71c01b3ce27af323ae7bfd51df53d24c00d1386473c3d0387593b27f53d");
            result.Add("eo", "75791ed1f83b82d07553b75e482cb033c1d4f14fb4360409c795c871be6f7d562f10f070ab658a0679e65407bfc438c330073c504bb62a472b79238d8a6a82fd");
            result.Add("es-AR", "6c224eca00db46e11f0b0d76c08293fc3ae630db957013c880bb6f28ab42e018381bf7299dd77848162d262de82e2fdcc19d8a5deb0320b48a15188c24178791");
            result.Add("es-CL", "700b7b09521ef2ffde6bc0b9a9de295bc6d1267468e30c35df32b471749f1e7c3c17473d9da78902ed7a1bafe40d4017ea00d2404eba47cd6c6d7f325400818b");
            result.Add("es-ES", "431b9efd53de242a63606458557f4f67daca97172fcd25209cc8276f5ae489712423efa328b566fb771099b01c8c51a4cc27fd24695b2c9d38e97d016b64b77f");
            result.Add("es-MX", "f7d617792a68297352e362791a0d1fa30c2835890a2bd49ec42b4907e3652abc5019cdcf4c45fec6b3d6b527cc23fe24ec89b5f6bf1c0ac943f8100aa1835a6f");
            result.Add("et", "1a830b95f62769747c1f1309b4ed8b1db95bae84e54188270b4a09548a4b19197b8af4abbb7438c020e4665a9a319ba090d43c47d3a4301a12c778044f9a7d0d");
            result.Add("eu", "421bf498e830f65cbb5fdb0229ec272f4978cc522955d1f7eb97db411c74cbea0221ad26b76e426a05c77fda6521d547bcbacf8727757d305597a22099106e6f");
            result.Add("fa", "50d9194257d637aa28ec4cb382c687671ab4fb97af06568d9d2ebec06a9e39dbf560ede56e6b7728cb3d007dfdf255ae9a58eba7ebc4617b47f6fcf6ccd59691");
            result.Add("ff", "e151818f58686ca728907a889d50fa2767258ce49ef144b3e1e886106a507d923041bb34d1b6627c3d5c33ce053aa33d15f8da9dbfc8f8c6f3cc17ce71d303b4");
            result.Add("fi", "cbf0fafbde94ccefa957abf8044197b764c2dc20864ad5f5678ce54eebf7fc51fc8bb0b3628074d0aed3ed91bfe1a21f2a4e84e765de08ad5a822bd00167b517");
            result.Add("fr", "10d3ffb2a08ec990bd214eb7b1e576684244a187edecbda7a01475a8568d718531a6ea3c3600ce2751b7a75b26579541c892de86f0276d94476633d0763b8637");
            result.Add("fy-NL", "ccb1856630a1000ccab7dc6bb3d57791035166c9ccd1006e995fd2d0e1078c48bc93abf6d4016161c95cb91b616759c344b35654a2761276489e008da0ceeed0");
            result.Add("ga-IE", "46c4af0ca506a9fc99378dc1962eede856ae3e6325a278dd3ba04e4d8273cb67122070dd84d725a9920356a8fe92bdd725f8329279a2985ad50b962a46b569e0");
            result.Add("gd", "2b03eb6f19d41bf2e6fcd3737005d3c36f555f486a70e8bf11ebd4c5197b6ecd977d3249aa2eb15aded76fc94cf40133115d972129b72b323ddf3d26de0d8f1f");
            result.Add("gl", "926cbeb09273c8855a02933bc61ff403881f886d266124565e516cb9f56b0352ea2496b025a8340706d6bae625f4414d6a1e533b0bc13916483d3898a0ff11ac");
            result.Add("gn", "dbcad17fd97baf768c66ae82bedc05fe82b550dde687c2a38fc0764c02031260f656dfd4b86c2aeb8a1ef9914131efa42bbefbf1ad8b23b116b71d36dedcfcd7");
            result.Add("gu-IN", "57720214684cd594e87027fa2a81492105339428057d4396af722f309ba0e0f4b60c7b2636a12e1ccc4582425c810f16336ad7e6784c695594ed164f5bf93cb4");
            result.Add("he", "45e565a206bdd1731b0cd8baa771c48dea0da0408144883708ae3bf0b90f0f585c70d3470ec700e56207a43fedfc8164ecd840b576bc0b1f5a8a6145e94b0416");
            result.Add("hi-IN", "644ff6b23ed0deece8810553e5b58580c48e6205318f64f186ddb7f7c02cabd088a01addf97ab11a37448ac0a2863a8921d6b0097bf458ae2a5a092464d0d234");
            result.Add("hr", "08adaabf52da62cae667543c007cc4aaf9c5b0e95c32c3c22b1323cc386a4c9e081d268db6a0364ed8a6dcef4bd1d406a83711bb65624a23375706888764e004");
            result.Add("hsb", "6b873a003579a21f599a083c4963ffb4ef555dbf1270f22562350e347ec80d18f43b926d880221f6e21f8aaf2e80432401b5e77ebfdf3496255f263e3aeff45e");
            result.Add("hu", "7c551cf2159e00e161d8bda40d8fb59e91d33ef42b3e1575506e03961f1988056a2918a6386a792f596beb1ee07735ed6c9047433fc3e27537129fb9ba6b5ed6");
            result.Add("hy-AM", "6911daa3817356aeba8f7346abc955495331e86ced60c2f9a844802a90cfaf0bacacc632cdb0ba2d2560b0d0c1d6a9d49bc10d1d3a3f4b57e0068b80bd1500c3");
            result.Add("id", "69be3a51fd7a8d501974f6ac4c53c432365d0ec08c0cc5ed7bdadc0aefee99c5b6f91a68ba74894b883903cf06816ba01fe99758ef2e3e89a7bd360dd0721c38");
            result.Add("is", "a6d866a84e6f95382f9b61b4adc1ed0296fd154550abe0fc4da7af97a44da1b53236beadc3462f26262b53c6f3e7a412dc985ea57c301303b8ce453fdb5d0ac8");
            result.Add("it", "b6faa60a79683c2441c6b6699798dd6bff73cbc41bdd97471bc6f407c6dd7789ee5eda4b834515759351948c0df9ee229017a8b911fcacefe5fc67a5736531c7");
            result.Add("ja", "a90576335385f000b9e703553f251e687ff467b850670254da1ca70a16b2202446ad1bced2b7273d65832daee81883b74ff60962afe9b850a2c90cdd3e5bfb42");
            result.Add("ka", "26dd2d7c7577bf93eb2530ef40f0eb79971369cb82c4b83c82665951a8f201ee3394177ae83aac02ceef0c5df7d0cb8c2a0dcfc96c669c3420a68733d2ff617f");
            result.Add("kab", "3b588475c4dba4de24eb126547ceb3092868dd3479780517fcf6380ed7c355d57117b57a394d1bdcc1c688a516d63de7cf2fc2c074782eac32d3f6f422ede28a");
            result.Add("kk", "c377dd84088df0e77b33da8fb3bce93d40320da84f84f9d8187d77a291f9a23d311c2fd034ea5637984f999ab56bbc88ec2f837819da739e4f1964ba801a3317");
            result.Add("km", "9721338f92731c70f5675391184fd88e9acab9641069121b0e7759a21878bbf24c0e7d89c39ae9792f8f9dc6082936ffef24f292240b93666345736993ffad4b");
            result.Add("kn", "86d864d68d233ff0077095e9d5b3b2c36b7b24ff3bbe8b0bf15b653800045040d99e3d1bdb1cdb4ae88f6ad01667bd0ba8f25ab9f6474002c64b48124b9dbf36");
            result.Add("ko", "868bb16838959ac92343ce808ad7fdf392c6855c851e6736dfa665d162c6745f13c7763d5869d2bb47f47c5d38f814956dd1b5b43bc8053c316fc92fefd9b0b9");
            result.Add("lij", "75d7c20fef9faebc1d810e02d624c8c91fdffe1a753a609c450e52d84a3e789db91b8333fc16f675665e81471674a489799f8af0f9c0792162077f7dfcfd0f0a");
            result.Add("lt", "5718e4409d3365dfd434917bc77d5272a02a0977a313bdfc0dcc9c849d2fbbbe61514e06d87e863d3b0d996affaa0a4f305261bedb71fd7a4ddc23086d48d154");
            result.Add("lv", "4e558cc675e23f9b51424f831ebb422a5649b92e52cffa6d0af41b48ad8a4a6f0fa7d4d4a86305025892df17cd3977d553edf825f0c2be7cd661ca4fd2f038a1");
            result.Add("mai", "45a063d317e7ba4a4885fa239aae9620138a15ca7765e8d35de64ca0827972a737d2a73d3ee116a104f8fcbcd04abdce70cba404670dbd05cce9c9b7bf4b16f6");
            result.Add("mk", "e66e615b307086768ebcd50a1cee795ce65753b0e14992f00a609d1ae5455a6403dc68788cc37637bba21a22f24c0aef59843f80799efee87481fe89ca74b31e");
            result.Add("ml", "8fe609ff652adcbd6b3e1bedd5fc039f3d854571bfc91c53977aab1d8d3d0c482d0056b37846bb3a1de9b7c8cd9a845d5a17306bc312fd997e0c4c8c64e8aa83");
            result.Add("mr", "bc70d1962f3595188b5f3d6e6f95ce0ce8e9d1cd326b6247bf5c1b5e3cf43893be68c94893f0e1834659febe7d6e194352666c2e1ca2b1125f2fe8d38cdc803a");
            result.Add("ms", "6bc5dc6198ab56122313652643d184b2868cf55054c053f16682437165e10bb4e70a5ef6ef863333bd18a9b5fcd32ce6f642d699ce9b3b3a635014505125694a");
            result.Add("nb-NO", "7eab6f7261b032dc8ce2df2c5e452417730767c1b0546bc124789a4e20cdeca7563b6af3548a350b05b4c64c65d6c791aa2c25ce1a1ebe6ac98cb7ceb42da65f");
            result.Add("nl", "f2da5ce707b7afff4ee2a1b2df19f043b59d9bda4a8942277515ca39720921bf77440f7f03c3bb66802e6f5f8ad74577278f67215fdc071fc05525cd0d504eac");
            result.Add("nn-NO", "5ef52342badbb7c6d524cd49a3203280064d6f9f1d55a196c71141a6edcaea1c8c1d9d661b2b32fbe2c8a5a70aa77394392f0821d0c3d37e230b50891f68f145");
            result.Add("or", "2d1f9a8ed02085b74eebc05032fb39ce2e0ccb178ab0c2a61ffd86f989bd2e287256627d9ab4a53a63e82bcc2cc0efebf606552901e06c9314ee0406978590cc");
            result.Add("pa-IN", "accbc2aefb1b594048bc45c90ed4cad2c5e3e752ec2bc6eed8e3d6f305088d628e56c8c74b090677ef6e06961c4c5ed34366810d670eb8ad83a62803482d49ce");
            result.Add("pl", "24cb09b9012e0dcb9408e359288c4885fca9d15bd4a1079c5cab3d53bb0039bd79883f80b845ed444c7bd5c426e952de9228df7c38ce805a414b8fd9e0a5fe5a");
            result.Add("pt-BR", "1f3df31651290b228303739cb9e0dc05719a79b68ec7d46a0835c780fabeb844a78ee426da2c93e8471c9dcb16788bdc1da92fcd22d1f279e436bb5057266706");
            result.Add("pt-PT", "fc576ed3bdf13bf05bc8862aee6905493eed5e7968a32727e7455d469292d92ec403fa9e2e3e8267bff6044429cf75bbc3c2df285cac0d86d5e5135fa96d4f6a");
            result.Add("rm", "4e4a572626c4457f9a38d67e6a1407014a157c476894e19b9b92b43b3496e08e7fd9abfb68d7e4ca10429bf877792800c9a628c2cd263399f0ce67c59bf20714");
            result.Add("ro", "99cbe95728aa7bab56b1dc673efd5f792db92ce73d219ff078d320d709ef8f4d97cb90c1adc33482c62cd1096ca995aa8a4176c910304319fa715ca7e57bcc61");
            result.Add("ru", "824a10a3205322f15f968ee76ea789f079556111b36e49a3523a234484d51bed03bdaee8f4d9cdb864a73b0a1228388d2ed7086cc0c63402558db7f7fb6c331f");
            result.Add("si", "b3a3fe104fbe5be195b7ef264e0195a33092b2b4d455d251ee42fc3c0b00f93d19baf374e8694e239eb08f63d6c03d4c800d9f74f8a93cc5562640bbea518205");
            result.Add("sk", "c8c71a0f5db40c86053b04c941f397cdc20831b3afb8a965191ceac659e346ddcdb44347b1b5b38c8c80a9b86d8a8625cc814cd0350a6b5f49d301f8aa77ec54");
            result.Add("sl", "e64c76eb67bb83ded914776248aac5cbd7a8d08063a8f316c8fbd154728a37e0ed02a403b61a12799cdd43750bfb7e040f55afc0e9b94e7c41782f1592f94ab1");
            result.Add("son", "58f20360742114f3152127e790fd36e3ee0e7c2ba180fc55eaa81233c0ebdf1b3b55cbff02bf9730230b49fbaeeaf002f5f7777a4307376fff8afccdec5cbc3f");
            result.Add("sq", "382aed5cc069ede044073bb7afa466a82d330bc174e5354054f6332d7ae015f5da1cf6eef5f06c8d29f25dea2f5835b5b03515ddeedfd92c571f4ce782f68d14");
            result.Add("sr", "ed870b5c46737615d7736931631f5f41a1e33b34ed65a75c9d6439a74e56b97490cea1522770046bb2d4e5b0d29068d6490e52e128e7cb8e5d9a84dafcb762b4");
            result.Add("sv-SE", "7e6f9f8fb7839ec9b30cf116c75609dd805821daad4ad02faf15ec35a5d7cebb83a16788ff5aa0f79094a36c97b9201ee1d283efd080c0d438a175ec0406976e");
            result.Add("ta", "9c4207ddae9121ea7ee408b2af5794608c0444ea04d51957f6b376537018132339b6b502fe54a0bbefb850a41890c90a267899187a1655d7af9a0dccdbb6715e");
            result.Add("te", "5e33304c211bfd14dd31ab6d98faf0b726586e1c06bdb468a4611ba959f81d175d987a2d3754b1b879a25bc96c15947f953f0f283a36a97ade80e854d8c791f6");
            result.Add("th", "8095b3299dedd355d8dbec7483ba2aee13754c0bc11141fe52f37418531069bd82485982cf10832eef765ff6a76632a9a3932dff232c3b5dc196d7ea2d169d8a");
            result.Add("tr", "abca55ec118038cdb06019af0996c35d8218f91841f0c34dfd5faf082470a6af9ca787406a540b16890695fd5716be5e9d17af5a7b38ec73a3a3bc386324f885");
            result.Add("uk", "8cb232f047b2aedc9b56c503eb856bbd6a88a1479e9aae8f4f48e55496cef5079b27ae254d5685035f48d09d5430ad8d9d1549210909ca5ccb9f6aa9bcf625c5");
            result.Add("uz", "185c57c969a9a0671e002db06beaa7a852a023c61a60bd03374baf187b0b7be0583cb0cad20a40fd1620a1c6f9f6a5b90a2885fc134b27c97a0d4273ca200079");
            result.Add("vi", "96b5ece95bcf549c4fe860c7bc5e8601b65c4ca200b2cfee31e1365bb43086748a6425dfb72f7028f9a3c7f43435e4e756fe1ce8933e7629cd7f598991649543");
            result.Add("xh", "edc5e050cc093ac1c35261696400e7e1c8b353ef5f3c42c4757f37af9459a114e0351266ee6a231fca9ddf221242831057922f985a10d087e030026d2de917b4");
            result.Add("zh-CN", "1509e6cc965571b2ff7b0fd2c95fe5ba62f0976be0c0d2d68f1e6532fae182b6130eca5dafa95c7ede27c039b484db682c6506b170d94fab2b052cac6f69b144");
            result.Add("zh-TW", "d7bed62d74a6786b177105df11f0c7e11e49b7ba3e537af5ff5d7db9fa73bfc289d77492bb7a6d44e58cfc7bc662ab2d404c8012d532ad68055fb1c94dc15d46");

            return result;
        }


        /// <summary>
        /// gets a dictionary with the known checksums for the installers (key: language, value: checksum)
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/52.1.1esr/SHA512SUMS
            var result = new Dictionary<string, string>();
            result.Add("ach", "69e0320bb19645db0d54c04f081ae71eea55f80700eed482f533b1958ed72f5b1e460ae12f3aad30248ef4f836319feda5ba7198fa30082a596dfdf3b4730b5a");
            result.Add("af", "a1a1994384f0b5a0b4135764b62b9a9bdc5fa10170ae9ac2b25cf7a855a49ddcd8c16eee4a06051c786276a4504bab9d1700a7679f352ce1a2afd21ffbfdaced");
            result.Add("an", "fd0ef5d2c17831da6ab87fb079f06b218ce71a54b3243123ea7a735a93b6c6dddecca0c124ba21db4233b4c2bee22fc908f81fc6a71f93999de6d0a038f15386");
            result.Add("ar", "d1db65c46db8ddefb033e50d4055af8b9f225f34a3d5f7aa31f29b2d6698cdf4a1944781e690abf014eb9f0576c72d448e7108789de67b18a019989a5026c605");
            result.Add("as", "9c2e750fcf853ce3238df0e5b8e65c4cc45a7119cee222df74d5390d2b55cada41d5dcf544d002a590344f8e8953aa4c486c3b11061916697be69e82eb8c8c28");
            result.Add("ast", "7948c973271002a0844fcad114bdebfc72e001c0f598afe09ec48b8f7d683f18825ec7d1a7731d060ad7c22afda6b177d371aad1eddc39b816480c118b8274b7");
            result.Add("az", "839cac6c3ee5332f4946f3f00ba943bb7e09435f3098dc8950c3f69b1626d22bbe6b15820057f927c3b4f6feed15feb2259db3e16eaf9b0e6f4ca4bf8549da01");
            result.Add("bg", "7d533585a9ac75eaabf83b04fadbb915e3c102653653fbd1ea6a0086779ea7d5747f9acc3fba538cc06466f29c37af15af14fafcc6da6f3afa8248ae8c7d6770");
            result.Add("bn-BD", "f08929cf8a16f5b9dabb79a45643e91c6cafe9a8c45d021c689e40abffdd71c76cddc60e4c719dfed77b5c40c960b0efa452cff77b73d6c5d4c3e9a34ca581c9");
            result.Add("bn-IN", "1adbdfef0df9f12513d6cb339b4766675fba83c22d0630aefc70f84c22159307d0e350e82584ce6ad07cfcf029ac4699944f9d7782810d44453aff781d01e72c");
            result.Add("br", "72f85a1d3ec0e68d69e9961fdf35efb2872a95c58a9f30cb84ff302ae42b58b02b61f285d1d60579af955a51faf9cd92896a3b90216635b5537a0576ce5573ed");
            result.Add("bs", "a0bddb00ffd524876639b436475d50592c690004a214142b90be76da738d80c1f58f5dbed8a83fe302139568631279a87c5351dbfd154881a24d0e8db86c5f8f");
            result.Add("ca", "8d6eb0b5189fc9a3af9cfc8f73571556296fea1c0b6cd4092017771d13c72f282dd06fdc8020b27fefc498dc3272f1764cfe81b6a9017b8a6f6df1bcacf02723");
            result.Add("cak", "1389f92bf256a96f087dd6fbc06cd59c65456391bd91b6174ce8defc1f3c574a99b0d824d916fcae844833348048b4d06fc010c7d2ba66de19b21c847288b3f0");
            result.Add("cs", "1de57bbdb2809f05e6a52bf6cdfdc4b06f24d7a0f283c56396335046aac147fa75ab07778332e287db5fdd49b8987c85f98f4a102177b33bc2b00f555488b61b");
            result.Add("cy", "e1f1d3d1d75a1cc6a86b6c8a008f3ad339be381a80362a629a072425984139d75689e7d7c5c2ec6de3b9f7c5a54896a5e91f83c15cfd73eb5a864f489f0cf7dc");
            result.Add("da", "839121929211e877da634defee7a95469772a650cd4b529c63cdc8788659aea8704b7bd3f55817b2937630e6f109d3e8dbea4a1a605bc0b2c9088b8f38f9fe28");
            result.Add("de", "d2f11d4d22e8c6b6f225c0fe76570dbe796b3782a1301ce10e9d183d0493cb2377b13e784e9c0a1f334370d908cb90b85c62a0ad76be06a44885f69fa32c6261");
            result.Add("dsb", "abf881eba328107d5902bd12e986718544d39ead591159473ccaf6c339029a275c0e7b56dc5844b6732a9f3778b16fb280e96ff2ba94e69c2b185381dd19734a");
            result.Add("el", "14dbbd151119bb891b12337476d30451725f873df8918ccac059135071c32e2a27136f95de91810c83f58491f732717d5485e6678616c27f08fc10c100bad9a8");
            result.Add("en-GB", "054e2b3033a12518ba5163368711589f2fcbdc238bbd8a25a8ff88afd86c5c946131e7d19fc18753d0f9deb27c4dea0ceeff59de93f236a4d4e3b36e5918e7ac");
            result.Add("en-US", "2f9dd51f7b05a9e8f058ed9afae70fbc223e14b95340d42cab9c6935843d2e675bf2d90d941f6a40ca52d95bc78c4ff705a52309361b07c15eab21db56b7ec58");
            result.Add("en-ZA", "4376bf2f0f5f48e0cfdc695450447f342a3c31527300ad17ac9abc5dc9a84bcd886d62fba3d9cd40c6afbab7be914f55611c7f3c5cd0256aa9f8713ecada8423");
            result.Add("eo", "479d8a93739a5d961f61e2b7acbc0b5630f109a3833a5f3694e34760d754c4e0f14f0bd8d87bc7b44ce67ee2c87d5c85c8ec512ce2f4c7aa9f68330df9de7d03");
            result.Add("es-AR", "e7da79f045e28e63841409555757f98468ef34d17418b1eef42ca1a33d35de41cc7897c135492d464251935e2991791289b1fa1c41b3bd96a826bb287363ac79");
            result.Add("es-CL", "3ae93a2cd5eabb9b11f04a6cf9035ea0b51bf4a7f16ce5c710ca87c6c44b30353ad623590456462a98e1d04f20bc0df259923e8232aca5819463120555d90987");
            result.Add("es-ES", "1f33a175ef2c1828958c468233653b042d174ec6fc8cdb6d9d190e6ac82940974a14e21f1c8010826e32f71a356fd33af9f0fa602fc7e9b052958b4aae0a8d51");
            result.Add("es-MX", "25ce996f310959157528e9e401d3c414358a30b5a7106f4fb874c6666ebac78352a466a6ccdd1a2e83cc61f44a0cf92cac27b3f22f5edb0ccbed56e805c2cd96");
            result.Add("et", "e1b07f349ce0f247d99665f55882e5ef2cba87627b1dfe6940724d0e71863fd804ddf17fb2d4543758c520576b291b46177834df0177b2332d6a7537712cd6bd");
            result.Add("eu", "a460747de6358bd8c6bb983c6fc9d53eecc4443203332b42e51053fd562c2b79965b5e82f6d3ef3645512ff203d581e54db53e67562249e52a8b58c8d9d82f74");
            result.Add("fa", "1bf2c4f8477f24e68dd22601388740d1f7c1d3083754f49b1e614df01eea52bbc7c7680a0c42ad14a3ad6ac97f764a483e00a0a43e9ece635e196adda5ba38e0");
            result.Add("ff", "efb49620e59476451723700cd4d994ae48ad7f9244671dd991ba470321b4701b6bcd83cddeb3219b9e894c1f9bf4797409a56acab1c17e61804653ef85b812b0");
            result.Add("fi", "aead230e65df98b499fcce4ec07068a33695bdf25aae287d4e6ba1fb42848612ec8c8d89483208d8f63ede4405a8df83e7d90c750a8a43427ae255338b7653ac");
            result.Add("fr", "7494880d757b632c1adf71d43e5b6706dbf9a55a236faa2f08991fdfcd015a57fd79133c2e40459908fb58349c875428ee50ccd8b3e4d83eab63c17d188a0877");
            result.Add("fy-NL", "6c2a86f4a961bced20200721f76dfb42e7d468d9855de4f0e7c55c1823f12683b498aaebb3e67831203b402f9da22e3db0e5f7927e6f1a801010034f44da7b11");
            result.Add("ga-IE", "30faa3fb54c64e7639b0374b6e01b1d19dc8615a1bfb48b43a60901063cb8636da1c409ac160ceb58d6d36d6d9cbb1bac717d79ab26a89f0e9cc4e0f9fc7e3ae");
            result.Add("gd", "c77ca87678a5230485bfbb041941d350bb98726ac934568b4969216f62f90707c8b9bedc7bbe3253604cbae3a7b107c1fee5e25fd2e417aee216ad619525a325");
            result.Add("gl", "dfeed5fdc91939276aa4aa8c4cfe31bbcb3dc9c52e22a9fd10448d7b06e3f378ce2c6ac4f1527bca3c038235a48ac86d9ee043ba5072e4249850488dff45a99e");
            result.Add("gn", "9a18b67c6f5b9bdf0416be9c71612a247710cdff9e614338ae15906e30d1e30b44ced447e529f287bb1c9c3c0be056c41cf9030b49debbd6fcb6f1e932cd1443");
            result.Add("gu-IN", "6adac773c94109cbf1b25dd90a7fdfcd63f69746180f17f1442809c25fabd236dde7a6d8a822b6ed20393df508e286ba5c73b1853aff026e2f5518352ce2139c");
            result.Add("he", "b6240513dcffb57f13a484c48025533b8332a1f9a5ca8baf91a82ab9b3377020073dcd7449398811abbcd375dc71acc27eb4865b3655a2ea028c198ea1be686e");
            result.Add("hi-IN", "6c091257d02bb98de9f89591bcb8d748406e210102fa000e66d92ed11175b3922670be6fd0dd5bb80605ed5521f09d187b2c4ec7ceb8d1d90e825c686174ad75");
            result.Add("hr", "9f6d5abd61e04e7a18c24e4298625cb91dc91d7c554001e8e82071c6a362f3e6e881a8a901566e5518596c00a2a3b0c00b20272b44a953696e23d42a78d9442e");
            result.Add("hsb", "4aa252bfb78c606bf81141821ccaa47f96dd467c454e7ef59df17710a78bf04d696cc14b6d5276d85e03648f4887d3db10edc24e42512014628bc4832d403640");
            result.Add("hu", "d623166d4e0c78fa5ef756d0fb412bd16a7fe4eb54f1c980bb24410f3cdab8043fe5798f4bf881d4344505dbe936c26b0a5190fa9963fcbcec051fa0d9ccef11");
            result.Add("hy-AM", "858a597894d44e382e3108c32bf8772104f75554d23ab2756a51e225f69538aaf6496bfbc433c43a1cdcd143387aa75d5ad35ae958d166fbff1c52bbbee6bb52");
            result.Add("id", "ca516eb17254751fc20b6154b070499dfad52c073decb50aaf1249225f72516f02dd59f74d19f763c68e413e888b10f34572a54353b0e948fb8ff008d7ac6dab");
            result.Add("is", "0602a307b8053716ac05eb1ec53ce9c06282d10ea2bde093ca6fbb3f8ce58341036ef08698b1f5c66a7763bd8bc0d3e6c96a9b3244e300038dc30684037d2ca7");
            result.Add("it", "69cf8d6e92c49ed3a6c720a1aa04e286da9d2038b0d9baa5a8779ddeaa0373cf07fe08b630c3982e02fb0bbeaa22dea55360a1210da655e0f50c4695c0eac79d");
            result.Add("ja", "93c87296f9fabab66d990ff0f8b1ec477d34f02816e0348cbeb00f9c1dc8ab325453f50eaf739a5f40a86a5a1874b40a1d13e7a6584e1751571340ae21ba524e");
            result.Add("ka", "223d4cb0f878f9174741ab8b039cc759467017b66ef529347c95efa1d87212968a48c3e9c75a47b251c7aab56b07136f4bb9912ea07d0a630340662e7f6903c9");
            result.Add("kab", "a5d10422508efa1d79171317ec90bffb98e270d607076fd4d83f2e9a4c1d70efe79732927dcae60bb3bf47a1082d88aee99a00690b7b68d35d446c1e0d800b8b");
            result.Add("kk", "3bb7da0788d6fea8217162d099f82072d87fb66ff352d5e626558a5f48a19e69602f2fce1f9f5075240e671003f37ccc7988e74f9a5ed70c997d8f4a28738fe8");
            result.Add("km", "d0711992dc7c34a4154352c0b3b3af80f856864735bd307d022c17963a1a806f5ae5b3643bca381046556490d5fd24b8de2cd7e3fcc38b7502c8d83536d6a4e3");
            result.Add("kn", "1d841e6b426ec97c482eec80497d73cfbba0a8713accb0b8840338c3637ba28f735a3389c86d3e66eca3435b063105a58f6119b7aa714ac385ffc4d2c4ebbf16");
            result.Add("ko", "e2dff1f4c140e17ea6c871334f2f18cec770b8dddd945fe481c3332e45b1238761a5317084b53f594108500ba1a5e0f807aff674375a80b3051ca9b5799d9861");
            result.Add("lij", "ef6d5c2e0257bd0851b9534085a979b0029cb26bdae1b06080ea752d1a4455de532f92f5434248bf2324f78a12ae79b966043493d868dbe127f6c1d1310b8d23");
            result.Add("lt", "23e59116987c77a85471ed38afb4709c1eb02d317b310d67b1a672c0b3d5450335aeae12b2b6fbfe8763d46b5be3f917ef8f27c7116237735a4641ba4c14f9ef");
            result.Add("lv", "e66facc24dc283ca54414c1bfdaddb540b0b08b82570e655c362521275a793ed420f0ecd41db482e2c79e1aad293be2f46fe7373f140ccb45c6d7d32d31a12bb");
            result.Add("mai", "ffe0af8b03a4c402f2c58ebdd93c880e19e1934dc5924ca1b810e05964e567c255f27efe7bef1f6805ee35442570fcb1d162184ef9a30fd8c51d3e1824bbb170");
            result.Add("mk", "481879b4f94b49bcc492811d0a2afd8c1763ddea277e4abc45a4316955b7e4b2f44211d2b0d79f487a05d0719252ee6cc4ae73caf724663b1bc47cc11451fbe8");
            result.Add("ml", "aed4d8be18b691e870b0cd5d65f8ec688bfbba2b8ae139d8ebb00188b50ef3b76518abbf59fbeec2cf0d988a3a184075ab3426869c2dbad93195747077b68f8b");
            result.Add("mr", "ef592bc56997fcc54bf5149758dcdab0e222d4d227138d696231e16de8eafd49479c90b9106f86dce5275ea7473a9f07659e3749b53b2852c8484582460457dc");
            result.Add("ms", "8b06e28424fcc2944a7ec69fb0f9797a82d2303be16def459c8eeae1a8271647bdf4f5378ba4da2c6772435f183bf47e6d63002ee04f78bdf5b11c8025d518d9");
            result.Add("nb-NO", "0521dc66e9614ddcd8a054c8908471bbdaab818bb764086e86bc1cfbf1814346ecbf7fa5612f929cf2e02a1e24a86a12d3c651c4167ac320c5570b15c04046f6");
            result.Add("nl", "48c5a6aef911e699cd9dfdb119f98cdc61fbba4cfdb5ea22f6f0c5dc40cd1748c2720059665d2fb3b30ba34893905e5afceca4659ed1f87d982c44704db39e81");
            result.Add("nn-NO", "99e4489bfa8d0b5f2c8c5dc49036df4169390331d9a746a50cce83f7f3aa5a3c7f7b6e8a2e2cbf3b3290b34c0e269db3c4e255a5dc0c15347ff7db83f7bba9c0");
            result.Add("or", "f3dc1f17e8f52fd41841d4a33d97ed32e66aeed9c95e1d8731dc1f533241b8ca4074486939426473581f5ca78bdd2c7b231fcf9a05059dc2a5b5e13406e55a33");
            result.Add("pa-IN", "3ce5f4547c052c4c892ab4b9afafa8edd2de9ed38e197e95704e450b144de50df22b3b3cf91e326f616caf33acda2cc63a02aa82ef37933beb8003f123dfa258");
            result.Add("pl", "691ac69084e57de209d820afcfa2f00ad00dfc690a544aaec06a2d154532e884d5ebf883f975a7d2a754e5afb8cdeb64aefd211e98270e2bbc4a9325912a0b23");
            result.Add("pt-BR", "d96aef7e91d62b6f2c46b93c4fd12afa6bc3a4974f9b36f1dccb556b7d8cb5af3d3b9df6d0bdaf5c2077aee52fa5ec050cb0af074b1167ca3211941c823369a9");
            result.Add("pt-PT", "8ecb253b57f509a5e6a18896081a238234be2be4543632c044f4f0364a3582da9ba96760b4b7f0856f5b8d0e278b0ec174580f7a24838ada61a01c8305731d14");
            result.Add("rm", "ea1aaf32f0a8f4819e1ab363a06e168fea5d47e6ec4d3378ed73ee13a86685881e22807dcaae1951b254c994928b5d8ae5a5b6d4996f8beca0db1cfb9b0fe8e6");
            result.Add("ro", "af77b0a27664de9f652db7deed15c0be65e272b8e3f24c56dcbddcaa5c9ff29d14c4725e6a1037f45c55d105b2b77bab56ea51112a11ae64a854b169e507275e");
            result.Add("ru", "bb15e7e2a33297a3cbd56831984e88718bf6aee35a00c3cce4ed13fcc1cdb645c61a7206d4e77b12e5db0de4066c84bdf7d4cb7c85e311f7e365eca611f265a8");
            result.Add("si", "2fbcf4203f6c771213a2ab199eddfda4bc1347b82b001a278cd3f4cadf00c896cf4c6569a23e7be1ab67efbdfc1a0355831a91a40e7047cadcebcf7888871238");
            result.Add("sk", "575e8c91d2b82832f62cba539fadeb97ba83b00928034f92b488c4dfbe15a695c4266dfe095c7167d5286a03a4bad36cef9460a58bf188f4cea1612b7615666c");
            result.Add("sl", "b26a8d8473b202e11c298147bc615eaa5dc534d11c1b302293a2d83097c2ba281bce6aeee325474a3454a37857c513d2d2d7bb04ee01b08eb299c8c12fd5dd67");
            result.Add("son", "c7d21c795574ac646fdb915ab8f26827db0f811dba95e72acc261f22409f8ec1d786fd6855adc25c2f39ecd213676de1569b1ea0ae5b58cb00aef793a79f997f");
            result.Add("sq", "02c841cd22f48b36ca46258bcb64e6b27948d87da0f85d77aa6ce03dc64855d24f405f83997e609c5d2fb0c598c7b6f8436fcc531685719727725242408ea27b");
            result.Add("sr", "3d5a4314f480247eb20766264b65bd0655c6f53b67214d4475a3e801c71563d7a90a28425f4029b84ccc7fee4a49322928d537f12d538474e6070191e6a74e17");
            result.Add("sv-SE", "d77baaba2bbef6d16ac0e57c1f3a1cabbdc9c88472fd96d651c26173dcf082f550ac56c53f5970bfcf1676fa5e0fccd53bbc04b31892fa322d25ced206cc4b3f");
            result.Add("ta", "ed63ef055e4b2b1ba1e0d8833e274d681013a262eab5f4cbffabd1636407af849091b03cab2dd1abf961b658f8ebc57f79f366ec016b29a7282183b57d34a7ba");
            result.Add("te", "868ae503fe822f088b6800716c142b4a387d3ce9004552aa9b0cb460ee0835864571af450c4493eb202320e4e57306d51fe5cc3a2bd21e33b5143cd3ca1b837b");
            result.Add("th", "0c9e36c30a8b8fa8eb702da2a3c34c8b9e2a43bef2d526a67e12f9ed3f6e01178c68366092b745b770bf3c80f70eeb2d19c0999347acfac7521ae5d5cd5cba15");
            result.Add("tr", "782a83cb844b56302faafca0e97e99b3795154da873efa48f96d24d7ae916f4c755daa10e1991783001afc39d4d09bf530204d9d1ea7e577197b0f68a811951b");
            result.Add("uk", "c2e9d81214228432dbdbcd81a5c0442218f79a3588db85512a2c9736759e412c422973aca0f2e227cee3616730408cc2f1feded6330d7f279158c2800783075d");
            result.Add("uz", "833ccfc1f0dbf8241f0cfcbfb81d56791d8f034b5241a73f17a7304db22401e4f22c3e0d72abe861c991742273ddf03c836b452a5d0a5d8ec7670a2ec00356ea");
            result.Add("vi", "1b480cd520e3ee5ca830e82a8edd7b2dc96806f7336404def19d7eb304bbf4ba37349312c5cef989d17b3a713fc69e24d71fce331a3977af2ea60ce974f53f6e");
            result.Add("xh", "39047af255a0efa6499c556c350bd6c24b5d0f7ace8f84aa65abd7f3799aa17fe20b295d5f20429bd9970532025ff256cbf035af5d5db1ae9f1f2078a4d6af95");
            result.Add("zh-CN", "82859b2e0c520ed04b877c585c0bd5d0715802b0ef5f634d93936cb9b1559dd6c2fdc0863c89cac9a33c014c1c27224ab9809628ab0b7c9ee7b75ad0bd229497");
            result.Add("zh-TW", "ba90ee45dc19ea6d4359840a91e60535d24b0965877192e65112f8cf237c24bbd9a30dd0d3b270565c6dcdfa92731bb29ac0ce10d3fd3d9960bbc039e20d4575");

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
            const string knownVersion = "52.1.1";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9](\\.[0-9])? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                //32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    "-ms -ma",
                    "C:\\Program Files\\Mozilla Firefox",
                    "C:\\Program Files (x86)\\Mozilla Firefox"),
                //64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
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
